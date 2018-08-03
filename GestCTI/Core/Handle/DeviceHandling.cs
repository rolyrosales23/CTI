using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Util
{
    public class DeviceHandling : HandlingBase
    {
        public static Tuple<Guid, String> CTIGetCalls(String deviceId){
            String[] Params = { deviceId };
            return makeRequest("CTIGetCalls", Params);
        }

        public static Tuple<Guid, String> CTIGetDeviceInfo(String deviceId){
            String[] Params = { deviceId };
            return makeRequest("CTIGetDeviceInfo", Params);
        }
    }
}