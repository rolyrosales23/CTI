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
        UNDEFINED,
        CTILogOut,
        CTIGetAgentInfo,
        LoginAuxWork, // Change state of agent AM_LOG_IN with AUX_MODE
        AM_READY,
        ON_EVENT, // When receive an event
        CTIAnswerCallRequest, // Handle call request
        CTIClearConnectionRequest
    }
}