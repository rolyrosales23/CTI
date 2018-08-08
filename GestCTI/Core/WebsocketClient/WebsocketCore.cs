using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GestCTI.Core.Enum;
using GestCTI.Core.Message;
using GestCTI.Util;
using Newtonsoft.Json.Linq;

namespace GestCTI.Core.WebsocketClient
{
    public class WebsocketCore
    {
        private CtiUser CtiUser;
        private const int sendChunkSize = 256;

        private const int receiveChunkSize = 256;

        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000);

        private readonly ClientWebSocket _ws = null;

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        /// <summary>
        /// Structure to tracker a message
        /// </summary>
        private Dictionary<Guid, MessageType> InvokeId = new Dictionary<Guid, MessageType>();
        public WebsocketCore(CtiUser ctiUser)
        {
            _ws = new ClientWebSocket();
            // Connect("ws://199.47.69.35:9102");
            // Connect("ws://localhost:8000");
            CtiUser = ctiUser;
            Connect(ctiUser.WebsocketUrl);
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
                    Thread.Sleep(100);
                }
                // Task.WhenAll(Receive(_ws), HeartBeat());
                RunInTask(() => Receive());
                RunInTask(() => HeartBeat());
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
            // Only send one at a time
            await semaphoreSlim.WaitAsync();
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
                finally
                {
                    semaphoreSlim.Release();
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
        private async Task Receive()
        {
            byte[] buffer = new byte[receiveChunkSize];
            try
            {
                while (_ws.State == WebSocketState.Open)
                {
                    var stringResult = new StringBuilder();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _ws.CloseAsync(
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
                _ws.Dispose();
            }
        }

        private static void RunInTask(Action action)
        {
            Task.Factory.StartNew(action);
        }

        /// <summary>
        /// On message do factory
        /// </summary>
        /// <param name="message">Message from websocket core</param>
        private void OnMessage(String message)
        {
            // Getting message type
            MessageType messageType = MessageType.UNDEFINED;
            Guid guid;
            try
            {
                JObject json = JObject.Parse(message);
                JToken token;
                json.TryGetValue("invokedId", out token);
                guid = new Guid(token.ToString());
                if (guid != null)
                {
                    if (!InvokeId.TryGetValue(guid, out messageType))
                    {
                        messageType = MessageType.UNDEFINED;
                    }
                }
                if (messageType != MessageType.UNDEFINED)
                {
                    MessageFactory.WebsocksCoreFactory(messageType, message, CtiUser.ConnectionId);
                }
                else
                {
                    //Handle event
                    MessageFactory.WebsocksCoreFactory(MessageType.ON_EVENT, message, CtiUser.ConnectionId);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}