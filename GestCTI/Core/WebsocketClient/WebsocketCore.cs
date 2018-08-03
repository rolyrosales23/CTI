using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GestCTI.Core.Enum;
using GestCTI.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GestCTI.Core.WebsocketClient
{
    public class WebsocketCore
    {
        private const int sendChunkSize = 256;

        private const int receiveChunkSize = 256;

        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000);

        private readonly ClientWebSocket _ws = null;
        
        /// <summary>
        /// Structure to tracker a message
        /// </summary>
        private Dictionary<Guid, MessageType> InvokeId = new Dictionary<Guid, MessageType>();
        public WebsocketCore(String Client)
        {
            _ws = new ClientWebSocket();
            //Connect("ws://199.47.69.35:9102");
            Connect("ws://localhost:8000");
        }

        /// <summary>
        /// Connect to Websocket core 
        /// </summary>
        /// <param name="uri">Core url </param>
        private void Connect(string uri)
        {
            try
            {
                Uri url = new Uri(uri);
                _ws.ConnectAsync(url, CancellationToken.None);
                while (_ws.State != WebSocketState.Open)
                {
                    Thread.Sleep(1000);
                }
                Task.WhenAll(Receive(_ws));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
        }

        public void Disconnect()
        {
            if (_ws != null)
            {
                _ws.Dispose();
            }
        }
        static UTF8Encoding encoder = new UTF8Encoding();

        /// <summary>
        /// Send message to websocket core
        /// </summary>
        /// <param name="Message">Message to send</param>
        /// <param name="guid">Generated guid for message</param>
        /// <param name="messageType">Typeof message</param>
        /// <returns>Flag if is send succefull</returns>
        public async Task<bool> Send(String Message, Guid guid, MessageType messageType)
        {
            if (_ws != null && _ws.State == WebSocketState.Open)
            {
                byte[] buffer = encoder.GetBytes(Message);
                try
                {
                    // Save invokeId
                    InvokeId.Add(guid, messageType);
                    await _ws.SendAsync(
                        new ArraySegment<byte>(buffer),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex);
                    InvokeId.Remove(guid);
                    return false;
                }
            } else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Receive message from websocket
        /// </summary>
        /// <param name="webSocket">Client for websocket</param>
        /// <returns>void</returns>
        private static async Task Receive(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[receiveChunkSize];
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var stringResult = new StringBuilder();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await webSocket.CloseAsync(
                                WebSocketCloseStatus.NormalClosure,
                                string.Empty,
                                CancellationToken.None
                            );
                        }
                        else
                        {
                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            stringResult.Append(str);
                        }
                    } while (!result.EndOfMessage);
                }
            // message process
                
            } catch(Exception)
            {
                // Disconnect
            }
            finally
            {
                webSocket.Dispose();
            }
            // call factory receive message
        }
    }
}