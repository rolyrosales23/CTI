using System;
using System.Linq;
using System.Threading.Tasks;
using GestCTI.Core.WebsocketClient;
using Microsoft.AspNet.SignalR;
using GestCTI.Util;
using System.Collections.Concurrent;
using GestCTI.Core.Enum;
using GestCTI.Models;

namespace GestCTI.Hubs
{
    /// <summary>
    /// Class to get all websocket connection with Web App and core
    /// </summary>
    public class Websocket : Hub
    {
        // all core connection
        static readonly ConcurrentDictionary<String, WebsocketCore> socks = new ConcurrentDictionary<string, WebsocketCore>();
        private static readonly DBCTIEntities db = new DBCTIEntities();
        /// <summary>
        /// Log in an user in core app
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="agentId">Agent id</param>
        /// <param name="password">Password</param>
        /// <returns>void</returns>
        public async Task sendLogInAgent(String deviceId, String agentId, String password)
        {
            if (baseConnectWebsocket(agentId))
            {
                await CTISetAgentState(deviceId, agentId, password, (int)AgentMode.AM_LOG_IN, (int)WorkMode.WM_WORK, 0);
            } else
            {
                Clients.Client(Context.ConnectionId).Notification("SERVER_LOGIN_ERROR");
            }
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
            await genericSender(toSend.Item1, toSend.Item2, MessageType.CTISetAgentState, I18n);
        }

        public async Task sendLockOutAgent()
        {
        }

        /// <summary>
        /// Send command Initialize
        /// </summary>
        /// <param name="deviceId">Device id to initialize</param>
        /// <returns>void</returns>
        public async Task sendInitialize(String deviceId)
        {
            var toSend = SystemHandling.Initialize(deviceId);
            String I18n = "COMMAND_INITIALIZE";
            await genericSender(toSend.Item1, toSend.Item2, MessageType.Initialize, I18n);
        }

        /// <summary>
        /// Generic sender to websocket core
        /// </summary>
        /// <param name="guid">messsage guid</param>
        /// <param name="toSend">Object to send</param>
        /// <param name="messageType">Typeof message</param>
        /// <param name="I18n">Internationalization</param>
        /// <returns>void</returns>
        private async Task genericSender(Guid guid, String message, MessageType messageType, String I18n)
        {
            WebsocketCore ws = null;
            if (socks.TryGetValue(Context.ConnectionId, out ws))
            {
                if (await ws.Send(guid, message, messageType))
                {
                    Clients.Client(Context.ConnectionId).Notification(I18n);
                }
                else
                {
                    Clients.Client(Context.ConnectionId).Notification("ERROR_SEND_MESSAGE_TO_WEBSOCKET");
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
            bool answ = socks.TryGetValue(Context.ConnectionId, out core);
            if (!answ)
            {
                var User = db.Users.Where(p => p.Username == nameUser).FirstOrDefault();
                if (User == null)
                {
                    return false;
                }
                CtiUser cti_User = new CtiUser();
                cti_User.WebsocketUrl = User.Company1.Switch.WebSocketIP;
                cti_User.HttpUrl = User.Company1.Switch.ApiServerIP;
                cti_User.ConnectionId = Context.ConnectionId;

                var ws = new WebsocketCore(cti_User);

                socks.AddOrUpdate(Context.ConnectionId, ws, (key, oldValue) => ws);
                Clients.Client(Context.ConnectionId).Notification("SERVER_CORE_WEBSOCKET_CONNECTED");
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
        /// On disconect with  Web App
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            WebsocketCore core = null;
            socks.TryRemove(Context.ConnectionId, out core);
            if (core != null)
            {
                core.Disconnect();
            }
            // Clients.Client(Context.ConnectionId).Notification("SERVER_WEBSOCKET_DISCONECTED");
            return base.OnDisconnected(stopCalled);
        }
    }
}