using System;
using System.Linq;
using System.Threading.Tasks;
using GestCTI.Core.WebsocketClient;
using Microsoft.AspNet.SignalR;
using GestCTI.Util;
using System.Collections.Concurrent;
using GestCTI.Core.Enum;
using GestCTI.Models;
using System.Collections.Generic;

namespace GestCTI.Hubs
{
    public class HoldConnection {
        private string deviceId;

        public HoldConnection(string ucid, string deviceId)
        {
            this.ucid = ucid;
            this.toDevice = deviceId;
        }

        public String ucid { get; set; }
        public String toDevice { get; set; }
    }
    /// <summary>
    /// Class to get all websocket connection with Web App and core
    /// </summary>
    public class Websocket : Hub
    {
        /// <summary>
        /// all core connection
        /// </summary>
        static readonly ConcurrentDictionary<String, WebsocketCore> socks = new ConcurrentDictionary<string, WebsocketCore>();
        /// <summary>
        /// DB Connection
        /// </summary>
        private static readonly DBCTIEntities db = new DBCTIEntities();
        /// <summary>
        /// List of hold connections
        /// </summary>
        private List<HoldConnection> holdConnections = new List<HoldConnection>();
        /// <summary>
        /// Log in an user in core app
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="agentId">Agent id</param>
        /// <param name="password">Password</param>
        /// <returns>void</returns>
        public async Task sendLogInAgent(String deviceId, String agentId, String password)
        {
            acceptLogout(agentId);
            if (baseConnectWebsocket(agentId))
            {
                WebsocketCore core;
                socks.TryGetValue(agentId, out core);
                core.CtiUser.DeviceId = deviceId;
                await CTISetAgentState(deviceId, agentId, password, (int)AgentMode.AM_LOG_IN, (int)WorkMode.WM_WORK, 0);
            }
            else
            {
                Clients.Client(Context.ConnectionId).Notification("SERVER_LOGIN_ERROR");
            }
        }

        /// <summary>
        /// Set the first mode of agent LOG_IN AUX_WORK
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <returns>void</returns>
        public async Task sendStateLoginAuxWork(String deviceId)
        {
            var toSend = AgentHandling.CTISetAgentState(deviceId, Context.User.Identity.Name, "", (int)AgentMode.AM_LOG_IN, (int)WorkMode.WM_WORK, 0);
            String I18n = "AGENT_LOGIN_AUX_MODE";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.LoginAuxWork, I18n, Context.User.Identity.Name);
        }

