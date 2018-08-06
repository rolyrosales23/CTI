using GestCTI.Core.Enum;
using GestCTI.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Core.Message
{
    public static class MessageFactory
    {
        public static void WebsocksCoreFactory(MessageType messageType, String message, String clientId)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<Websocket>();
            var client = hubContext.Clients.Client(clientId);

            switch (messageType)
            {
                case MessageType.CallIn:
                    return;
                case MessageType.CTIMakeCallRequest:
                    return;
                case MessageType.Initialize:
                    client.addInitialize(message);
                    return;
                case MessageType.HeartBeat:
                    client.Notification(message);
                    return;
                case MessageType.CTISetAgentState:
                    client.LogInAgent(message);
                    return;
            }
        }
    }
}