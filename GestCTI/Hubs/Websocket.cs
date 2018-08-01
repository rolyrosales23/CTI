using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

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
        
        public void Send()
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNotification();
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