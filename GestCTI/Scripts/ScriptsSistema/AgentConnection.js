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
    var panel = $('#lista_espera');
    panel.find('ul').remove();

    if (lista.length)
        panel.append("<ul class='list-unstyled'></ul>");

    for (var i in lista) {
        var ind = i + 1;
        panel.find('ul').append("<li><a href='#' class='list-group-item form-group'><label class='check contacts-title'><input type='radio' class='icheckbox' name='hold_list' value='" + lista[i].ucid + "' />" + Resources.Llamada + " " + ind + "</label><p>" + Resources.Device + ": " + lista[i].toDevice + "</p></a></li>");
    }

    //reinicializar el plugin icheck
    if ($(".icheckbox").length > 0) {
        $(".icheckbox,.iradio").iCheck({ checkboxClass: 'icheckbox_minimal-grey', radioClass: 'iradio_minimal-grey' });
    }
}

function changeState(id, enable) {
    if (enable)
        $('#' + id).removeAttr('disabled');
    else
        $('#' + id).attr('disabled', 'disabled');
}

function updateControlsState(list, enable = true) {
    var map = {
        'ready':           'ReadyToWork',
        'pause':           'doPause',
        'answer':          'acceptCallRequest',
        'hold':            'doHoldConnection',
        'retrieve':        'doRetrieve',
        'transfer':        'doTransfer',
        'conference':      'doConference',
        'end_conference':  'doEndConference',
        'hangout':         'hangoutCallRequest'
    };
    for (var button in map)
        changeState(map['button'], !enable);

    for (var i in list) {
        var button_id = map[ list[i] ];
        if (button_id != undefined)
            changeState(button_id, enable);
    }
}

