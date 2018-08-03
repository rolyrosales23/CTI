using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace GestCTI.Util
{
    public class HandlingBase
    {
        protected static String format = "{{\"request\":\"{0}\",\"args\":[{1}],\"invokedId\":\"{2}\"}}";

        protected static String joinParams(String[] Params) {
            if (Params.Length == 0)  return "";

            return '"' + string.Join("\",\"", Params) + '"';
        }

        protected static Tuple<Guid, String> makeRequest(String Method, String[] Params) {
            StringBuilder builder = new StringBuilder();
            Guid invokedId = Guid.NewGuid();
            String request = builder.AppendFormat(format, Method, joinParams(Params), invokedId).ToString();
            return Tuple.Create<Guid, String>(invokedId, request);
        }
    }
}