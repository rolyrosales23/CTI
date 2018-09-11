
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

function setActiveCall(ucid) {
    localStorage.setItem('activeCall', ucid);

    changeState('hangout', true);
    changeState('hold', true);
    changeState('retrieve', false);
    if ($('#lista_espera input:checked').length) {
        changeState('conference', true);
        changeState('transfer', true);
    }

    changeState('inputPhone', false);
    changeState('doCallBtn', false);
}

function removeActiveCall() {
    localStorage.removeItem('activeCall');

    changeState('hangout', false);
    changeState('hold', false);
    changeState('conference', false);
    changeState('transfer', false);
    changeState('end_conference', false);
    if ($('#lista_espera input:checked').length)
        changeState('retrieve', true);

    $('#inputPhone').val('').removeAttr('disabled');
}

function pintarListaEspera(lista) {
    var panel = $('#lista_espera');
    panel.find('ul').remove();

    if (notEmpty(lista)) {
        panel.append("<ul class='list-unstyled'></ul>");

        for (var i in lista) {
            var ind = Number(i) + 1;
            panel.find('ul').append("<li><a href='#' class='list-group-item form-group'><label class='check contacts-title'><input type='radio' class='icheckbox' name='hold_list' value='" + lista[i].ucid + "' />" + Resources.Llamada + " " + ind + "</label><p>" + Resources.CallerId + ": " + lista[i].toDevice + "</p></a></li>");
        }

        $('#tab-first').removeClass('active');
        $('#li_tab-first').removeClass('active');
        $('#tab-third').removeClass('active');
        $('#li_tab-third').removeClass('active');

        $('#tab-second').addClass('active');
        $('#li_tab-second').addClass('active');

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
        successNoty(Resources.Calling, false);
    } else {
        errorNoty(Resources.MakeCallFail, false);
    }
};

function receiveAcceptCallRequest(response) {
    json = JSON.parse(response);
    if (json.success === false) {
        changeState('answer', false);
        errorNoty(json.response, false);
    }
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
    var activeCall = localStorage.getItem('activeCall');
    if (notEmpty(activeCall)) {
        agent.server.sendCTIAnswerCallRequest(activeCall, deviceId);
    }
    else infoNoty(Resources.NoActiveCall, false);
};

function hangoutCallRequest(agent) {
    var deviceId = localStorage.getItem('deviceId');
    var activeCall = localStorage.getItem('activeCall');
    var superListener = localStorage.getItem('ucid-listener');
    // var superWhisper = localStorage.getItem('ucid-whisper');
    var superWhisper = null;
    if (notEmpty(activeCall)) {
        if (notEmpty(superWhisper)) {
            // agent.server.sendCTIListenRetrieveAllRequest(deviceId, activeCall);
        } else if (notEmpty(superListener)) {
            agent.server.sendCTIListenRetrieveAllRequest(deviceId, activeCall);
        } else {
            agent.server.sendCTIClearConnectionRequest(activeCall, deviceId);
        }
    }
    else infoNoty(Resources.NoActiveCall, false);
};

function doHoldConnection(agent) {
    var deviceId = localStorage.getItem('deviceId');
    var activeCall = localStorage.getItem('activeCall');
    if (notEmpty(activeCall)) {
        if (notEmpty(deviceId)) {
            changeState('hold', false);
            agent.server.sendCTIHoldConnectionRequest(activeCall, deviceId);
        }
    }
    else
        infoNoty(Resources.NoActiveCall, false);
};

function doTransfer(agent) {
    var deviceId = localStorage.getItem('deviceId');
    var heldUcid = $('#lista_espera input[type="radio"]:checked').val();
    if (notEmpty(heldUcid)) {
        var activeCall = localStorage.getItem('activeCall');
        if (notEmpty(activeCall)) {
            if (notEmpty(deviceId))
                agent.server.sendTransferCall(heldUcid, activeCall, deviceId);
            else
                infoNoty(Resources.NoActiveDevice, false);
        }
        else
            infoNoty(Resources.NoActiveCall, false);
    }
    else
        infoNoty(Resources.SelectHoldCall, false);
};

function doRetrieve(agent) {
    var deviceId = localStorage.getItem('deviceId');
    var radio = $('#lista_espera input[type="radio"]:checked');
    var heldUcid = radio.val();
    if (notEmpty(heldUcid)) {
        var strAC = localStorage.getItem('activeCall');
        if (notEmpty(strAC))
            infoNoty(Resources.NotRetrieve, false);
        else {
            agent.server.sendRetrieveCall(heldUcid, deviceId);
        }
    }
    else
        infoNoty(Resources.SelectHoldCall, false);
};

