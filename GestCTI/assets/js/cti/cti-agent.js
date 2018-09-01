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
}


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
        'whisper': 'doWhisperRequest'
    };
    var id = map[alias] !== undefined ? map[alias] : alias;

    if (enable)
        $('#' + id).removeAttr('disabled');
    else
        $('#' + id).attr('disabled', 'disabled');
}

function handlingEvent(response, data) {
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
            changeState('inputPhone', false);
            changeState('doCallBtn', false);
            changeState('answer', true);

            localStorage.setItem('activeCall', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2] }));

            printDisposition(eventArgs[9]);      //cargo las dispositions segun el VDN de la llamada
            localStorage.setItem('callforsave', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2], 'deviceCustomer': eventArgs[4] }));

            infoNoty(Resources.IncomingCall);

            tempNoty('onCallDelivered');
            break;

        case 'onCallExternalDelivered':
            changeState('inputPhone', false);
            changeState('doCallBtn', false);
            //changeState('answer', true);

            localStorage.setItem('activeCall', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2] }));


            // printDisposition(eventArgs[9]);
            localStorage.setItem('callforsave', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2], 'deviceCustomer': eventArgs[5] }));

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
            var activeCall = { 'ucid': eventArgs[0], 'deviceId': eventArgs[4] !== myId ? eventArgs[4] : eventArgs[5] };
            localStorage.setItem('activeCall', JSON.stringify(activeCall));

            changeState('hangout', true);
            changeState('hold', true);
            changeState('answer', false);
            changeState('pause', false);
            changeState('whisper', false);

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
            localStorage.removeItem('activeCall');
            changeState('hangout', false);
            changeState('hold', false);
            changeState('pause', true);
            changeState('whisper', true);

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
            changeState('whisper', true);
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