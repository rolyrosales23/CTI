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

namespace GestCTI.Hubs
{
    public class Websocket : Hub
    {
        static ConcurrentDictionary<String, WebsocketCore> socks = new ConcurrentDictionary<string, WebsocketCore>();
        public async Task Send(String v1, String v2)
        {
            WebsocketCore ws = null;
            if (socks.TryGetValue(Context.ConnectionId, out ws))
            {
                // var ws = new HubCoreClient("ws://localhost:8000", "tester");
                String toSend = SystemHandling.Initialize("8006");
                await ws.Send("{\"request\":\"Initialize\",\"args\":[\"8006\"],\"invokedId\":\"07f25e72-d2d3-4a48-ff80-b5f9b1f84ae5\"}");
                // Call the Notification method to update clients.
                Clients.Client(Context.ConnectionId).addNotification(v1, v2);
            } else
            {
                Clients.Client(Context.ConnectionId).Notification("No connection with websocket");
            }
            
        }

        public void ConnectWebsocket()
        {
            var ws = new WebsocketCore(Context.ConnectionId);
            socks.AddOrUpdate(Context.ConnectionId, ws, (key, oldValue) => ws);
        }

        public override Task OnConnected()
        {
            Clients.Client(Context.ConnectionId).addNotification("Server", "Conectado satisfactoriamente");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            WebsocketCore core = null;
            socks.TryRemove(Context.ConnectionId, out core);
            core.Disconnect();
            Clients.All.addNotification("Server", "Desconexión satisfactoria");
            return base.OnDisconnected(stopCalled);
        }
    }
}