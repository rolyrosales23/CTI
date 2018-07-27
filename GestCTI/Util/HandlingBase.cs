using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace GestCTI.Util
{
    public class HandlingBase
    {
        protected static String format = "{{ request: \"{0}\", args: [{1}] }}";
        protected static StringBuilder builder = new StringBuilder();

        protected static String joinParams(String[] Params) {
            return '"' + string.Join("\",\"", Params) + '"';
        }
    }
}