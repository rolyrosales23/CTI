﻿using GestCTI.Core.Enum;
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
            switch (messageType)
            {
                case MessageType.CallIn:
                    return;
                case MessageType.CTIMakeCallRequest:
                    return;
                case MessageType.Initialize:
                    return;
            }
        }
    }
}