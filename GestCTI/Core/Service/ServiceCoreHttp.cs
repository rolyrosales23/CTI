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
        // -------------------------------------------------- CAMPAIGN METHODS URLS --------------------------------------------------
        private static readonly String Campaign_Start = "/Campaign/Start";
        private static readonly String Campaign_Stop = "/Campaign/Stop";
        private static readonly String Campaign_QueuedCalls = "/Campaign/QueuedCalls";
        private static readonly String Campaign_AssignCall = "/Campaign/AssignCall";

        // -------------------------------------------------- AGENT METHODS URLS --------------------------------------------------
        private static readonly String Agent_GetAgentsByState = "/Agent/GetAgentsByState";
        private static readonly String Agent_List = "/Agent/List";
        private static readonly String Agent_Range = "/Agent/Range";
        private static readonly String Agent_Detail = "/Agent/Detail";
        private static readonly String Agent_LoggedList = "/Agent/LoggedList";
        private static readonly String Agent_Add = "/Agent/Add";
        private static readonly String Agent_GetSkills = "/Agent/GetSkills";
        private static readonly String Agent_SetSkills = "/Agent/SetSkills";
        private static readonly String Agent_Edit = "/Agent/Edit";
        private static readonly String Agent_Delete = "/Agent/Delete";

        // -------------------------------------------------- SKILLS METHODS URLS --------------------------------------------------
        private static readonly String Skill_GetAgentsByState = "/Skill/GetAgentsByState";
        private static readonly String Skill_List = "/Skill/List";
        private static readonly String Skill_Add = "/Skill/Add";
        private static readonly String Skill_Detail = "/Skill/Detail";
        private static readonly String Skill_Edit = "/Skill/Edit";
        private static readonly String Skill_Delete = "/Skill/Delete";
        


        private static readonly DBCTIEntities db = new DBCTIEntities();

        // -------------------------------------------------- CAMPAIGN METHODS --------------------------------------------------
        public static async Task<bool> CampaignStart(int campaignId, String host, int campaignTypeId)
        {
            var campaign = db.Campaign.Where(item => item.Id == campaignId).FirstOrDefault();
            try
            {
                String QueryParam =
                    "campaignId=" + campaignId.ToString() +
                    "&campaignTypeId=" + campaignTypeId +
                    "&host=" + host;
                var raw = await GetRequest(campaign.Company.Switch.ApiServerIP + Campaign_Start + "?" + QueryParam);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> CampaignStop(int campaignId)
        {
            var campaign = db.Campaign.Where(item => item.Id == campaignId).FirstOrDefault();
            try
            {
                String QueryParam = "campaignId=" + campaignId.ToString();
                var raw = await GetRequest(campaign.Company.Switch.ApiServerIP + Campaign_Stop + "?" + QueryParam);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<List<Call>> CampaignQueuedCalls(String userName)
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
                return new List<Call>();
            }

            String apiServer = user.Company1.Switch.ApiServerIP;
            String QueryParam = GenerateQueryFromList(campaignIds, "campaigns");
            try
            {
                var raw = await GetRequest(apiServer + Campaign_QueuedCalls + "?" + QueryParam);
                return JsonConvert.DeserializeObject<List<Call>>((string)raw);
            }
            catch (Exception)
            {
                return new List<Call>();
            }
        }

        /// <summary>
        /// Get All queque calls from list of campaigns
        /// </summary>
        /// <param name="campaigns">Campaign list</param>
        /// <returns>List<Callss></returns>
        public static async Task<List<Calls>> CampaignQueuedCalls(List<String> campaigns)
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
                        QueryParam = GenerateQueryFromList(list, "campaigns");
                        var raw = await GetRequest(coreHttp + Campaign_QueuedCalls + "?" + QueryParam);
                        var response = JsonConvert.DeserializeObject<List<Calls>>((string)raw);
                        result.Concat(response);
                    }
                }
            }
            return result;
        }

        // -------------------------------------------------- AGENT METHODS --------------------------------------------------
        public static async Task<Boolean> AgentAdd(String agentId, String name, string password)
        {
            var user = db.Users.Where(item => item.Id.ToString() == agentId).FirstOrDefault();
            var ApiServerIp = user.Company.FirstOrDefault().Switch.ApiServerIP;
            return await AgentAdd(agentId, name, password, ApiServerIp);
        }

        public static async Task<Boolean> AgentAdd(String agentId, String name, string password, String ApiServerIp)
        {
            var result = await GetRequest(Agent_Add + "/" + agentId + "/" + name + "/" + password);
            return true;
        }

        // -------------------------------------------------- SKILLS METHODS --------------------------------------------------
        public static async Task<Object> SkillGetAgentsByState(List<String> devices) {
            String query = GenerateQueryFromList(devices, "devices");
            return await GetRequest(Agent_GetAgentsByState + "?" + query);
        }

        public static async Task<string> SkillList(String filter = ""){
            return await GetRequest(Skill_List + "?filter=" + filter);
        }

       /* public static async Task<Boolean> SkillAdd(String skillNumber, String name, String extension){
            Object status = await GetRequest(Skill_Add + "?=skillNumber" + skillNumber + "&name=" + name + "&extension=" + extension);
            
        }

        public static async Task<Object> SkillDetail(string skillNumber) {
            return await GetRequest(Skill_Detail + "?=skillNumber" + skillNumber);
        }

        public static async Task<Boolean> SkillEdit(string skillNumber, string name, string extension) {
            return await GetRequest(Skill_Edit + "?=skillNumber" + skillNumber + "&name=" + name + "&extension=" + extension);
        }

        public static async Task<Boolean> SkillDelete(string skillNumber) {
            return await GetRequest(Skill_Delete + "?=skillNumber" + skillNumber);
        }*/

        // -------------------------------------------------- GENERAL PURPOSE METHODS --------------------------------------------------
        /// <summary>
        /// Wraper to do a request GET
        /// </summary>
        /// <param name="url">Object</param>
        /// <returns></returns>
        private static async Task<string> GetRequest(String url)
        {
            String username = System.Web.HttpContext.Current.User.Identity.Name;
            Users user = db.Users.Where(u => u.Username == username).FirstOrDefault();
            if (user != null) {
                String ApiServerIp = user.Company.FirstOrDefault().Switch.ApiServerIP;
                url = ApiServerIp + url;

                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(url);
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }

            return null;
        }

        /// <summary>
        /// Generate a query param from list
        /// </summary>
        /// <param name="param">List param</param>
        /// <param name="key">Key query param</param>
        /// <returns>QueryParam</returns>
        private static String GenerateQueryFromList(List<String> param, String key)
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