$(function () {
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;

    agent.client.inicializarApp = function (message) {
        var response = JSON.parse(message);
        if (response['success']) {
            var result = JSON.parse(response.result);
            var agentData  = result[0];
            var deviceData = result[1];
            var state = agentData['State'];
            if (state != AgentState.AS_READY)
                updateControlsState(['ready']);
            else
                updateControlsState(['ready'], false);
        }

        spinnerHide();
    }

    agent.client.Notification = function (message, type = "success") {
        notify(message, type);
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

/*    agent.client.resultHoldConnections = function (response) {
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
*/

    agent.client.getAmReady = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            successNoty(Resources.ChangeToReadyOk);
        } else {
            errorNoty(Resources.ChangeToReadyFail);
            //$("#ReadyToWork").removeAttr("disabled");
        }
    };

    agent.client.receiveAcceptCallRequest = function (response) {
        json = JSON.parse(response);
        if (json.success === false) {
            //$("#acceptCallRequest").attr("disabled", "disabled");
            localStorage.removeItem('ucid');
            successNoty(Resources.InCall);
        }
        // Do nothing or check is fail call request 
    };

    agent.client.onEventHandler = function (response, data) {
        json = JSON.parse(response);
        var eventName = json.request.request;
        var eventArgs = json.request.args;
        var deviceId = localStorage.getItem('deviceId');

        switch (eventName) {
            case 'onServiceInitiated':
               // $("#hangoutCallRequest").removeAttr("disabled");

                tempNoty('onServiceInitiated');
                break;

            case 'onCallOriginated':

                tempNoty('onCallOriginated');
                break;

            case 'onCallDelivered':
                //localStorage.setItem('ucid', eventArgs[0]);
                //$('#doHoldConnection').removeAttr('disabled');
                localStorage.setItem('activeCall', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2] }));
               // $('#acceptCallRequest').removeAttr('disabled');
                infoNoty(Resources.IncomingCall);

                tempNoty('onCallDelivered');
                break;

            case 'onCallExternalDelivered':
                //localStorage.setItem('ucid', eventArgs[0]);
                //$('#doHoldConnection').removeAttr('disabled');
                localStorage.setItem('activeCall', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2] }));
                // $('#acceptCallRequest').removeAttr('disabled');
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
                var myId = localStorage.getItem('deviceId');
                var activeCall = { 'ucid': eventArgs[0], 'deviceId': ((eventArgs[4] != myId) ? eventArgs[4] : eventArgs[5]) };
                localStorage.setItem('activeCall', JSON.stringify(activeCall));
                //$("#hangoutCallRequest").removeAttr("disabled");
                //$("#acceptCallRequest").attr("disabled", "disabled");
                //$("#doHoldConnection").removeAttr("disabled");

                tempNoty('onEstablishedConnection');
                break;

            case 'onHoldConnection':
                pintarListaEspera(data);
                //$('#inputPhone').text('').removeAttr('disabled');
                localStorage.removeItem('activeCall');

                tempNoty('onHoldConnection');
                break;

            case 'onHoldPartyConnection':
                
                tempNoty('onHoldPartyConnection');
                break;

            case 'onRetrieveConnection':
                localStorage.setItem('activeCall', JSON.stringify({
                    'ucid': eventArgs[0],
                    'deviceId': eventArgs[2]
                }));
                pintarListaEspera(data);

                tempNoty('onRetrieveConnection');
                break;

            case 'onRetrievePartyConnection':

                tempNoty('onRetrievePartyConnection');
                break;

            case 'onEndConnection':
                localStorage.removeItem('activeCall');
               // $("#hangoutCallRequest").attr("disabled", "disabled");

                tempNoty('onEndConnection');
                break;

            case 'onEndPartyConnection':

                tempNoty('onEndPartyConnection');
                break;

            case 'onEndCall':
               // $("#hangoutCallRequest").attr("disabled", "disabled");
               // $("#ReadyToWork").removeAttr("disabled");
               // $("#inputPhone").removeAttr("disabled");
               // $("#inputPhone").val('');
               // $('#doHoldConnection').attr('disabled', 'disabled');
                pintarListaEspera(data);
                localStorage.removeItem('activeCall');

                tempNoty('onEndCall');
                break;

            case 'onTransferredCall':
                localStorage.setItem('activeCall', JSON.stringify({
                    'ucid': eventArgs[4],
                    'deviceId': ((eventArgs[5] != deviceId) ? eventArgs[5] : eventArgs[6])
                }));
                pintarListaEspera(data);

                tempNoty('onTransferredCall');
                break;

            case 'onConferencedCall':
                localStorage.setItem('activeCall', JSON.stringify({
                    'ucid': eventArgs[4],
                    'deviceId': ((eventArgs[5] != deviceId) ? eventArgs[5] : eventArgs[6])
                }));
                pintarListaEspera(data);

                tempNoty('onConferencedCall');
                break;

            case 'onAgentChangedState': {
                var agentState = eventArgs[1];
                switch (agentState) {
                    case AgentState.AS_READY:
                       // $("#ReadyToWork").attr("disabled", "disabled");
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
           // $('#inputPhone').attr('disabled', 'disabled');
           // $('#doCallBtn').attr('disabled', 'disabled');
            successNoty(Resources.Calling);
        } else {
            errorNoty(Resources.MakeCallFail);
        }
    };


    // Start the connection.
    $.connection.hub.start().done(function () {
        var deviceId = localStorage.getItem('deviceId');

        spinnerShow();
        agent.server.inicializarApp();

        $('#ReadyToWork').click(function () {
            // Put de agent to AM_READY and MANUAL_IN
           // $("#ReadyToWork").attr("disabled", "disabled");
            agent.server.sendStateReadyManual(deviceId);
        });

        $('#LogOutCore').click(function () {
            agent.server.sendLogOutCore(deviceId);
        });

        $("#acceptCallRequest").click(function () {
            var strAC = localStorage.getItem('activeCall');
            if (notEmpty(strAC)) {
                var activeCall = JSON.parse(strAC);
                if (notEmpty(activeCall.ucid)) {
                    agent.server.sendCTIAnswerCallRequest(activeCall.ucid, deviceId);
                } else {
                    console.error(Resources.NotUcid);
                }
            }
            else infoNoty("No hay llamada activa!");
        });

        $("#hangoutCallRequest").click(function () {
            var strAC = localStorage.getItem('activeCall');
            if (notEmpty(strAC)) {
                var activeCall = JSON.parse(strAC);
                if (notEmpty(activeCall.ucid)) {
                    agent.server.sendCTIClearConnectionRequest(activeCall.ucid, deviceId);
                } else {
                    errorNoty(Resources.NotUcid);
                }
            }
            else infoNoty("No hay llamada activa!");
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
            var strAC = localStorage.getItem('activeCall');
            if (notEmpty(strAC)) {
                var activeCall = JSON.parse(strAC);
                if (notEmpty(activeCall.ucid) && notEmpty(deviceId)) {
                   // $("#doHoldConnection").attr("disabled", "disabled");
                    agent.server.sendCTIHoldConnectionRequest(activeCall.ucid, deviceId);
                }
            }
            else
                infoNoty("No hay Llamada activa!");
        });

        $('#doTransfer').click(function () {
            var heldUcid = $('#lista_espera input[type="radio"]:checked').val();
            if (notEmpty(heldUcid)) {
                var strAC = localStorage.getItem('activeCall');
                if (notEmpty(strAC)) {
                    var activeCall = JSON.parse(strAC);
                    if (notEmpty(deviceId))
                        agent.server.sendTransferCall(heldUcid, activeCall.ucid, deviceId);
                    else
                        infoNoty("No se puede reconocer el dispositivo asociado al agente activo!");
                }
                else
                    infoNoty("No hay llamada activa!");
            }
            else
                infoNoty("Debe seleccionar una llamada en espera!");
        });

        $('#doRetrieve').click(function () {
            var radio = $('#lista_espera input[type="radio"]:checked');
            var heldUcid = radio.val();
            if (notEmpty(heldUcid)) {
                var strAC = localStorage.getItem('activeCall');
                if (notEmpty(strAC))
                    infoNoty("No se puede recuperar porque hay una llamada activa!");
                else {
                    agent.server.sendRetrieveCall(heldUcid, deviceId);
                }
            }
            else
                infoNoty("Debe seleccionar una llamada en espera!");
        });

        $('#doConference').click(function () {
            var heldUcid = $('#lista_espera input[type="radio"]:checked').val();
            if (notEmpty(heldUcid)) {
                var strAC = localStorage.getItem('activeCall');
                if (notEmpty(strAC)) {
                    var activeCall = JSON.parse(strAC);
                    if (notEmpty(deviceId))
                        agent.server.sendConferenceCall(heldUcid, activeCall.ucid, deviceId);
                    else
                        infoNoty("No se puede reconocer el dispositivo asociado al agente activo!");
                }
                else
                    infoNoty("No hay llamada activa!");
            }
            else
                infoNoty("Debe seleccionar una llamada en espera!");
        });

        $('#doEndConference').click(function () {
            var strAC = localStorage.getItem('activeCall');
            if (notEmpty(strAC)) {
                var activeCall = JSON.parse(strAC);
                agent.server.sendCTIClearCallRequest(activeCall.ucid);
            }
            else
                infoNoty("No hay llamada activa!");
        });
    });
});
