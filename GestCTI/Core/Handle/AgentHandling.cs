using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

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

        public static Tuple<Guid, String> CTISetAgentState(String deviceId, String agentId, String password, int agentMode, int workMode, int reason){
            String[] Params = { deviceId, agentId, password };
            String str_params = joinParams(Params);
            String complete_str = str_params + "," + agentMode.ToString() + "," + workMode.ToString() + "," + reason.ToString();

            StringBuilder builder = new StringBuilder();
            Guid invokedId = Guid.NewGuid();
            String request = builder.AppendFormat(format, "CTISetAgentState", complete_str, invokedId).ToString();
            return Tuple.Create<Guid, String>(invokedId, request);
        }

    }
}