using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestCTI.Util
{
    public class CallHandling : HandlingBase
    {
        public static String CTIGetDevices(String ucid){
            String[] Params = { ucid };

            return builder.AppendFormat(format, "CTIGetDevices", joinParams(Params)).ToString();
        }

        public static String CTIMakeCallRequest(String fromDevice, String toDevice, String callerId){
            String[] Params = { fromDevice, toDevice, callerId };

            return builder.AppendFormat(format, "CTIMakeCallRequest", joinParams(Params)).ToString();
        }

        public static String CTIAnswerCallRequest(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };

            return builder.AppendFormat(format, "CTIAnswerCallRequest", joinParams(Params)).ToString();
        }

        public static String CTIDeflectCallRequest(String ucid, String fromDeviceId, String toDeviceId){
            String[] Params = { ucid, fromDeviceId, toDeviceId };

            return builder.AppendFormat(format, "CTIDeflectCallRequest", joinParams(Params)).ToString();
        }

        public static String CTIRetrieveConnectionRequest(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };

            return builder.AppendFormat(format, "CTIRetrieveConnectionRequest", joinParams(Params)).ToString();
        }

        public static String CTIHoldConnectionRequest(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };

            return builder.AppendFormat(format, "CTIHoldConnectionRequest", joinParams(Params)).ToString();
        }

        public static String CTIClearConnectionRequest(String ucid, String deviceId) {
            String[] Params = { ucid, deviceId };

            return builder.AppendFormat(format, "CTIClearConnectionRequest", joinParams(Params)).ToString();
        }

        public static String CTIClearCallRequest(String ucid) {
            String[] Params = { ucid };

            return builder.AppendFormat(format, "CTIClearCallRequest", joinParams(Params)).ToString();
        }

        public static String CTISingleStepConferenceRequest(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };

            return builder.AppendFormat(format, "CTISingleStepConferenceRequest", joinParams(Params)).ToString();
        }

        public static String CTISingleStepConferenceRequestV2(String ucid, String deviceId){
            String[] Params = { ucid, deviceId };

            return builder.AppendFormat(format, "CTISingleStepConferenceRequestV2", joinParams(Params)).ToString();
        }

        public static String CTIConferenceRequest(String heldUcid, String activeUcid, String deviceId){
            String[] Params = { heldUcid, activeUcid, deviceId };

            return builder.AppendFormat(format, "CTIConferenceRequest", joinParams(Params)).ToString();
        }

        public static String CTISingleStepTransferRequest(String ucid, String transferringDeviceId, String deviceId){
            String[] Params = { ucid, transferringDeviceId, deviceId };

            return builder.AppendFormat(format, "CTISingleStepTransferRequest", joinParams(Params)).ToString();
        }

        public static String CTITransferRequest(String heldUcid, String activeUcid, String deviceId){
            String[] Params = { heldUcid, activeUcid, deviceId };

            return builder.AppendFormat(format, "CTITransferRequest", joinParams(Params)).ToString();
        }

        public static String CTIWhisperRequest(String deviceId, String ucid, String selectedParty){
            String[] Params = { deviceId, ucid, selectedParty };

            return builder.AppendFormat(format, "CTIWhisperRequest", joinParams(Params)).ToString();
        }

        public static String CTIListenHoldAllRequest(String deviceId, String ucid){
            String[] Params = { deviceId, ucid };

            return builder.AppendFormat(format, "CTIListenHoldAllRequest", joinParams(Params)).ToString();
        }

        public static String CTIListenRetrieveAllRequest(String deviceId, String ucid){
            String[] Params = { deviceId, ucid };

            return builder.AppendFormat(format, "CTIListenRetrieveAllRequest", joinParams(Params)).ToString();
        }

        public static String CTIMuteRequest(String deviceId, String ucid){
            String[] Params = { deviceId, ucid };

            return builder.AppendFormat(format, "CTIMuteRequest", joinParams(Params)).ToString();
        }
    }
}