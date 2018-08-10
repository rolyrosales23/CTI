var AgentState = {
    AS_NOT_READY:  0,
    AS_LOGGED_OUT: 1,
    AS_READY:      2,
    AS_AFTER_CALL: 3,
    AS_WORK_READY: 4
};


$(function () {
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;
    // Function to get response for CallIn
    agent.client.addCallIn = function (response) {
        // Active buttom in call
        console.log("No implemented yet", response);
    };

    agent.client.Notification = function (response) {
        successNoty(response);
    }

    // Logout from web app
    agent.client.logOutCore = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            $('#LogOutForm').submit();
        } else {
            // Notificar error
        }
    }

    agent.client.getAmReady = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            console.log("SET STATE AM READY SUCESS");
        } else {
            console.error("FAIL SET STATE AM READY ")
        }
    }

    agent.client.receiveAcceptCallRequest = function (response) {
        // Do nothing or check is fail call request 
    }

    agent.client.onEventHandler = function (response) {
        json = JSON.parse(response);
        var eventName = json.request.request;
        var eventArgs = json.request.args;

        switch (eventName) {
            case 'onServiceInitiated':
                break;

            case 'onCallOriginated':
                break;

            case 'onCallDelivered':
                localStorage.setItem('ucid', eventArgs[0]);
                $('#acceptCallRequest').removeAttr('disabled');
                infoNoty("LLamada Entrante!!");
                break;

            case 'onCallDiverted':
                break;

            case 'onCallFailed':
                break;

            case 'onEstablishedConnection':
                $("#hangoutCallRequest").removeAttr("disabled");
                $("#acceptCallRequest").attr("disabled", "disabled");
                break;

            case 'onHoldConnection':
                break;

            case 'onHoldPartyConnection':
                break;

            case 'onRetrieveConnection':
                break;

            case 'onRetrievePartyConnection':
                break;

            case 'onEndConnection':
                break;

            case 'onEndPartyConnection':
                break;

            case 'onEndCall':
                $("#hangoutCallRequest").attr("disabled", "disabled");
                $("#ReadyToWork").removeAttr("disabled");
                $("#inputPhone").removeAttr("disabled");
                $("#doCallBtn").removeAttr("disabled");
                break;

            case 'onTransferredCall':
                break;

            case 'onConferencedCall':
                break;

            case 'onAgentChangedState': {
                var agentState = eventArgs[1];
                switch (agentState) {
                    case AgentState.AS_READY:
                        $("#ReadyToWork").attr("disabled", "disabled");
                        break;
                }
                break;
            }

            case 'onRecordingStartedPlaying':
                break;

            case 'onRecordingEndedPlaying':
                break;

            case 'onCollectedDigits':
                break;
        }
    }

    agent.client.addCTIMakeCallRequest = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            $('#inputPhone').attr('disabled', 'disabled');
            $('#doCallBtn').attr('disabled', 'disabled');
            console.log("MAKE CALL SUCCESS");
        } else {
            console.error("FAIL SET STATE AM READY ")
        }
    }


    // Start the connection.
    $.connection.hub.start().done(function () {
        var deviceId = localStorage.getItem('deviceId');
        /*if (deviceId !== undefined && deviceId !== "") {
            // Put the Agent to login aux_work
            agent.server.sendStateLoginAuxWork(deviceId);
        }*/

        $('#ReadyToWork').click(function () {
            // Put de agent to AM_READY and MANUAL_IN
            agent.server.sendStateReadyManual(deviceId);
        });

        $('#LogOutCore').click(function () {
            var deviceId = localStorage.getItem('deviceId');
            agent.server.sendLogOutCore(deviceId);
        });

        // Send initialize device
        $('#sendInitialize').click(function () {
            console.log("Sending initialize");
            agent.server.sendInitialize("8006"  /*string deviceId*/);
        });

        $("#acceptCallRequest").click(function () {
            var ucid = localStorage.getItem('ucid');
            if (ucid !== undefined && ucid !== "") {                
                agent.server.sendCTIAnswerCallRequest(ucid, deviceId);
                //localStorage.removeItem('ucid');
            } else {
                console.error("Call id request (ucid) not specify");
            }
        });

        $("#hangoutCallRequest").click(function () {
            var ucid = localStorage.getItem('ucid');
            if (ucid !== undefined && ucid !== "") {
                agent.server.sendCTIClearConnectionRequest(ucid, deviceId);
            } else {
                console.error("Call id request (ucid) not specify");
            }
        });

        $("#doCallBtn").click(function () {
            var toDevice = $('#inputPhone').val();
            if (deviceId !== undefined && deviceId !== "" && toDevice !== undefined && toDevice !== "") {
                agent.server.sendCTIMakeCallRequest(deviceId, toDevice, "*99");
            }
            else {
                console.error("Call id request (fromDevice or toDevice) not specify");
            }

        });

        $('#sendCTIAnswerCallRequest').click(function () {
            // agent.server.sendCTIAnswerCallRequest(string ucid, string fromDeviceId, string toDeviceId);
        });

        $('#sendCTIRetrieveConnectionRequest').click(function () {
            // agent.server.sendCTIRetrieveConnectionRequest(string ucid, string deviceId)
        });

        $('#sendCTIHoldConnectionRequest').click(function () {
            // agent.server.sendCTIHoldConnectionRequest(string ucid, string deviceId)
        });

        $('#sendCTIRetrieveConnectionRequest').click(function () {
            // agent.server.sendCTIRetrieveConnectionRequest(string ucid, string deviceId)
        });

        $('#sendCTIClearConnectionRequest').click(function () {
            // agent.server.sendCTIClearConnectionRequest(string ucid, string deviceId)
        });

        $('#sendCTIClearCallRequest').click(function () {
            // agent.server.sendCTIClearCallRequest(string ucid)
        });

        $('#sendCTISingleStepConferenceRequest').click(function () {
            // agent.server.sendCTISingleStepConferenceRequest(string ucid, string deviceId)
        });

        $('#sendCTISingleStepConferenceRequestV2').click(function () {
            // agent.server.sendCTISingleStepConferenceRequestV2(string ucid, string deviceId)
        });

        $('#sendCTIConferenceRequest').click(function () {
            // agent.server.sendCTIConferenceRequest(string heldUcid, string activeUcid, string deviceId)
        });

        $('#sendCTISingleStepTransferRequest').click(function () {
            // agent.server.sendCTISingleStepTransferRequest(string ucid, string transferringDeviceId, string deviceId)
        });

        $('#sendCTITransferRequest').click(function () {
            // agent.server.sendCTITransferRequest(string heldUcid, string activeUcid, string deviceId)
        });

        $('#sendCTIWhisperRequest').click(function () {
            // agent.server.sendCTIWhisperRequest(string deviceId, string ucid, string selectedParty)
        });

        $('#sendCTIListenHoldAllRequest').click(function () {
            // agent.server.sendCTIListenHoldAllRequest(string deviceId, string ucid)
        });

        $('#sendCTIListenRetrieveAllRequest').click(function () {
            // agent.server.sendCTIListenRetrieveAllRequest(string deviceId, string ucid)
        });
    });
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}