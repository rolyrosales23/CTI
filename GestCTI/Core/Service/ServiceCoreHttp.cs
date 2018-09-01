using GestCTI.Core.Util;
using GestCTI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace GestCTI.Core.Service
{
    public class ServiceCoreHttp
    {
        private static readonly String CampaignUrl = "/Campaign";
        private static readonly String QueueCallsUrl = CampaignUrl + "/QueuedCalls";
        private static readonly String CampaignStartUrl = CampaignUrl + "Start";
        private static readonly String CampaignStoptUrl = CampaignUrl + "Stop";

        private static readonly String AgentUrl = "/Agent";
        private static readonly String AddAgentUrl = AgentUrl + "/User";
        private static readonly String GetSkillsUrl = AgentUrl + "/GetSkills";

        private static readonly String Skill = "/Skill";
        private static readonly String AddSkill = Skill + "/Add";

        private static readonly DBCTIEntities db = new DBCTIEntities();

        public static async Task<List<Call>> QueueCallsResult(String userName)
        {
            var user = db.Users.Where(item => item.Username == userName).FirstOrDefault();
            var campaigns = db.Campaign.Where(item => item.IdCompany == user.IdCompany).ToList();
            List<String> campaignIds = new List<string>();
            foreach (var item in campaigns)
            {
                campaignIds.Add(item.Id.ToString());
            }

            if (campaignIds == null || campaignIds.Count == 0)
            {
                return new List<Call>(); ;
            }

            String apiServer = user.Company1.Switch.ApiServerIP;
            String QueryParam = GenerateQueryParamCampaigns(campaignIds, "campaigns");
            try
            {
                var raw = await GetRequest(apiServer + QueueCallsUrl + "?" + QueryParam);
                return JsonConvert.DeserializeObject<List<Call>>((string)raw);
            }
            catch (Exception)
            {
                return new List<Call>();
            }
        }


        public async Task<bool> CampaignStart(int campaignId, String host, int campaignTypeId)
        {
            var campaign = db.Campaign.Where(item => item.Id == campaignId).FirstOrDefault();
            try
            {
                String QueryParam =
                    "campaignId=" + campaignId.ToString() +
                    "&campaignTypeId=" + campaignTypeId +
                    "&host=" + host;
                var raw = await GetRequest(campaign.Company.Switch.ApiServerIP + CampaignStartUrl + "?" + QueryParam);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> CampaignStop(int campaignId)
        {
            var campaign = db.Campaign.Where(item => item.Id == campaignId).FirstOrDefault();
            try
            {
                String QueryParam = "campaignId=" + campaignId.ToString();
                var raw = await GetRequest(campaign.Company.Switch.ApiServerIP + CampaignStoptUrl + "?" + QueryParam);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Get All queque calls from list of campaigns
        /// </summary>
        /// <param name="campaigns">Campaign list</param>
        /// <returns>List<Callss></returns>
        public static async Task<List<Calls>> QueueCallsResult(List<String> campaigns)
        {
            List<Calls> result = new List<Calls>();
            if (campaigns == null)
            {
                return result;
            }
            // Getting all campaign from database
            var requestedCampaigns =
                db.Campaign.Where(item => campaigns.ToList().Contains(item.Id.ToString()));
            // Split campaign by core http
            Dictionary<String, List<String>> cores = new Dictionary<string, List<string>>();
            foreach (var campaign in requestedCampaigns)
            {
                String ApiServer = campaign.Company.Switch.ApiServerIP;
                List<String> vs;
                if (cores.TryGetValue(ApiServer, out vs))
                {
                    vs.Add(campaign.Id.ToString());
                }
                else
                {
                    cores.Add(ApiServer, new List<string> { campaign.Id.ToString() });
                }
            }
            foreach (var coreHttp in cores.Keys)
            {
                String QueryParam = "";
                List<String> list;
                if (cores.TryGetValue(coreHttp, out list))
                {
                    if (list != null && list.Count > 0)
                    {
                        QueryParam = GenerateQueryParamCampaigns(list, "campaigns");
                        var raw = await GetRequest(coreHttp + QueueCallsUrl + "?" + QueryParam);
                        var response = JsonConvert.DeserializeObject<List<Calls>>((string)raw);
                        result.Concat(response);
                    }
                }
            }
            return result;
        }

        public static async Task<Boolean> AddAgent(String agentId, String name, string password)
        {
            var user = db.Users.Where(item => item.Id.ToString() == agentId).FirstOrDefault();
            var ApiServerIp = user.Company.FirstOrDefault().Switch.ApiServerIP;
            return await AddAgent(agentId, name, password, ApiServerIp);
        }

        public static async Task<Boolean> AddAgent(String agentId, String name, string password, String ApiServerIp)
        {
            var result = await GetRequest(AddAgentUrl + "/" + agentId + "/" + name + "/" + password);
            return true;
        }

        /// <summary>
        /// Wraper to do a request GET
        /// </summary>
        /// <param name="url">Object</param>
        /// <returns></returns>
        private static async Task<Object> GetRequest(String url)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }


        /// <summary>
        /// Generate a query param from list
        /// </summary>
        /// <param name="param">List param</param>
        /// <param name="key">Key query param</param>
        /// <returns>QueryParam</returns>
        private static String GenerateQueryParamCampaigns(List<String> param, String key)
        {
            String result = "";
            foreach (var item in param)
            {
                if (result.Count() == 0)
                {
                    result = result + key + "=" + item;
                }
                else
                {
                    result = result + "&" + key + "=" + item;
                }
            }
            return result;
        }
    }
}