function doConference() {
    var deviceId = localStorage.getItem('deviceId');
    var heldUcid = $('#lista_espera input[type="radio"]:checked').val();
    if (notEmpty(heldUcid)) {
        var activeCall = localStorage.getItem('activeCall');
        if (notEmpty(activeCall)) {
            if (notEmpty(deviceId))
                agent.server.sendConferenceCall(heldUcid, activeCall, deviceId);
            else
                infoNoty(Resources.NoActiveDevice, false);
        }
        else
            infoNoty(Resources.NoActiveCall, false);
    }
    else
        infoNoty(Resources.SelectHoldCall, false);
};

function endCall(agent) {
    var activeCall = localStorage.getItem('activeCall');
    if (notEmpty(activeCall)) {
        agent.server.sendCTIClearCallRequest(activeCall);
    }
    else
        infoNoty(Resources.NoActiveCall, false);
};

function onEstablishedConnection(ucid, deviceId, partyDeviceId) {
    setActiveCall(ucid);
    changeState('hangout', true);
    changeState('hold', true);
    changeState('answer', false);
    //changeState('pause', false);
}

function handlingEvent(response, data) {
    var deviceId = localStorage.getItem('deviceId');
    json = JSON.parse(response);
    var eventName = json.request.request;
    var eventArgs = json.request.args;
    var deviceId = localStorage.getItem('deviceId');
    switch (eventName) {
        case 'onServiceInitiated':
            changeState('hangout', true);

            tempNoty('onServiceInitiated');
            break;

        case 'onCallOriginated':

            tempNoty('onCallOriginated');
            break;

        case 'onCallDelivered':
            changeState('answer', true);

            setActiveCall(eventArgs[0]);

            // printDisposition(eventArgs[9]);      //cargo las dispositions segun el VDN de la llamada
            // localStorage.setItem('callforsave', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2], 'deviceCustomer': eventArgs[4] }));

            infoNoty(Resources.IncomingCall);

            tempNoty('onCallDelivered');
            break;

        case 'onCallExternalDelivered':

            setActiveCall(eventArgs[0]);

            // printDisposition(eventArgs[9]);
            // localStorage.setItem('callforsave', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2], 'deviceCustomer': eventArgs[5] }));

            tempNoty('onCallExternalDelivered');
            break;

        case 'onCallDiverted':
            changeState('answer', false);
            if (eventArgs[3] === deviceId)
                removeActiveCall();

            infoNoty(Resources.CallDiverted, false);
            tempNoty('onCallDiverted');
            break;

        case 'onCallFailed':
            infoNoty(Resources.CallFailed, false);
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
            removeActiveCall();

            successNoty(Resources.CallOnHold, false);
            tempNoty('onHoldConnection');
            break;

        case 'onHoldPartyConnection':

            tempNoty('onHoldPartyConnection');
            break;

        case 'onRetrieveConnection':
            pintarListaEspera(data);
            setActiveCall(eventArgs[0]);

            successNoty(Resources.CallRetrieved, false);
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

            removeActiveCall();
            //changeState('pause', true);

            infoNoty(Resources.CallEnded, false);
            tempNoty('onEndConnection');
            break;

        case 'onEndPartyConnection':
            changeState('answer', false);
            tempNoty('onEndPartyConnection');
            break;

        case 'onEndCall':
            pintarListaEspera(data);
            removeActiveCall();

            tempNoty('onEndCall');
            break;

        case 'onTransferredCall':
            setActiveCall(eventArgs[4]);
            pintarListaEspera(data);

            successNoty(Resources.CallTransferred, false);
            tempNoty('onTransferredCall');
            break;

        case 'onConferencedCall':
            setActiveCall(eventArgs[4]);
            pintarListaEspera(data);

            successNoty(Resources.CallConferenced, false);
            tempNoty('onConferencedCall');
            break;

        case 'onAgentChangedState': {
            //var agentState = eventArgs[1];
            //if (agentState === AgentState.AS_READY) {
            //    updateControlsState(['pause']);

            //    var timeout_id = localStorage.getItem('timeout_id');
            //    if (notEmpty(timeout_id)) {
            //        clearTimeout(timeout_id);
            //        localStorage.removeItem('timeout_id');
            //    }
            //}
            //else
            //    updateControlsState(['ready']);

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