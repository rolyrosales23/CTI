using GestCTI.Core.Enum;
using GestCTI.Core.WebsocketClient;
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
        /// <summary>
        /// Factory of message
        /// </summary>
        /// <param name="messageType">Typeof message</param>
        /// <param name="message">Message</param>
        /// <param name="clientId">Client connection id</param>
        public static void WebsocksCoreFactory(MessageType messageType, String message, String clientId, WebsocketCore core)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<Websocket>();
            var client = hubContext.Clients.Client(clientId);

            switch (messageType)
            {
                case MessageType.CallIn:
                    return;
                case MessageType.CTIMakeCallRequest:
                    client.addCTIMakeCallRequest(message);
                    return;
                case MessageType.Initialize:
                    client.addInitialize(message);
                    return;
                case MessageType.HeartBeat:
                    // client.Notification(message);
                    return;
                case MessageType.CTISetAgentState:
                    client.logInAgent(message);
                    return;
                case MessageType.CTILogOut:
                    core.attendRequest = false;
                    client.logOutCore(message);
                    return;
                case MessageType.CTIGetAgentInfo:
                    client.getAgentInfo(message);
                    return;
                case MessageType.LoginAuxWork:
                    client.getLoginAuxWork(message);
                    client.Notification("AM_LOG_IN_AUX_WORK_SUCCESS");
                    break;
                case MessageType.AM_READY:
                    client.getAmReady(message);
                    client.Notification("READY_TO_WORK_SUCCESS");
                    break;
                case MessageType.ON_EVENT:
                    client.onEventHandler(message);
                    break;
                case MessageType.CTIAnswerCallRequest:
                    client.receiveAcceptCallRequest(message);
                    break;
                case MessageType.CTIClearConnectionRequest:
                    break;
            }
        }
    }
}