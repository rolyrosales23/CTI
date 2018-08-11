﻿var AgentState = {
    AS_NOT_READY: 0,
    AS_LOGGED_OUT: 1,
    AS_READY: 2,
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
    };

    // Logout from web app
    agent.client.logOutCore = function (response) {
        if (response === null) {
            $('#LogOutForm').submit();
        } else {
            json = JSON.parse(response);
            if (json['success'] === true) {
                // agent.server.stop();
                $('#LogOutForm').submit();
            } else {
                // Notificar error
            }
        }
    };

    agent.client.resultHoldConnections = function (response) {
        //painting in this position
    }

    agent.client.getAmReady = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            console.log("SET STATE AM READY SUCESS");
        } else {
            console.error("FAIL SET STATE AM READY");
        }
    };

    agent.client.receiveAcceptCallRequest = function (response) {
        json = JSON.parse(response);
        if (json.success === false) {
            $("#acceptCallRequest").attr("disabled", "disabled");
            localStorage.removeItem('ucid');
        }
        // Do nothing or check is fail call request 
    };

    agent.client.onEventHandler = function (response, data) {
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
                $('#doHoldConnection').removeAttr('disabled');
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
                // Repinta la lista de hold
                console.log(data);
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
                $("#inputPhone").val('');
                $('#doHoldConnection').attr('disabled', 'disabled');
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
    };

    agent.client.addCTIMakeCallRequest = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            $('#inputPhone').attr('disabled', 'disabled');
            $('#doCallBtn').attr('disabled', 'disabled');
            localStorage.setItem('ucid', json.result.ucid);
            console.log("MAKE CALL SUCCESS");
        } else {
            console.error("FAIL SET STATE AM READY ");
        }
    };


    // Start the connection.
    $.connection.hub.start().done(function () {
        var deviceId = localStorage.getItem('deviceId');

        $('#ReadyToWork').click(function () {
            // Put de agent to AM_READY and MANUAL_IN
            agent.server.sendStateReadyManual(deviceId);
        });

        $('#LogOutCore').click(function () {
            var deviceId = localStorage.getItem('deviceId');
            agent.server.sendLogOutCore(deviceId);
        });

        $("#acceptCallRequest").click(function () {
            var ucid = localStorage.getItem('ucid');
            if (ucid !== undefined && ucid !== "") {
                agent.server.sendCTIAnswerCallRequest(ucid, deviceId);
            } else {
                console.error("Call id request (ucid) not specify");
            }
        });

        $("#hangoutCallRequest").click(function () {
            var ucid = localStorage.getItem('ucid');
            if (ucid !== undefined && ucid !== "") {
                $("#hangoutCallRequest").attr("disabled", "disabled");
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

        $("#doHoldConnection").click(function () {
            var ucid = localStorage.getItem('ucid');
            if (ucid !== undefined && ucid !== "" && deviceId !== undefined && deviceId !== "") {
                $("#doHoldConnection").attr("disabled", "disabled");
                agent.server.sendCTIHoldConnectionRequest(ucid, deviceId);
            }
        });
    });
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}