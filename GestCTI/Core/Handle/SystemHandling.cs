using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Util
{
    public class SystemHandling : HandlingBase
    {
        public static String Initialize(String deviceId){
            return builder.AppendFormat(format, "Initialize", '"' + deviceId + '"').ToString();
        }

        public static String CTIHeartbeatRequest(){
            return builder.AppendFormat(format, "CTIHeartbeatRequest", "").ToString();
        }
    }
}