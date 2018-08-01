using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Core.WebsocketClient.Interface
{
    public interface IRecieveHubSync
    {
        void Recieve_AddMessage(string name, string message);
        void Recieve_Heartbeat();
    }
}