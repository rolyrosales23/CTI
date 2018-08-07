using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Core.Enum
{
    public enum MessageType
    {
        CallIn,
        Initialize,
        CTIMakeCallRequest,
        HeartBeat,
        CTISetAgentState,
        UNDEFINED
    }
}