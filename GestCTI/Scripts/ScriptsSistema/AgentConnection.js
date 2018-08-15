var AgentState = {
    AS_NOT_READY: 0,
    AS_LOGGED_OUT: 1,
    AS_READY: 2,
    AS_AFTER_CALL: 3,
    AS_WORK_READY: 4
};

function notEmpty(value) {
    return value != undefined && value !== null && value != "";
}

function pintarListaEspera(lista) {
    var listaContainer = $('#lista_espera');
    listaContainer.find('li').remove();
    for (var i in lista) {
        listaContainer.append("<li><input type='radio' name='hold_list' value='" + lista[i].ucid + "'> deviceId:" + lista[i].toDevice + "</li>");
    }
} 

$(function () {
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;

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
                errorNoty(json.response);
            }
        }
    };

    agent.client.resultHoldConnections = function (response) {
        spinnerHide();
        if (response.length) {
            //llenar select
            var select = $('#transfer-modal select');
            select.find('option').remove();
            for (var i in response) {
                select.append(new Option(response[i].toDevice, response[i].ucid));
            }
            select.selectpicker('refresh');

            $('#transfer-modal').modal();
        }
    };

    agent.client.getAmReady = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            successNoty(Resources.ChangeToReadyOk);
        } else {
            errorNoty(Resources.ChangeToReadyFail);
            $("#ReadyToWork").removeAttr("disabled");
        }
    };

    agent.client.receiveAcceptCallRequest = function (response) {
        json = JSON.parse(response);
        if (json.success === false) {
            $("#acceptCallRequest").attr("disabled", "disabled");
            localStorage.removeItem('ucid');
            successNoty(Resources.InCall);
        }
        // Do nothing or check is fail call request 
    };

    agent.client.onEventHandler = function (response, data) {
        json = JSON.parse(response);
        var eventName = json.request.request;
        var eventArgs = json.request.args;

        switch (eventName) {
            case 'onServiceInitiated':
                $("#hangoutCallRequest").removeAttr("disabled");

                tempNoty('onServiceInitiated');
                break;

            case 'onCallOriginated':

                tempNoty('onCallOriginated');
                break;

            case 'onCallDelivered':
                localStorage.setItem('ucid', eventArgs[0]);
                $('#acceptCallRequest').removeAttr('disabled');
                $('#doHoldConnection').removeAttr('disabled');
                infoNoty(Resources.IncomingCall);

                tempNoty('onCallDelivered');
                break;

            case 'onCallExternalDelivered':
                localStorage.setItem('ucid', eventArgs[0]);
                $('#acceptCallRequest').removeAttr('disabled');
                $('#doHoldConnection').removeAttr('disabled');
                infoNoty(Resources.InExternalCall);

                tempNoty('onCallExternalDelivered');
                break;

            case 'onCallDiverted':

                tempNoty('onCallDiverted');
                break;

            case 'onCallFailed':

                tempNoty('onCallFailed');
                break;

            case 'onEstablishedConnection':
                localStorage.setItem('calledDeviceId', eventArgs[5]);
                $("#hangoutCallRequest").removeAttr("disabled");
                $("#acceptCallRequest").attr("disabled", "disabled");
                $("#doHoldConnection").removeAttr("disabled");

                tempNoty('onEstablishedConnection');
                break;

            case 'onHoldConnection':
                pintarListaEspera(data);

                tempNoty('onHoldConnection');
                break;

            case 'onHoldPartyConnection':

                tempNoty('onHoldPartyConnection');
                break;

            case 'onRetrieveConnection':

                tempNoty('onRetrieveConnection');
                break;

            case 'onRetrievePartyConnection':

                tempNoty('onRetrievePartyConnection');
                break;

            case 'onEndConnection':

                tempNoty('onEndConnection');
                break;

            case 'onEndPartyConnection':

                tempNoty('onEndPartyConnection');
                break;

            case 'onEndCall':
                $("#hangoutCallRequest").attr("disabled", "disabled");
                $("#ReadyToWork").removeAttr("disabled");
                $("#inputPhone").removeAttr("disabled");
                $("#inputPhone").val('');
                $('#doHoldConnection').attr('disabled', 'disabled');
                pintarListaEspera(data);

                tempNoty('onEndCall');
                break;

            case 'onTransferredCall':

                tempNoty('');
                break;

            case 'onConferencedCall':

                tempNoty('onTransferredCall');
                break;

            case 'onAgentChangedState': {
                var agentState = eventArgs[1];
                switch (agentState) {
                    case AgentState.AS_READY:
                        $("#ReadyToWork").attr("disabled", "disabled");
                        break;
                }

                tempNoty('onAgentChangedState');
                break;
            }

            case 'onRecordingStartedPlaying':

                tempNoty('onRecordingStartedPlaying');
                break;

            case 'onRecordingEndedPlaying':

                tempNoty('onRecordingEndedPlaying');
                break;

            case 'onCollectedDigits':

                tempNoty('onCollectedDigits');
                break;
        }
    };

    agent.client.addCTIMakeCallRequest = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            $('#inputPhone').attr('disabled', 'disabled');
            $('#doCallBtn').attr('disabled', 'disabled');
            localStorage.setItem('ucid', json.result.ucid);
            successNoty(Resources.Calling);
        } else {
            errorNoty(Resources.MakeCallFail);
        }
    };


    // Start the connection.
    $.connection.hub.start().done(function () {
        var deviceId = localStorage.getItem('deviceId');

        $('#ReadyToWork').click(function () {
            // Put de agent to AM_READY and MANUAL_IN
            $("#ReadyToWork").attr("disabled", "disabled");
            agent.server.sendStateReadyManual(deviceId);
        });

        $('#LogOutCore').click(function () {
            agent.server.sendLogOutCore(deviceId);
        });

        $("#acceptCallRequest").click(function () {
            var ucid = localStorage.getItem('ucid');
            if (ucid !== undefined && ucid !== "") {
                agent.server.sendCTIAnswerCallRequest(ucid, deviceId);
            } else {
                console.error(Resources.NotUcid);
            }
        });

        $("#hangoutCallRequest").click(function () {
            var ucid = localStorage.getItem('ucid');
            if (ucid !== undefined && ucid !== "") {
                $("#hangoutCallRequest").attr("disabled", "disabled");
                agent.server.sendCTIClearConnectionRequest(ucid, deviceId);
            } else {
                errorNoty(Resources.NotUcid);
            }
        });

        $("#doCallBtn").click(function () {
            var toDevice = $('#inputPhone').val();
            if (notEmpty(deviceId) && notEmpty(toDevice)) {
                agent.server.sendCTIMakeCallRequest(deviceId, toDevice, "*99");
            }
            else {
                errorNoty(Resources.NotDevice);
            }
        });

        $("#doHoldConnection").click(function () {
            var ucid = localStorage.getItem('ucid');
            var deviceId = localStorage.getItem('calledDeviceId');
            if (ucid !== undefined && ucid !== "" && deviceId !== undefined && deviceId !== "") {
                $("#doHoldConnection").attr("disabled", "disabled");
                agent.server.sendCTIHoldConnectionRequest(ucid, deviceId);
            }
        });

        $('#doTransfer').click(function () {
            //hay que arreglar el selector
            var heldUcid = $('input[type="radio"]:checked').val();
            if (notEmpty(heldUcid)) {
                var activeUcid = localStorage.getItem('ucid');
                if (notEmpty(activeUcid))
                    if (notEmpty(deviceId))
                        agent.server.sendTransferCall(heldUcid, activeUcid, deviceId);
                    else
                        infoNoty("No se puede reconocer el dispositivo asociado al agente activo!");
                else
                    infoNoty("No hay llamada activa!");
            }
            else
                infoNoty("Debe seleccionar una llamada en espera!");
        });
    });
});
