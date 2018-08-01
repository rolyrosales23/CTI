using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GestCTI.Util
{
    public class WebServicesCapaign
    {
        private static HttpClient WebClient;

        public WebServicesCapaign(string ip, string port)
        {
            WebClient = new HttpClient();
            WebClient.BaseAddress = new Uri("http://" + ip + ":" + port + "/Campaign");
        }

        public static Task<Object> Start(int campaignId, string host, int campaignTypeId)
        {
            string url = "/Start?campaignId=" + campaignId + "&campaignTypeId=" + campaignTypeId + "&host=" + host;

            return GetRequest(url);
        }

        public static Task<Object> Stop(int campaignId)
        {
            string url = "/Stop?campaignId =" + campaignId;

            return GetRequest(url);
        }

        public static Task<Object> QueuedCalls(int[] campaignsId)
        {
            string url = "/QueuedCalls?campaigns=" + campaignsId[0];

            for (int i = 1; i < campaignsId.Length; i++)
            {
                url += "&campaigns=" + campaignsId[i];
            }

            return GetRequest(url);
        }

        public static Task<Object> AssignCall(string campaignHost, string campaignId, string ucid)
        {
            string url = "/AssignCall?campaignHost=" + campaignHost + "&campaignId=" + campaignId + "&ucid=" + ucid;
            
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