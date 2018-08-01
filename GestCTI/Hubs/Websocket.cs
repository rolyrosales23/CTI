using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GestCTI.Core.WebsocketClient;
using Microsoft.AspNet.SignalR;
using System.Net.WebSockets;

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

        public void Send(String v1, String v2)
        {
            // Call the Notification method to update clients.
            Clients.All.addNotification(v1, v2);
        }
        public override Task OnConnected()
        {
            Users.Add(new User
            {
                idConnection = Context.ConnectionId
            });
            var ws = new WebsocketCore();
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