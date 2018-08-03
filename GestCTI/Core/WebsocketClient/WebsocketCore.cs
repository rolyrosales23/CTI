using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using GestCTI.Core.Enum;
using GestCTI.Core.Message;
using GestCTI.Hubs;
using GestCTI.Util;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json.Linq;

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
        private String clientSessionId;
        public WebsocketCore(String Client)
        {
            _ws = new ClientWebSocket();
            //Connect("ws://199.47.69.35:9102");
            Connect("ws://localhost:8000");
            clientSessionId = Client;
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
                Task.WhenAll(Receive(_ws), HeartBeat());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
        }

        /// <summary>
        /// Disconnect from websocket
        /// </summary>
        public void Disconnect()
        {
            if (_ws != null)
            {
                _ws.Dispose();
            }
        }

        static UTF8Encoding encoder = new UTF8Encoding();

        /// <summary>
        /// Sending heartbeat to core app
        /// </summary>
        /// <returns></returns>
        private async Task HeartBeat()
        {
            while (true)
            {
                Thread.Sleep(10000);
                // get heartbeat
                var toSend = SystemHandling.CTIHeartbeatRequest();
                if (!(await Send(toSend.Item1, toSend.Item2, MessageType.HeartBeat)))
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Send message to websocket core
        /// </summary>
        /// <param name="Message">Message to send</param>
        /// <param name="guid">Generated guid for message</param>
        /// <param name="messageType">Typeof message</param>
        /// <returns>Flag if is send succefull</returns>
        public async Task<bool> Send(Guid guid, String Message, MessageType messageType)
        {
            if (_ws != null && _ws.State == WebSocketState.Open)
            {
                byte[] buffer = encoder.GetBytes(Message);
                try
                {
                    // Save invokeId
                    if (messageType != MessageType.HeartBeat)
                    {
                        InvokeId.Add(guid, messageType);
                    }
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
            }
            else
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
        private async Task Receive(ClientWebSocket webSocket)
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

                    //Process a message
                    OnMessage(stringResult.ToString());
                }

            }
            catch (Exception)
            {
                // Disconnect
            }
            finally
            {
                webSocket.Dispose();
            }
            // call factory receive message
        }

        /// <summary>
        /// On message do factory
        /// </summary>
        /// <param name="message">Message from websocket core</param>
        private void OnMessage(String message)
        {
            // message process
            // Getting message type
            MessageType messageType = MessageType.HeartBeat;
            Guid guid;
            try
            {
                JObject json = JObject.Parse(message);
                JToken token;
                json.TryGetValue("invokedId", out token);
                String guidString = token.ToString();
                guid = new Guid(guidString);
                if (guid != null)
                InvokeId.TryGetValue(guid, out messageType);
                MessageFactory.WebsocksCoreFactory(messageType, message, clientSessionId);
            } catch(Exception)
            {
                return;
            }
        }
    }
}