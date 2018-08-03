using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace GestCTI.Util
{
    public class WebServicesAgent
    {
        private static HttpClient WebClient;

        public WebServicesAgent(string ip, string port)
        {
            WebClient = new HttpClient();
            WebClient.BaseAddress = new Uri("http://" + ip + ":" + port + "/Skill");
        }

        public static Task<Object> GetAgentsByState(string[] agents)
        {
            string url = "/GetAgentsByState?agents=" + agents;
            for (int i = 1; i < agents.Length; i++)
                url += "&devices=" + agents;

            return GetRequest(url);
        }

        public static Task<Object> List(string filter = null)
        {
            string url = "/List";
            if (filter != null)
                url += "?filter =" + filter;

            return GetRequest(url);
        }

        public static Task<Object> Range(int fromAgentId, int toAgentId)
        {
            string url = "/Range?fromAgentId=" + fromAgentId + "&toAgentId=" + toAgentId;

            return GetRequest(url);
        }

        public static Task<Object> Detail(string agentId)
        {
            string url = "/Detail?agentId=" + agentId;

            return GetRequest(url);
        }

        public static Task<Object> LoggedList(string filter = null)
        {
            string url = "/LoggedList";
            if(filter != null)
                url += "?filter=" + filter;

            return GetRequest(url);
        }

        public static Task<Object> Add(string agentId, string name, string password)
        {
            string url = "/Add?agentId=" + agentId + "&name=" + name + "&password=" + password;

            return GetRequest(url);
        }

        public static Task<Object> GetSkills(int[] agentId)
        {
            string url = "/GetSkills?agentId=" + agentId;
            for (int i = 1; i < agentId.Length; i++)
                url += "&agentId=" + agentId[i];

            return GetRequest(url);
        }

        public static Task<Object> SetSkills(int[] agentId, string[] skillNumbers, string[] skillLevels)
        {
            string url = "/SetSkills?agentId=" + agentId;
            for (int i = 1; i < agentId.Length; i++)
                url += "&agentId=" + agentId[i];
            for (int i = 1; i < skillNumbers.Length; i++)
                url += "&skillNumbers=" + skillNumbers[i];
            for (int i = 1; i < skillLevels.Length; i++)
                url += "&skillLevels=" + skillLevels[i];

            return GetRequest(url);
        }

        private static async Task<Object> GetRequest(string url)
        {
            var response = await WebClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}