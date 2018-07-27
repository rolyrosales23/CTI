using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Util
{
    public class DeviceHandling : HandlingBase
    {
        public static String CTIGetCalls(String deviceId){
            String[] Params = { deviceId };

            return builder.AppendFormat(format, "CTIGetCalls", joinParams(Params)).ToString();
        }

        public static String CTIGetDeviceInfo(String deviceId){
            String[] Params = { deviceId };

            return builder.AppendFormat(format, "CTIGetDeviceInfo", joinParams(Params)).ToString();
        }
    }
}