using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Core.WebsocketClient
{
    public class CTIRequestData {
        public String request { get; set; }
        public String[] args { get; set; }
    }

    public class CTIEvent
    {
        public String invokedId { get; set; }
        public CTIRequestData request { get; set; }
    }

    public class CTIErrorResponse {
        public String success { get; set; }
        public String result { get; set; }
    }
}