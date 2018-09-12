using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace GestCTI.Core.WebsocketClient
{
    public class DeviceState{
        public String deviceId { get; set; }
        public String state { get; set; }

        public DeviceState(String _deviceId, String _state) {
            deviceId = _deviceId;
            state = _state;
        }
    }

    public class HoldConnection
    {
        public String ucid { get; set; }
        public List<DeviceState> devices;

        public HoldConnection(string ucid)
        {
            this.ucid = ucid;
            devices = new List<DeviceState>();
        }

        public void setDevices(List<DeviceState> lista) {
            devices = lista;
        }
    }

    public class HoldList
    {
        //listado de llamadas en espera por usuarios
        private ConcurrentDictionary<String, List<HoldConnection>> map;

        //ucid por invokedId
        private Dictionary<Guid, String> ucids = new Dictionary<Guid, String>();

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

        public void addUcid(Guid invokedId, String ucid) {
            ucids[invokedId] = ucid;
        }

        public void updateElement(String username, Guid invokedId, List<DeviceState> devices) {
            List<HoldConnection> list;
            map.TryGetValue(username, out list);
            String ucid = ucids[invokedId];
            ucids.Remove(invokedId);
            int index = list.FindIndex(hc => hc.ucid == ucid);
            if(index != -1) {
                list[index].setDevices(devices);
            }
        }
    }
}