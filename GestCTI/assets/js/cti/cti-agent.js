function pintarListaEspera(lista) {
    var panel = $('#lista_espera');
    panel.find('ul').remove();

    if (notEmpty(lista)) {
        panel.append("<ul class='list-unstyled'></ul>");

        for (var i in lista) {
            var ind = Number(i) + 1;
            panel.find('ul').append("<li><a href='#' class='list-group-item form-group'><label class='check contacts-title'><input type='radio' class='icheckbox' name='hold_list' value='" + lista[i].ucid + "' />" + Resources.Llamada + " " + ind + "</label><p>" + Resources.Device + ": " + lista[i].toDevice + "</p></a></li>");
        }

        //reinicializar el plugin icheck
        if ($(".icheckbox").length > 0) {
            $(".icheckbox,.iradio").iCheck({ checkboxClass: 'icheckbox_minimal-grey', radioClass: 'iradio_minimal-grey' });
        }
    }
};
//############ HANDLING ACTION BUTTON ########################

function addCTIMakeCallRequest(response) {
    json = JSON.parse(response);
    if (json['success'] === true) {
        successNoty(Resources.Calling);
    } else {
        errorNoty(Resources.MakeCallFail);
    }
};

function receiveAcceptCallRequest(response) {
    json = JSON.parse(response);
    if (json.success === false) {
        changeState('answer', false);
        localStorage.removeItem('ucid');
        successNoty(Resources.InCall);
    }
    // Do nothing or check is fail call request 
};

function enEspera(call, holdList) {
    for (var i in holdList)
        if (call[1] === holdList[i].ucid)
            return true;
    return false;
};

//###########    ACTION BUTTON    ############################
function acceptCallRequest(agent) {
    var deviceId = localStorage.getItem('deviceId');
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
};

function hangoutCallRequest(agent) {
    var deviceId = localStorage.getItem('deviceId');
    var strAC = localStorage.getItem('activeCall');
    var superListener = localStorage.getItem('ucid-listener');
    // var superWhisper = localStorage.getItem('ucid-whisper');
    if (notEmpty(strAC)) {
        var activeCall = JSON.parse(strAC);
        if (notEmpty(activeCall.ucid)) {
            if (notEmpty(superWhisper)) {
                // agent.server.sendCTIListenRetrieveAllRequest(deviceId, activeCall.ucid);
            } else if (notEmpty(superListener)) {
                agent.server.sendCTIListenRetrieveAllRequest(deviceId, activeCall.ucid);
            } else {
                agent.server.sendCTIClearConnectionRequest(activeCall.ucid, deviceId);
            }
        } else {
            errorNoty(Resources.NotUcid);
        }
    }
    else infoNoty("No hay llamada activa!");
};

function doHoldConnection(agent) {
    var deviceId = localStorage.getItem('deviceId');
    var strAC = localStorage.getItem('activeCall');
    if (notEmpty(strAC)) {
        var activeCall = JSON.parse(strAC);
        if (notEmpty(activeCall.ucid) && notEmpty(deviceId)) {
            changeState('hold', false);
            agent.server.sendCTIHoldConnectionRequest(activeCall.ucid, deviceId);
        }
    }
    else
        infoNoty("No hay Llamada activa!");
};

function doTransfer(agent) {
    var deviceId = localStorage.getItem('deviceId');
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
};

function doRetrieve(agent) {
    var deviceId = localStorage.getItem('deviceId');
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
};

function doConference() {
    var deviceId = localStorage.getItem('deviceId');
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
};

function endCall(agent) {
    var strAC = localStorage.getItem('activeCall');
    if (notEmpty(strAC)) {
        var activeCall = JSON.parse(strAC);
        agent.server.sendCTIClearCallRequest(activeCall.ucid);
    }
    else
        infoNoty("No hay llamada activa!");
};

function changeState(alias, enable) {
    var map = {
        'ready': 'ReadyToWork',
        'pause': 'doPause',
        'answer': 'acceptCallRequest',
        'hold': 'doHoldConnection',
        'retrieve': 'doRetrieve',
        'transfer': 'doTransfer',
        'conference': 'doConference',
        'end_conference': 'doEndConference',
        'hangout': 'hangoutCallRequest',
    };
    var id = map[alias] !== undefined ? map[alias] : alias;

    if (enable)
        $('#' + id).removeAttr('disabled');
    else
        $('#' + id).attr('disabled', 'disabled');
};

