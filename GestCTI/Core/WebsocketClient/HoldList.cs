using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace GestCTI.Core.WebsocketClient
{
    public class HoldConnection
    {
        public String ucid { get; set; }
        public String toDevice { get; set; }
        public HoldConnection(string ucid, string deviceId)
        {
            this.ucid = ucid;
            this.toDevice = deviceId;
        }
    }

    public class HoldList
    {
        private ConcurrentDictionary<String, List<HoldConnection>> map;

        public HoldList() {
            map = new ConcurrentDictionary<string, List<HoldConnection>>();
        }

        public List<HoldConnection> getList(String username) {
            List<HoldConnection> list;
            map.TryGetValue(username, out list);
            return list;
        }

        public void addElement(String username, HoldConnection element) {
            List<HoldConnection> list;
            map.TryGetValue(username, out list);

            if(list == null)
                list = new List<HoldConnection>();
            list.Add(element);

            map.AddOrUpdate(username, list, (key, oldValue) => list);
        }

        public void removeElement(String username, String ucid){
            List<HoldConnection> list;
            map.TryGetValue(username, out list);

            list.RemoveAll(element => element.ucid == ucid);

            map.AddOrUpdate(username, list, (key, oldValue) => list);
        }
    }
}