using GestCTI.Core.Util;
using GestCTI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public static async Task<Boolean> CampaignStart(int campaignId, String host, int campaignTypeId)
        {
            try
            {
                var response = await GetRequest(Campaign_Start + "?campaignId=" + campaignId + "&host=" + host + "&campaignTypeId=" + campaignTypeId);
                JObject result = JObject.Parse(response.ToString());
                return (result["Success"].ToString().ToUpper() == "TRUE");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<Boolean> CampaignStop(int campaignId)
        {
            try
            {
                var response = await GetRequest(Campaign_Stop + "?campaignId=" + campaignId);
                JObject result = JObject.Parse(response.ToString());
                return (result["Success"].ToString().ToUpper() == "TRUE");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<List<Call>> CampaignQueuedCalls(String userName)
        {
            List<String> campaigns = db.GetCampaigns(userName).Select(c => c.Id.ToString()).ToList<String>();
            if (campaigns == null || campaigns.Count == 0)
            {
                return new List<Call>();
            }

            String query = GenerateQueryFromList(campaigns, "campaigns");
            try
            {
                var raw = await GetRequest(Campaign_QueuedCalls + "?" + query);
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
                        var raw = await GetRequest(Campaign_QueuedCalls + "?" + QueryParam, coreHttp);
                        var response = JsonConvert.DeserializeObject<List<Calls>>((string)raw);
                        result.Concat(response);
                    }
                }
            }
            return result;
        }

        public static async Task<Boolean> CampaignAssignCall(String campaignHost, String campaignId, String ucid){
            Campaign campaign = db.Campaign.Find(campaignId);
            if (campaign != null) {
                String httpHost = campaign.Company.Switch.ApiServerIP;

                var response = await GetRequest(Campaign_AssignCall + "?campaignHost=" + campaignHost + "&campaignId=" + campaignId + "&ucid=" + ucid, httpHost);
                JObject result = JObject.Parse(response.ToString());
                return (result["Success"].ToString().ToUpper() == "TRUE");
            }

            return false;
        }

        // -------------------------------------------------- AGENT METHODS --------------------------------------------------
        public static async Task<string> AgentGetAgentsByState(List<String> agents){
            String query = GenerateQueryFromList(agents, "agents");
            return await GetRequest(Agent_GetAgentsByState + "?" + query);
        }

        public static async Task<string> AgentList(String filter = ""){
            return await GetRequest(Agent_List + "?filter=" + filter);
        }

        public static async Task<string> AgentRange(int fromAgentId, int toAgentId) {
            return await GetRequest(Agent_Range + "?fromAgentId=" + fromAgentId + "&toAgentId=" + toAgentId);
        }

        public static async Task<string> AgentDetail(String agentId){
            return await GetRequest(Agent_Detail + "?agentId=" + agentId);
        }

        public static async Task<string> AgentLoggedList(string filter = ""){
            return await GetRequest(Agent_LoggedList + "?filter=" + filter);
        }

        public static async Task<Boolean> AgentAdd(String agentId, String name, string password){
            var response = await GetRequest(Agent_Add + "?agentId=" + agentId + "&name=" + name + "&password=" + password);
            JObject result = JObject.Parse(response.ToString());
            return (result["Success"].ToString().ToUpper() == "TRUE");
        }

        public static async Task<string> AgentGetSkills(List<String> agentId){
            String query = GenerateQueryFromList(agentId, "agentId");
            return await GetRequest(Agent_GetSkills + "?" + query);
        }

        public static async Task<Boolean> SetSkills(List<String> agentId, List<String> skillNumbers, List<String> skillLevels){
            String qAgentId = GenerateQueryFromList(agentId, "agentId");
            String qSkillNumbers = GenerateQueryFromList(skillNumbers, "skillNumbers");
            String qSkillLevels = GenerateQueryFromList(skillLevels, "skillLevels");
            var response = await GetRequest(Agent_SetSkills + "?" + qAgentId + "&" + qSkillNumbers + "&" + qSkillLevels);
            JObject result = JObject.Parse(response.ToString());
            return (result["Success"].ToString().ToUpper() == "TRUE");
        }

        public static async Task<Boolean> AgentEdit(String agentId, String name, String password) {
            var response = await GetRequest(Agent_Edit + "?agentId=" + agentId + "&name=" + name + "&password=" + password);
            JObject result = JObject.Parse(response.ToString());
            return (result["Success"].ToString().ToUpper() == "TRUE");
        }

        public static async Task<Boolean> AgentDelete(String agentId){
            var response = await GetRequest(Agent_Delete + "?agentId=" + agentId);
            JObject result = JObject.Parse(response.ToString());
            return (result["Success"].ToString().ToUpper() == "TRUE");
        }

        // -------------------------------------------------- SKILLS METHODS --------------------------------------------------
        public static async Task<string> SkillGetAgentsByState(List<String> devices) {
            String query = GenerateQueryFromList(devices, "devices");
            return await GetRequest(Skill_GetAgentsByState + "?" + query);
        }

        public static async Task<string> SkillList(String filter = ""){
            return await GetRequest(Skill_List + "?filter=" + filter);
        }

        public static async Task<Boolean> SkillAdd(String skillNumber, String name, String extension){
            var response = await GetRequest(Skill_Add + "?=skillNumber" + skillNumber + "&name=" + name + "&extension=" + extension);
            JObject result = JObject.Parse( response.ToString() );
            return (result["Success"].ToString().ToUpper() == "TRUE");
        }

        public static async Task<string> SkillDetail(string skillNumber) {
            return await GetRequest(Skill_Detail + "?=skillNumber" + skillNumber);
        }

        public static async Task<Boolean> SkillEdit(string skillNumber, string name, string extension) {
            var response = await GetRequest(Skill_Edit + "?=skillNumber" + skillNumber + "&name=" + name + "&extension=" + extension);
            JObject result = JObject.Parse(response.ToString());
            return (result["Success"].ToString().ToUpper() == "TRUE");
        }

        public static async Task<Boolean> SkillDelete(string skillNumber) {
            var response = await GetRequest(Skill_Delete + "?=skillNumber" + skillNumber);
            JObject result = JObject.Parse(response.ToString());
            return (result["Success"].ToString().ToUpper() == "TRUE");
        }

        // -------------------------------------------------- GENERAL PURPOSE METHODS --------------------------------------------------
        /// <summary>
        /// Wraper to do a request GET
        /// </summary>
        /// <param name="url">Object</param>
        /// <returns></returns>
        private static async Task<string> GetRequest(String url, String coreHttp = "")
        {
            if (coreHttp != "")
                url = coreHttp + url;
            else {
                String username = System.Web.HttpContext.Current.User.Identity.Name;
                Users user = db.Users.Where(u => u.Username == username).FirstOrDefault();
                if (user != null && user.Company1 != null)
                {
                    String ApiServerIp = user.Company1.Switch.ApiServerIP;
                    url = ApiServerIp + url;
                }
                else return null;
            }

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
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