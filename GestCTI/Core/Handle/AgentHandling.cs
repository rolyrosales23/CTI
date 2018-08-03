using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Util
{
    public class AgentHandling : HandlingBase
    {
        public static Tuple<Guid, String> CTIGetAgentInfo(String agentId){
            String[] Params = { agentId };
            return makeRequest("CTIGetAgentInfo", Params);
        }

        public static Tuple<Guid, String> CTIGetAgentState(String agentId){
            String[] Params = { agentId };
            return makeRequest("CTIGetAgentState", Params);
        }

        public static Tuple<Guid, String> CTISetAgentState(String deviceId, String agentId, String password, long agentMode, long workMode, long reason){
            String[] Params = { deviceId, agentId, password, agentMode.ToString(), workMode.ToString(), reason.ToString() };
            return makeRequest("CTISetAgentState", Params);
        }

    }
}