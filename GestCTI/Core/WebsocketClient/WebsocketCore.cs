using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GestCTI.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GestCTI.Core.WebsocketClient
{
    public class WebsocketCore
    {
        private const int sendChunkSize = 256;

        private const int receiveChunkSize = 256;

        private const bool verbose = true;

        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000);

        private readonly ClientWebSocket _ws = null;

        private static IHubContext hubContext =
        GlobalHost.ConnectionManager.GetHubContext<Websocket>();

        public WebsocketCore(String Client)
        {
            _ws = new ClientWebSocket();
            //Connect("ws://199.47.69.35:9102");
            hubContext.Clients.All.addNotification("Server", "Esto es una prueba");

            Connect("ws://localhost:8000");
        }
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

        public async Task Send(String Message)
        {
            //byte[] buffer = encoder.GetBytes("{\"op\":\"blocks_sub\"}"); //"{\"op\":\"unconfirmed_sub\"}");
            if (_ws != null && _ws.State == WebSocketState.Open)
            {
                byte[] buffer = encoder.GetBytes(Message);
                try
                {
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
                }
            }
        }

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