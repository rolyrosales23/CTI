using GestCTI.Core.WebsocketClient.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Client;

namespace GestCTI.Core.WebsocketClient
{
    public class HubCoreClient : BaseHubClient, IRecieveHubSync, ISendHubSync
    {

        public HubCoreClient(String url, String hubName)
        {
            HubConnectionUrl = url;
            HubProxyName = hubName;
            Init();
        }
        public override void StartHub()
        {
            _hubConnection.Dispose();
            Init();
        }

        public new void Init() {
            base.Init();
            _myHubProxy.On("heartbeat", Recieve_Heartbeat);
            StartHubInternal();
        }

        public void Recieve_AddMessage(string name, string message)
        {
            throw new NotImplementedException();
        }

        public void Recieve_Heartbeat()
        {
            throw new NotImplementedException();
        }

        public void Recieve_SendObject(string obj)
        {
            throw new NotImplementedException();
        }

        public void AddMessage(string name, string message)
        {
            _myHubProxy.Invoke("addMessage", "client message", " sent from console client").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // HubClientEvents.Log.Error("There was an error opening the connection:" + task.Exception.GetBaseException());
                }

            }).Wait();
            // HubClientEvents.Log.Informational("Client Sending addMessage to server");
        }

        public void Heartbeat()
        {
            _myHubProxy.Invoke("Heartbeat").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // HubClientEvents.Log.Error("There was an error opening the connection:" + task.Exception.GetBaseException());
                }

            }).Wait();
            // HubClientEvents.Log.Informational("Client heartbeat sent to server");
        }
    }
}