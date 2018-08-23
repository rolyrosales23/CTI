using GestCTI.Core.Service;
using GestCTI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GestCTI.Controllers
{
    public class CoreHttpController : Controller
    {
        /// <summary>
        /// Queue Calls from campaigns
        /// </summary>
        /// <param name="campaigns">List of Campaigns</param>
        /// <returns>List<Calls></returns>
        public async Task<List<Calls>> QueuedCalls(List<String> campaigns)
        {
            return await ServiceCoreHttp.QueueCallsResult(campaigns);
        }

        /// <summary>
        /// Add a new agent to core app
        /// </summary>
        /// <param name="agentId">Agent id</param>
        /// <param name="name">Agent name </param>
        /// <param name="passw">Agent passw</param>
        /// <returns>Boolean</returns>
        public async Task<Boolean> AddAgent(String agentId, String name, String passw) {
            return await AddAgent(agentId, name, passw);
        }
    }
}