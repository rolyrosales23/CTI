$(function () {
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;
    // Function to get response for CallIn
    agent.client.addCallIn = function (response) {
        // Active buttom in call
        console.log("No implemented yet", response);
    };

    agent.client.Notification = function (response) {
        noty(
            {
                text: response,
                layout: 'topRight',
                type: 'success',
                maxVisible: 5,
                animation: {
                    open: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceInLeft'
                    close: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceOutLeft'
                    easing: 'swing',
                    speed: 500 // opening & closing animation speed
                },
                timeout: 2000
            });
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
                notify("LLamada Entrante!!", 'info');
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
                break;

            case 'onTransferredCall':
                break;

            case 'onConferencedCall':
                break;

            case 'onAgentChangedState':
                console.log(response);
                break;

            case 'onRecordingStartedPlaying':
                break;

            case 'onRecordingEndedPlaying':
                break;

            case 'onCollectedDigits':
                break;
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
            $("#ReadyToWork").attr("disabled", "disabled");
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