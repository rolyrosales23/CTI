using GestCTI.Core.Service;
using GestCTI.Core.Util;
using GestCTI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestCTI.Controllers
{
    public class CoreHttpController : Controller
    {
        public CoreHttpController() { }
        /// <summary>
        /// Queue Calls from campaigns
        /// </summary>
        /// <param name="campaigns">List of Campaigns</param>
        /// <returns>List<Calls></returns>
        public async Task<String> QueuedCalls(String id)
        {
            var result = await ServiceCoreHttp.QueueCallsResult(id);
            JsonConvert.SerializeObject(result);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Add a new agent to core app
        /// </summary>
        /// <param name="agentId">Agent id</param>
        /// <param name="name">Agent name </param>
        /// <param name="passw">Agent passw</param>
        /// <returns>Boolean</returns>
        public async Task<Boolean> AddAgent(String agentId, String name, String passw)
        {
            return await AddAgent(agentId, name, passw);
        }
    }
}