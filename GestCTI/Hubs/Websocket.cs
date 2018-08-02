﻿using System;
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
    public class User
    {
        public string idConnection;
        public string name;
    }
    public class Websocket : Hub
    {
        static List<User> Users = new List<User>();
        static ConcurrentDictionary<String, WebsocketCore> socks = new ConcurrentDictionary<string, WebsocketCore>();
        public async Task Send(String v1, String v2)
        {
            var ws = new WebsocketCore(Context.ConnectionId);
            socks.AddOrUpdate(Context.ConnectionId, ws, (key, oldValue) => ws);
            // var ws = new HubCoreClient("ws://localhost:8000", "tester");
            String toSend = SystemHandling.Initialize("8006");
            await ws.Send("{\"request\":\"Initialize\",\"args\":[\"8006\"],\"invokedId\":\"07f25e72-d2d3-4a48-ff80-b5f9b1f84ae5\"}");

            // Call the Notification method to update clients.
            Clients.All.addNotification(v1, v2);
        }

        public override Task OnConnected()
        {
            Users.Add(new User
            {
                idConnection = Context.ConnectionId
            });
            
            Clients.Client(Context.ConnectionId).addNotification("Server", "Conectado satisfactoriamente");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = from item in Users
                       where item.idConnection == Context.ConnectionId
                       select item;
            if (user.Count() > 0)
            {
                Users.Remove(user.FirstOrDefault());
            }
            Clients.All.addNotification("Server", "Desconexión satisfactoriamente");
            return base.OnDisconnected(stopCalled);
        }
    }
}