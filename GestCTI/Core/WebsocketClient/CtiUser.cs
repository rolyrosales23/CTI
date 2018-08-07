using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Core.WebsocketClient
{
    public class CtiUser
    {
        public String user_name { get; set; }
        public String phone_extension { get; set; }
        public String WebsocketUrl { get; set; }
        public String HttpUrl { get; set; }
        public String ConnectionId { get; set; }
        public String DeviceId { get; set; }
    }
}