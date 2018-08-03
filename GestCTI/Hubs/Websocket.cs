using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GestCTI.Core.WebsocketClient;
using Microsoft.AspNet.SignalR;
using System.Net.WebSockets;
using GestCTI.Util;
using System.Collections.Concurrent;
using GestCTI.Core.Enum;

namespace GestCTI.Hubs
{
    public class Websocket : Hub
    {
        static ConcurrentDictionary<String, WebsocketCore> socks = new ConcurrentDictionary<string, WebsocketCore>();
        public async Task Send(String v1, String v2)
        {
            WebsocketCore ws = null;
            if (socks.TryGetValue(Context.ConnectionId, out ws) && ws != null)
            {
                // var ws = new HubCoreClient("ws://localhost:8000", "tester");
                String toSend = SystemHandling.Initialize("8006");
                await ws.Send(toSend, Guid.NewGuid(), MessageType.Initialize);
                // Call the Notification method to update clients.
                Clients.Client(Context.ConnectionId).Notification("Sended initialize command");
            }
            else
            {
                Clients.Client(Context.ConnectionId).Notification("No connection with websocket");
            }

        }

        /// <summary>
        /// Send command Initialize
        /// </summary>
        /// <param name="deviceId">Device id to initialize</param>
        /// <returns>void</returns>
        public async Task sendInitialize(String deviceId)
        {
            String toSend = SystemHandling.Initialize(deviceId);
            String I18n = "COMMAND_INITIALIZE";
            await genericSender(Guid.NewGuid(), toSend, MessageType.Initialize, I18n);
        }

        /// <summary>
        /// Generic sender to websocket core
        /// </summary>
        /// <param name="guid">messsage guid</param>
        /// <param name="toSend">Object to send</param>
        /// <param name="messageType">Typeof message</param>
        /// <param name="I18n">Internationalization</param>
        /// <returns>void</returns>
        private async Task genericSender(Guid guid, String toSend, MessageType messageType, String I18n)
        {
            WebsocketCore ws = null;
            if (socks.TryGetValue(Context.ConnectionId, out ws))
            {
                if (await ws.Send(toSend, guid, messageType))
                {
                    Clients.Client(Context.ConnectionId).Notification(I18n);
                }
                else
                {
                    Clients.Client(Context.ConnectionId).Notification("ERROR_SEND_MESSAGE_TO_WEBSOCKET");
                }
            }
            else
            {
                Clients.Client(Context.ConnectionId).Notification("NO_CONNECTION_WEBSOCKET");
            }
        }

        public void ConnectWebsocket()
        {
            var ws = new WebsocketCore(Context.ConnectionId);
            socks.AddOrUpdate(Context.ConnectionId, ws, (key, oldValue) => ws);
        }

        public override Task OnConnected()
        {
            ConnectWebsocket();
            Clients.Client(Context.ConnectionId).Notification("Conectado satisfactoriamente");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            WebsocketCore core = null;
            socks.TryRemove(Context.ConnectionId, out core);
            // core.Send("Test").Wait();
            core.Disconnect();
            Clients.All.addNotification("Server", "Desconexión satisfactoria");
            return base.OnDisconnected(stopCalled);
        }
    }
}