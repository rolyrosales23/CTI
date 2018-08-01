using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace GestCTI.Util
{
    public class WebServicesSkill
    {
        private static HttpClient WebClient;

        public WebServicesSkill(string ip, string port)
        {
            WebClient = new HttpClient();
            WebClient.BaseAddress = new Uri("http://" + ip + ":" + port + "/Skill");
        }

        public static Task<Object> GetAgentsByState(string[] devices)
        {
            string url = "/GetAgentsByState?devices=" + devices;
            for (int i = 1; i < devices.Length; i++)
                url += "&devices=" + devices;

            return GetRequest(url);
        }

        public static Task<Object> List(string filter = null)
        {
            string url = "/List";
            if(filter != null)
                url += "?filter =" + filter;

            return GetRequest(url);
        }

        public static Task<Object> Add(string skillNumber, string name, string extension)
        {
            string url = "/Add?skillNumber=" + skillNumber + "?name=" + name + "?extension=" + extension;
            
            return GetRequest(url);
        }

        public static Task<Object> Detail(string skillNumber)
        {
            string url = "/Detail?skillNumber=" + skillNumber;

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