        /// <summary>
        /// Change agent to state READY
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task sendStateReadyManual(String deviceId)
        {
            var toSend = AgentHandling.CTISetAgentState(deviceId, Context.User.Identity.Name, "", (int)AgentMode.AM_READY, (int)WorkMode.WM_MANUAL, 0);
            String I18n = "AGENT_AM_READY";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.AM_READY, I18n, Context.User.Identity.Name);
        }

        /// <summary>
        /// Get Agent info
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public async Task sendCTIGetAgentInfo(String agentId)
        {
            var toSend = AgentHandling.CTIGetAgentInfo(agentId);
            String I18n = "COMMAND_GET_AGENT_STATE";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.CTIGetAgentInfo, I18n, agentId);
        }

        /// <summary>
        /// Change state of user
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="agentId">Agent Id</param>
        /// <param name="password">Password</param>
        /// <param name="agentMode">Agent Mode</param>
        /// <param name="workMode">Work Mode</param>
        /// <param name="reason">Reason</param>
        /// <returns>void</returns>
        private async Task CTISetAgentState(String deviceId, String agentId, String password, int agentMode, int workMode, int reason)
        {
            var toSend = AgentHandling.CTISetAgentState(deviceId, agentId, password, agentMode, workMode, reason);
            String I18n = "COMMAND_LOG_IN";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.CTISetAgentState, I18n, agentId);
        }

        /// <summary>
        /// Accept a call
        /// </summary>
        /// <param name="ucid"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task sendCTIAnswerCallRequest(String ucid, String deviceId)
        {
            var toSend = CallHandling.CTIAnswerCallRequest(ucid, deviceId);
            String I18n = "COMMAND_ANSWER_CALL";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.CTIAnswerCallRequest, I18n, Context.User.Identity.Name);
        }

        public async Task sendCTIClearConnectionRequest(String ucid, String deviceId)
        {
            var toSend = CallHandling.CTIClearConnectionRequest(ucid, deviceId);
            String I18n = "COMMAND_CLEAR_CONNECTION";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.CTIClearConnectionRequest, I18n, Context.User.Identity.Name);
        }

        /// <summary>
        /// Send hold request
        /// </summary>
        /// <param name="ucid">Ucid</param>
        /// <param name="deviceId">Device id</param>
        /// <returns>void</returns>
        public async Task sendCTIHoldConnectionRequest(String ucid, String deviceId) {
            holdConnections.Add(new HoldConnection(ucid, deviceId));
            var toSend = CallHandling.CTIHoldConnectionRequest(ucid, deviceId);
            String I18n = "COMMAND_HOLD_CONNECTION";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.CTIHoldConnectionRequest, I18n, Context.User.Identity.Name);
        }

        /// <summary>
        /// Get all holded connections for an user in this context
        /// </summary>
        /// <returns>void</returns>
        public void getHoldConnections() {
            // Sending message
            //holdConnections.Add(new HoldConnection("2508", "3025"));
            //holdConnections.Add(new HoldConnection("9009", "3003"));
            Clients.Client(Context.ConnectionId).resultHoldConnections(holdConnections);
        }

        /// <summary>
        /// Log out
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <returns>void</returns>
        public async Task sendLogOutCore(String deviceId)
        {
            var toSend = AgentHandling.CTISetAgentState(deviceId, Context.User.Identity.Name, "", (int)AgentMode.AM_LOG_OUT, 0, 0);
            String I18n = "COMMAND_LOG_OUT";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.CTILogOut, I18n, Context.User.Identity.Name);
        }

        /// <summary>
        /// Make a call
        /// </summary>
        /// <param name="fromDevice">this deviceId</param>
        /// <param name="toDevice">To another Device</param>
        /// <param name="callerId">Identifier of this call</param>
        /// <returns>void</returns>
        public async Task sendCTIMakeCallRequest(String fromDevice, String toDevice, String callerId){
            var toSend = CallHandling.CTIMakeCallRequest(fromDevice, toDevice, callerId);
            String I18n = "COMMAND_MAKE_CALL_REQUEST";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.CTIMakeCallRequest, I18n, Context.User.Identity.Name);
        }

        /// <summary>
        /// If accept logout
        /// </summary>
        private void acceptLogout(String agentId)
        {
            WebsocketCore core = null;
            if (socks.TryGetValue(agentId, out core) && !core.attendRequest)
            {
                socks.TryRemove(agentId, out core);
                if (core != null)
                {
                    core.Disconnect();
                }
            }
        }

        /// <summary>
        /// Send command Initialize
        /// </summary>
        /// <param name="deviceId">Device id to initialize</param>
        /// <returns>void</returns>
        public async Task sendInitialize(String deviceId, String user)
        {
            var toSend = SystemHandling.Initialize(deviceId);
            String I18n = "COMMAND_INITIALIZE";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.Initialize, I18n, user);
        }

        public async Task sendTransferCall(String heldUcid, String activeUcid, String deviceId) {
            var toSend = CallHandling.CTITransferRequest(heldUcid, activeUcid, deviceId);
            String I18n = "COMMAND_TRANSFER_REQUEST";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.CTITransferRequest, I18n, Context.User.Identity.Name);
        }

        /// <summary>
        /// Generic sender to websocket core
        /// </summary>
        /// <param name="guid">messsage guid</param>
        /// <param name="toSend">Object to send</param>
        /// <param name="messageType">Typeof message</param>
        /// <param name="I18n">Internationalization</param>
        /// <returns>void</returns>
        private async Task genericSender(Guid guid, String message, MessageType messageType, String I18n, String User)
        {
            WebsocketCore ws = null;
            if (socks.TryGetValue(User, out ws))
            {
                if (await ws.Send(guid, message, messageType))
                {
                    Clients.Client(Context.ConnectionId).Notification(I18n);
                }
                else
                {
                    //Don't have connection with core but you need to logout
                    if (messageType == MessageType.CTILogOut)
                    {
                        Clients.Client(Context.ConnectionId).logOutCore(null);
                    } else
                    {
                        Clients.Client(Context.ConnectionId).Notification("ERROR_SEND_MESSAGE_TO_WEBSOCKET");
                    }
                    
                }
            }
            else
            {
                Clients.Client(Context.ConnectionId).Notification("NO_CONNECTION_WEBSOCKET");
            }
        }

        /// <summary>
        /// Conection to websocket core
        /// </summary>
        public void ConnectWebsocket()
        {
            baseConnectWebsocket(Context.User.Identity.Name);
        }

        /// <summary>
        /// Connect to websocket with param user
        /// </summary>
        /// <param name="nameUser">User name</param>
        /// <returns>bool</returns>
        private bool baseConnectWebsocket(String nameUser)
        {
            WebsocketCore core;            
            bool answ = socks.TryGetValue(nameUser, out core);
            if (!answ)
            {
                //Getting user from databse
                var User = db.Users.Where(p => p.Username == nameUser).FirstOrDefault();
                if (User == null)
                {
                    return false;
                }
                CtiUser cti_User = new CtiUser();
                cti_User.WebsocketUrl = User.Company1.Switch.WebSocketIP;
                cti_User.HttpUrl = User.Company1.Switch.ApiServerIP;
                cti_User.ConnectionId = Context.ConnectionId;
                cti_User.user_name = nameUser;

                //Create websocket connection with core
                var ws = new WebsocketCore(cti_User);

                socks.AddOrUpdate(nameUser, ws, (key, oldValue) => ws);
                Clients.Client(Context.ConnectionId).Notification("SERVER_CORE_WEBSOCKET_CONNECTED");
            }
            else
            {
                core.setConnectionId(Context.ConnectionId);
            }

            return true;
        }
        /// <summary>
        /// On connection with Web App
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            // If user is authenticated
            if (Context.Request.User.Identity.IsAuthenticated)
            {
                ConnectWebsocket();
            }
            Clients.Client(Context.ConnectionId).Notification("SERVER_WEBSOCKET_CONNECTED");
            return base.OnConnected();
        }

        /// <summary>
        /// On disconect with Web App
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            // Clients.Client(Context.ConnectionId).Notification("SERVER_WEBSOCKET_DISCONECTED");
            return base.OnDisconnected(stopCalled);
        }
    }
}