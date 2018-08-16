using GestCTI.Core.Enum;
using GestCTI.Core.WebsocketClient;
using GestCTI.Hubs;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;
using Newtonsoft.Json;

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

            if (messageType != MessageType.ON_EVENT)
            {
                int successIndex = message.IndexOf("success");
                String success = message.Substring(successIndex + 9, 1);
                if (success.ToUpper() == "F")
                {
                    try
                    {
                        CTIErrorResponse response = JsonConvert.DeserializeObject<CTIErrorResponse>(message);
                        client.Notification(success + response.result, "error");
                    }
                    catch (Exception) {
                        client.Notification("DESZERIALIZE ERROR: " + message, "error");
                    }
                }
                else if (success.ToUpper() == "T")
                    client.Notification(messageType.ToString() + " DONE!", "success");
                else
                    client.Notification(message, "warning");
            }

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
                    CTIEvent evento = JsonConvert.DeserializeObject<CTIEvent>(message);
                    String   eventName = evento.request.request;
                    String[] eventArgs = evento.request.args;

                    switch (eventName) {
                        case "onEndCall":
                            {
                                String ucid = eventArgs[0];
                                String username = core.CtiUser.user_name;
                                ConcurrentDictionary<String, List<HoldConnection>> hc = Websocket.holdConnections;
                                List<HoldConnection> lista;
                                hc.TryGetValue(username, out lista);
                                lista.RemoveAll(element => element.ucid == ucid);
                                hc.AddOrUpdate(username, lista, (key, oldValue) => lista);
                                client.onEventHandler(message, lista);
                                return;
                            }

                        case "onHoldConnection":
                            {
                                String username = core.CtiUser.user_name;
                                ConcurrentDictionary<String, List<HoldConnection>> hc = Websocket.holdConnections;
                                List<HoldConnection> lista;
                                hc.TryGetValue(username, out lista);
                                client.onEventHandler(message, lista);
                                return;
                            }
                        case "onRetrieveConnection": {
                                String username = core.CtiUser.user_name;
                                ConcurrentDictionary<String, List<HoldConnection>> hc = Websocket.holdConnections;
                                List<HoldConnection> lista;
                                hc.TryGetValue(username, out lista);
                                lista.RemoveAll(element => element.ucid == eventArgs[0]);
                                hc.AddOrUpdate(username, lista, (key, oldValue) => lista);
                                client.onEventHandler(message, lista);
                                return;
                            }
                        case "onTransferredCall": {
                                String username = core.CtiUser.user_name;
                                ConcurrentDictionary<String, List<HoldConnection>> hc = Websocket.holdConnections;
                                List<HoldConnection> lista;
                                hc.TryGetValue(username, out lista);
                                lista.RemoveAll(element => element.ucid == eventArgs[0]);
                                hc.AddOrUpdate(username, lista, (key, oldValue) => lista);
                                client.onEventHandler(message, lista);
                                return;
                            }
                        case "onConferenceCall": {
                                String username = core.CtiUser.user_name;
                                ConcurrentDictionary<String, List<HoldConnection>> hc = Websocket.holdConnections;
                                List<HoldConnection> lista;
                                hc.TryGetValue(username, out lista);
                                lista.RemoveAll(element => element.ucid == eventArgs[0]);
                                hc.AddOrUpdate(username, lista, (key, oldValue) => lista);
                                client.onEventHandler(message, lista);
                                return;
                            }
                    }

                    client.onEventHandler(message);
                    break;
                case MessageType.CTIAnswerCallRequest:
                    client.receiveAcceptCallRequest(message);
                    break;
                case MessageType.CTIClearConnectionRequest:
                    break;
                case MessageType.CTITransferRequest:
                    break;
                case MessageType.CTIRetrieveConnection:
                    break;
                case MessageType.CTIConferenceRequest:
                    break;
                case MessageType.InicializarApp:
                    client.InicializarApp(message);
                    break;
            }
        }
    }
}