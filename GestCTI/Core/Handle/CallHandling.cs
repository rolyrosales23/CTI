using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Util
{
    public class CallHandling : HandlingBase
    {
        public static Tuple<Guid, String> CTIGetDevices(String ucid){
            String[] Params = { ucid };
            return makeRequest("CTIGetDevices", Params);
        }

        public static Tuple<Guid, String> CTIMakeCallRequest(String fromDevice, String toDevice, String callerId){
            String[] Params = { fromDevice, toDevice, callerId };
            return makeRequest("CTIMakeCallRequest", Params);
        }

        public static Tuple<Guid, String> CTIAnswerCallRequest(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };
            return makeRequest("CTIAnswerCallRequest", Params);
        }

        public static Tuple<Guid, String> CTIDeflectCallRequest(String ucid, String fromDeviceId, String toDeviceId){
            String[] Params = { ucid, fromDeviceId, toDeviceId };
            return makeRequest("CTIDeflectCallRequest", Params);
        }

        public static Tuple<Guid, String> CTIRetrieveConnectionRequest(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };
            return makeRequest("CTIRetrieveConnectionRequest", Params);
        }

        public static Tuple<Guid, String> CTIHoldConnectionRequest(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };
            return makeRequest("CTIHoldConnectionRequest", Params);
        }

        public static Tuple<Guid, String> CTIClearConnectionRequest(String ucid, String deviceId) {
            String[] Params = { ucid, deviceId };
            return makeRequest("CTIClearConnectionRequest", Params);
        }

        public static Tuple<Guid, String> CTIClearCallRequest(String ucid) {
            String[] Params = { ucid };
            return makeRequest("CTIClearCallRequest", Params);
        }

        public static Tuple<Guid, String> CTISingleStepConferenceRequest(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };
            return makeRequest("CTISingleStepConferenceRequest", Params);
        }

        public static Tuple<Guid, String> CTISingleStepConferenceRequestV2(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };
            return makeRequest("CTISingleStepConferenceRequestV2", Params);
        }

        public static Tuple<Guid, String> CTIConferenceRequest(String heldUcid, String activeUcid, String deviceId){
            String[] Params = { heldUcid, activeUcid, deviceId };
            return makeRequest("CTIConferenceRequest", Params);
        }

        public static Tuple<Guid, String> CTISingleStepTransferRequest(String ucid, String transferringDeviceId, String deviceId){
            String[] Params = { ucid, transferringDeviceId, deviceId };
            return makeRequest("CTISingleStepTransferRequest", Params);
        }

        public static Tuple<Guid, String> CTITransferRequest(String heldUcid, String activeUcid, String deviceId){
            String[] Params = { heldUcid, activeUcid, deviceId };
            return makeRequest("CTITransferRequest", Params);
        }

        public static Tuple<Guid, String> CTIWhisperRequest(String deviceId, String ucid, String selectedParty){
            String[] Params = { deviceId, ucid, selectedParty };
            return makeRequest("CTIWhisperRequest", Params);
        }

        public static Tuple<Guid, String> CTIListenHoldAllRequest(String deviceId, String ucid){
            String[] Params = { deviceId, ucid };
            return makeRequest("CTIListenHoldAllRequest", Params);
        }

        public static Tuple<Guid, String> CTIListenRetrieveAllRequest(String deviceId, String ucid){
            String[] Params = { deviceId, ucid };
            return makeRequest("CTIListenRetrieveAllRequest", Params);
        }

        public static Tuple<Guid, String> CTIMuteRequest(String deviceId, String ucid){
            String[] Params = { deviceId, ucid };
            return makeRequest("CTIMuteRequest", Params);
        }
    }
}