function onEstablishedConnection(ucid, deviceId, partyDeviceId) {
    var activeCall = { 'ucid': ucid, 'deviceId': partyDeviceId };
    localStorage.setItem('activeCall', JSON.stringify(activeCall));
    changeState('hangout', true);
    changeState('hold', true);
    changeState('answer', false);
    changeState('pause', false);
}

function handlingEvent(response, data) {
    var deviceId = localStorage.getItem('deviceId');
    json = JSON.parse(response);
    var eventName = json.request.request;
    var eventArgs = json.request.args;
    var deviceId = localStorage.getItem('deviceId');
    console.log(response);
    switch (eventName) {
        case 'onServiceInitiated':
            changeState('hangout', true);

            tempNoty('onServiceInitiated');
            break;

        case 'onCallOriginated':

            tempNoty('onCallOriginated');
            break;

        case 'onCallDelivered':
            changeState('inputPhone', false);
            changeState('doCallBtn', false);
            changeState('answer', true);

            localStorage.setItem('activeCall', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2] }));

            // printDisposition(eventArgs[9]);      //cargo las dispositions segun el VDN de la llamada
            // localStorage.setItem('callforsave', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2], 'deviceCustomer': eventArgs[4] }));

            infoNoty(Resources.IncomingCall);

            tempNoty('onCallDelivered');
            break;

        case 'onCallExternalDelivered':
            changeState('inputPhone', false);
            changeState('doCallBtn', false);
            //changeState('answer', true);

            localStorage.setItem('activeCall', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2] }));

            // printDisposition(eventArgs[9]);
            // localStorage.setItem('callforsave', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2], 'deviceCustomer': eventArgs[5] }));

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
            var anotherPartyDeviceId = eventArgs[4] !== myId ? eventArgs[4] : eventArgs[5];

            onEstablishedConnection(eventArgs[0], myId, anotherPartyDeviceId);

            tempNoty('onEstablishedConnection');
            break;

        case 'onHoldConnection':
            pintarListaEspera(data);
            $('#inputPhone').text('').removeAttr('disabled');
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
            if (localStorage.getItem('ucid-listener') === eventArgs[0] ||
                localStorage.getItem('ucid-whisper') === eventArgs[0]) {
                localStorage.removeItem('ucid-listener');
                localStorage.removeItem('ucid-whisper');
            }

            localStorage.removeItem('activeCall');
            changeState('hangout', false);
            changeState('hold', false);
            changeState('pause', true);

            if (localStorage.getItem('IsCampaignCall') === 'true') {
                $('#modal-dispositions').modal('show');
            }

            $("#inputPhone").val('').removeAttr("disabled");

            tempNoty('onEndConnection');
            break;

        case 'onEndPartyConnection':

            tempNoty('onEndPartyConnection');
            break;

        case 'onEndCall':
            changeState('hangout', false);
            changeState('hold', false);
            changeState('pause', true);
            //changeState('ready', true);
            $("#inputPhone").val('').removeAttr("disabled");

            pintarListaEspera(data);
            localStorage.removeItem('activeCall');

            tempNoty('onEndCall');
            break;

        case 'onTransferredCall':
            localStorage.setItem('activeCall', JSON.stringify({
                'ucid': eventArgs[4],
                'deviceId': eventArgs[5] !== deviceId ? eventArgs[5] : eventArgs[6]
            }));
            pintarListaEspera(data);

            tempNoty('onTransferredCall');
            break;

        case 'onConferencedCall':
            localStorage.setItem('activeCall', JSON.stringify({
                'ucid': eventArgs[4],
                'deviceId': eventArgs[5] !== deviceId ? eventArgs[5] : eventArgs[6]
            }));
            pintarListaEspera(data);

            tempNoty('onConferencedCall');
            break;

        case 'onAgentChangedState': {
            var agentState = eventArgs[1];
            if (agentState === AgentState.AS_READY) {
                updateControlsState(['pause']);

                var timeout_id = localStorage.getItem('timeout_id');
                if (notEmpty(timeout_id)) {
                    clearTimeout(timeout_id);
                    localStorage.removeItem('timeout_id');
                }
            }
            else
                updateControlsState(['ready']);

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
}