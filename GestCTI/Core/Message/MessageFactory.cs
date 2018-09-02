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

            //decoding the message into a json
            JObject json_msg = JObject.Parse(message);

            if (messageType != MessageType.ON_EVENT)
            {
                JToken success;
                json_msg.TryGetValue("success", out success);
                if (success.ToString().ToUpper() == "FALSE")
                {
                    try
                    {
                        CTIErrorResponse response = JsonConvert.DeserializeObject<CTIErrorResponse>(message);
                        client.Notification(response.result, "error");
                    }
                    catch (Exception)
                    {
                        client.Notification("DESZERIALIZE ERROR: " + message, "error");
                    }
                }
                else if (success.ToString().ToUpper() == "TRUE")
                    client.Notification(messageType.ToString() + " DONE!", "success");
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
                    String eventName = evento.request.request;
                    String[] eventArgs = evento.request.args;
                    String username = core.CtiUser.user_name;

                    switch (eventName)
                    {
                        case "onEndCall":
                            {
                                // Delete current call
                                core.CtiUser.CurrentUCID = null;
                                String ucid = eventArgs[0];
                                HoldList hl = Websocket.holdConnections;
                                hl.removeElement(username, ucid);
                                client.onEventHandler(message, hl.getList(username));
                                return;
                            }
                        case "onEndConnection":
                            {
                                core.CtiUser.CurrentUCID = null;
                                client.onEventHandler(message);
                                return;
                            }
                        case "onHoldConnection":
                            {
                                // Delete current call
                                core.CtiUser.CurrentUCID = null;
                                String ucid = eventArgs[0];
                                String callId = eventArgs[1];
                                HoldList hl = Websocket.holdConnections;
                                hl.addElement(username, new HoldConnection(ucid, callId));
                                client.onEventHandler(message, hl.getList(username));
                                return;
                            }
                        case "onRetrieveConnection":
                            {
                                // Add ucid of call
                                core.CtiUser.CurrentUCID = eventArgs[0];
                                String ucid = eventArgs[0];
                                HoldList hl = Websocket.holdConnections;
                                hl.removeElement(username, ucid);
                                client.onEventHandler(message, hl.getList(username));
                                return;
                            }
                        case "onTransferredCall":
                            {
                                String ucid = null;
                                HoldList hl = Websocket.holdConnections;
                                hl.removeElement(username, ucid);
                                client.onEventHandler(message, hl.getList(username));
                                return;
                            }
                        case "onConferenceCall":
                            {
                                core.CtiUser.CurrentUCID = eventArgs[0];
                                String ucid = eventArgs[0];
                                HoldList hl = Websocket.holdConnections;
                                hl.removeElement(username, ucid);
                                client.onEventHandler(message, hl.getList(username));
                                return;
                            }
                        case "onEstablishedConnection":
                            {
                                // Add ucid of call
                                core.CtiUser.CurrentUCID = eventArgs[0];
                                client.onEventHandler(message);
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
                case MessageType.InicializarAppFase1:
                    {
                        HoldList hl = Websocket.holdConnections;
                        client.inicializarAppFase1(message, hl.getList(core.CtiUser.user_name));
                        break;
                    }
                case MessageType.InicializarAppFase2:
                    {
                        HoldList hl = Websocket.holdConnections;
                        client.inicializarAppFase2(message, hl.getList(core.CtiUser.user_name));
                        break;
                    }
                case MessageType.CTIClearCallRequest:
                    break;
                case MessageType.CTIHoldConnectionRequest:
                    break;
                case MessageType.Pause:
                    break;
                case MessageType.CTIWhisperRequest:
                    client.receiveWhisperRequest(message);
                    break;
                case MessageType.CTIListenHoldAllRequest:
                    client.receiveListenRequest(message);
                    break;
            }
        }
    }
}