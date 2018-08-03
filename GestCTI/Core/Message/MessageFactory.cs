using GestCTI.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Core.Message
{
    public static class MessageFactory
    {
        public static void WebsocksCoreFactory()
        {
            MessageType message = MessageType.CallIn;
            switch (message)
            {
                case MessageType.CallIn:
                    return;
                case MessageType.CTIMakeCallRequest:
                    return;
            }
        }
    }
}