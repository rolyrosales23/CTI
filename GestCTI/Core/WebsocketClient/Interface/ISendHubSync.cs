using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Core.WebsocketClient.Interface
{
    public interface ISendHubSync
    {
        void AddMessage(string name, string message);
        void Heartbeat();
    }
}