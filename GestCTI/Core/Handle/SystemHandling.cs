using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Util
{
    public class SystemHandling : HandlingBase
    {
        public static Tuple<Guid, String> Initialize(String deviceId){
            String[] Params = { deviceId };
            return makeRequest("Initialize", Params);
        }

        public static Tuple<Guid, String> CTIHeartbeatRequest(){
            String[] Params = { };
            return makeRequest("CTIHeartbeatRequest", Params);
        }
    }
}