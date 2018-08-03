using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Util
{
    public class AgentHandling : HandlingBase
    {
        public static String CTIGetAgentInfo(String agentId){
            String[] Params = { agentId };

            return builder.AppendFormat(format, "CTIGetAgentInfo", joinParams(Params)).ToString();
        }

        public static String CTIGetAgentState(String agentId){
            String[] Params = { agentId };

            return builder.AppendFormat(format, "CTIGetAgentState", joinParams(Params)).ToString();
        }

        public static String CTISetAgentState(String deviceId, String agentId, String password, long agentMode, long workMode, long reason){
            String[] Params = { deviceId, agentId, password, agentMode.ToString(), workMode.ToString(), reason.ToString() };

            return builder.AppendFormat(format, "CTISetAgentState", joinParams(Params)).ToString();
        }

    }
}