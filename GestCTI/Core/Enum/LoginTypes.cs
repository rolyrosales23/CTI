using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Core.Enum
{
    public enum AgentMode
    {
        AM_LOG_IN,
        AM_LOG_OUT,
        AM_NOT_READY,
        AM_READY,
        AM_WORK_NOT_READY,
        AM_WORK_READY
    }

    public enum WorkMode{
        WM_NONE = -1,
        WM_WORK = 1,
        WM_AFTER_CALL,
        WM_AUTO_IN,
        WM_MANUAL
    }

    public enum AgentState {
        AS_NOT_READY,
        AS_LOGGED_OUT,
        AS_READY,
        AS_AFTER_CALL,
        AS_WORK_READY
    }

    public enum TalkState{
        TS_ON_CALL,
        TS_IDLE
    }
}