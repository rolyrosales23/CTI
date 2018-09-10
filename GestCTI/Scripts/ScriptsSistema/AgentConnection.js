var AgentState = {
    AS_NOT_READY: 0,
    AS_LOGGED_OUT: 1,
    AS_READY: 2,
    AS_AFTER_CALL: 3,
    AS_WORK_READY: 4
};

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
        'hangout': 'hangoutCallRequest'
    };
    var id = (map[alias] !== undefined) ? map[alias] : alias;

    if (enable)
        $('#' + id).removeAttr('disabled');
    else
        $('#' + id).attr('disabled', 'disabled');
}

function updateControlsState(list, enable = true) {
    var buttons = ['ready', 'pause', 'answer', 'hold', 'retrieve', 'transfer', 'conference', 'end_conference', 'hangout'];
    for (var alias in buttons)
        changeState(buttons[alias], !enable);

    for (var i in list)
        changeState(list[i], enable);
}

function pintarScriptByVDN(vdn, data) {
    $.ajax({
        url: "../Home/GetUrlScriptByVDN/",
        data: { vdn: vdn },
        success: function (url) {
            pintarScript(url, data);
        },
        error: function (error) {
            errorNoty(Resources.ErrorScript, false);
        }
    });
}

function printDisposition(vdn) {
    if (notEmpty(vdn)) {
        var select = $('#SelDisposition');
        select.find('option').remove();
        select.attr('disabled', 'disabled');
        $.ajax({
            url: "../Home/GetDispositionsByVDN/",
            data: { vdn: vdn },
            success: function (resp) {
                if (notEmpty) {
                    select.append("<option disabled selected value>" + Resources.SelectDisposition + "</option>");
                    for (var i in resp) {
                        select.append("<option value='" + resp[i].Id + "'>" + resp[i].Name + "</option>");
                    }
                    select.selectpicker('refresh');
                    localStorage.setItem('IsCampaignCall', 'true');
                }
                else {
                    localStorage.setItem('IsCampaignCall', 'false');
                }
            },
            error: function () {
                errorNoty(Resources.ErrorDispositions, false);
            },
            complete: function () {
                select.removeAttr('disabled');
                select.selectpicker('refresh');
            }
        });
    }
    localStorage.setItem('IsCampaignCall', 'false');
}


$('#SendCallDisposition').click(function () {
    var dispositionCamp = $('#SelDisposition').val();
    var strCforS = localStorage.getItem('callforsave');
    if (notEmpty(strCforS)) {
        var CallForSave = JSON.parse(strCforS);
        spinnerShow();
        $.ajax({
            url: "../Home/SaveCallDisposition/",
            type: "post",
            data: { ucid: CallForSave.ucid, disposition: dispositionCamp, username: User.Name, deviceId: CallForSave.deviceId, deviceCustomer: CallForSave.deviceCustomer },
            success: function (resp) {
                infoNoty(Resources.CallSavedOK, false);
            },
            error: function () {
                errorNoty(Resources.CallSavedFail, false);
            },
            complete: function () {
                spinnerHide();
            }
        });
    }
});

function printCampaignsByUser() {
    var select = $('#SelCampaigns');
    select.find('option').remove();
    spinnerShow();
    $.ajax({
        url: "../Home/GetCampaignsByUser/",
        data: { username: User.Name },
        success: function (resp) {
            if (notEmpty(resp)) {
                select.append("<option selected value>" + Resources.SelectCampaign + "</option>");
                for (var i in resp) {
                    select.append("<option value='" + resp[i].Id + "'>" + resp[i].Name + "</option>");
                }
                select.selectpicker('refresh');
            }
            else
                infoNoty(Resources.UserNotCampaign, false);
        },
        error: function () {
            errorNoty(Resources.ObtCampUserError, false);
        },
        complete: function () {
            spinnerHide();
        }
    });
}

$(function () {
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;

    agent.client.inicializarAppFase1 = function (message, holdList) {
        pintarListaEspera(holdList);

        var response = JSON.parse(message);
        if (response['success']) {
            var result = JSON.parse(response.result);
            var agentData = result[0];
            var deviceData = result[1];
            var state = agentData['State'];

            if (state != AgentState.AS_READY)
                updateControlsState(['ready']);
            else if (deviceData['Busy']) {
                updateControlsState(['hold', 'end_conference', 'hangout']);
                agent.server.inicializarApp(2, deviceData['DeviceId']);
                return;
            }
            else
                updateControlsState(['pause']);
        }

        spinnerHide();
    }

    function enEspera(call, holdList) {
        for (var i in holdList)
            if (call[1] == holdList[i].ucid)
                return true;
        return false;
    }

    agent.client.inicializarAppFase2 = function (message, holdList) {
        var response = JSON.parse(message);
        if (response['success'])
            for (var i in response.result) {
                var deviceId = localStorage.getItem('deviceId');
                var call = response.result[i];
                if (!enEspera(call, holdList))
                    localStorage.setItem('activeCall', JSON.stringify({ 'ucid': call[1], 'deviceId': deviceId }));
            }

        spinnerHide();
    }

    agent.client.Notification = function (message, type = "success", MsgDebugMode = true) {
        notify(message, type, MsgDebugMode);
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
                errorNoty(json.response, false);
            }
        }
    };

    agent.client.getAmReady = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            successNoty(Resources.ChangeToReadyOk, false);
        } else {
            errorNoty(Resources.ChangeToReadyFail);
            changeState('ready', true);
        }
    };

    agent.client.receiveAcceptCallRequest = function (response) {
        json = JSON.parse(response);
        if (json.success === false) {
            changeState('answer', false);
            localStorage.removeItem('ucid');
            errorNoty(json.response, false);
        }
    };

    agent.client.onEventHandler = function (response, data) {
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
                pintarScriptByVDN(eventArgs[9], {
                    'ucid': eventArgs[0],
                    'callId': eventArgs[1],
                    'deviceId': eventArgs[2],
                    'alertingDeviceId': eventArgs[3],
                    'callingDeviceId': eventArgs[4],
                    'calledDeviceId': eventArgs[5],
                    'trunkGroupId': eventArgs[6],
                    'trunkMember': eventArgs[7],
                    'splitDeviceId': eventArgs[8],
                    'lastRedirectionDeviceId': eventArgs[9],
                    'callerId': eventArgs[10],
                    'appName': eventArgs[11]
                });
                localStorage.setItem('callforsave', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2], 'deviceCustomer': eventArgs[4] }));

                infoNoty(Resources.IncomingCall, false);

                tempNoty('onCallDelivered');
                break;

            case 'onCallExternalDelivered':
                changeState('inputPhone', false);
                changeState('doCallBtn', false);
                //changeState('answer', true);
                localStorage.setItem('activeCall', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2] }));

                //asignar la llamada a la campaña
                var campaignId = localStorage.getItem('campaignId');
                if (notEmpty(campaignId)) {
                    $.ajax({
                        url: '../CoreHttp/CampaignAssignCall/',
                        data: { campaignId: campaignId, ucid: eventArgs[0] },
                        success: function (response) {
                            if (response)
                                localStorage.setItem('IsCampaignCall', true);
                            else
                                localStorage.setItem('IsCampaignCall', false);
                        },
                        error: function () {
                            localStorage.setItem('IsCampaignCall', false);
                        }
                    });
                }

                printDisposition(campaignId);
                localStorage.setItem('callforsave', JSON.stringify({ 'ucid': eventArgs[0], 'deviceId': eventArgs[2], 'deviceCustomer': eventArgs[5] }));

                tempNoty('onCallExternalDelivered');
                break;

            case 'onCallDiverted':
                changeState('answer', false);
                tempNoty('onCallDiverted');
                infoNoty(Resources.CallDiverted, false);
                break;

            case 'onCallFailed':

                tempNoty('onCallFailed');
                infoNoty(Resources.CallFailed, false);
                break;

            case 'onEstablishedConnection':
                var myId = localStorage.getItem('deviceId');
                var activeCall = { 'ucid': eventArgs[0], 'deviceId': ((eventArgs[4] != myId) ? eventArgs[4] : eventArgs[5]) };
                localStorage.setItem('activeCall', JSON.stringify(activeCall));

                changeState('hangout', true);
                changeState('hold', true);
                changeState('answer', false);
                changeState('pause', false);


                tempNoty('onEstablishedConnection');
                break;

            case 'onHoldConnection':
                pintarListaEspera(data);
                $('#inputPhone').text('').removeAttr('disabled');
                localStorage.removeItem('activeCall');

                successNoty(Resources.CallOnHold, false);
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

                successNoty(Resources.CallRetrieved, false);
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

                if (localStorage.getItem('IsCampaignCall')  === 'true') {
                    $('#modal-dispositions').modal('show');
                }
                $("#inputPhone").val('').removeAttr("disabled");

                tempNoty('onEndConnection');
                infoNoty(Resources.CallEnded, false);
                break;

            case 'onEndPartyConnection':
                changeState('answer', false);
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
                    'deviceId': ((eventArgs[5] != deviceId) ? eventArgs[5] : eventArgs[6])
                }));
                pintarListaEspera(data);

                successNoty(Resources.CallTransferred, false);
                tempNoty('onTransferredCall');
                break;

            case 'onConferencedCall':
                localStorage.setItem('activeCall', JSON.stringify({
                    'ucid': eventArgs[4],
                    'deviceId': ((eventArgs[5] != deviceId) ? eventArgs[5] : eventArgs[6])
                }));
                pintarListaEspera(data);

                successNoty(Resources.CallConferenced, false);
                tempNoty('onConferencedCall');
                break;

            case 'onAgentChangedState': {
                var agentState = eventArgs[1];
                if (agentState == AgentState.AS_READY) {
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
    };

    agent.client.addCTIMakeCallRequest = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            successNoty(Resources.Calling, false);
        } else {
            errorNoty(Resources.MakeCallFail, false);
        }
    };

    //$.connection.hub.disconnected(function () {
    //    setTimeout(function () {
    //        $.connection.hub.start();
    //    }, 5000); // Restart connection after 5 seconds.
    //});

    // Start the connection.
    $.connection.hub.start().done(function () {
        var deviceId = localStorage.getItem('deviceId');

        spinnerShow();
        agent.server.inicializarApp(1, "");

        $('#ReadyToWork').click(function () {
            // Put de agent to AM_READY and MANUAL_IN
            changeState('ready', false);
            agent.server.sendStateReadyManual(deviceId);
        });

        $('#doPause').click(function () {
            var select = $('#SelPauseCodes');
            select.find('option').remove();
            spinnerShow();
            $.ajax({
                url: "../Home/GetPauseCodesByUser/",
                data: { username: User.Name },
                success: function (resp) {
                    if (notEmpty(resp)) {
                        select.append("<option selected value>" + Resources.SelectPauseCode + "</option>");
                        for (var i in resp) {
                            select.append("<option value='" + resp[i].Value + "' data-duration='" + resp[i].MaxDuration + "' data-id='" + resp[i].Id + "' data-auto='" + resp[i].AutoReady + "'>" + resp[i].Name + "</option>");
                        }
                        select.selectpicker('refresh');
                        $('#modal-PauseCodes').modal('show');
                    }
                    else {
                        errorNoty(Resources.NoPasusesAvailable, false);
                    }
                },
                error: function () {
                    errorNoty(Resources.NotPausesUser, false);
                },
                complete: function () {
                    spinnerHide();
                }
            });
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
                    errorNoty(Resources.NotUcid, false);
                }
            }
            else infoNoty(Resources.NoActiveCall, false);
        });

        $("#hangoutCallRequest").click(function () {
            var strAC = localStorage.getItem('activeCall');
            if (notEmpty(strAC)) {
                var activeCall = JSON.parse(strAC);
                if (notEmpty(activeCall.ucid)) {
                    agent.server.sendCTIClearConnectionRequest(activeCall.ucid, deviceId);
                } else {
                    errorNoty(Resources.NotUcid, false);
                }
            }
            else infoNoty(Resources.NoActiveCall, false);
        });

        $("#doCallBtn").click(function () {
            printCampaignsByUser();
            $('#modal-Campaigns').modal();            
        });

        $("#doHoldConnection").click(function () {
            var strAC = localStorage.getItem('activeCall');
            if (notEmpty(strAC)) {
                var activeCall = JSON.parse(strAC);
                if (notEmpty(activeCall.ucid) && notEmpty(deviceId)) {
                    changeState('hold', false);
                    agent.server.sendCTIHoldConnectionRequest(activeCall.ucid, deviceId);
                }
            }
            else
                infoNoty(Resources.NoActiveCall, false);
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
                        infoNoty(Resources.NoActiveDevice, false);
                }
                else
                    infoNoty(Resources.NoActiveCall, false);
            }
            else
                infoNoty(Resources.SelectHoldCall, false);
        });

        $('#doRetrieve').click(function () {
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
                        infoNoty(Resources.NoActiveDevice, false);
                }
                else
                    infoNoty(Resources.NoActiveCall, false);
            }
            else
                infoNoty(Resources.SelectHoldCall, false);
        });

        $('#doEndConference').click(function () {
            var strAC = localStorage.getItem('activeCall');
            if (notEmpty(strAC)) {
                var activeCall = JSON.parse(strAC);
                agent.server.sendCTIClearCallRequest(activeCall.ucid);
            }
            else
                infoNoty(Resources.NoActiveCall, false);
        });

        $('#SendPauseCode').click(function () {
            var reason   = $('#SelPauseCodes').val();
            var id       = $('#SelPauseCodes option:selected').data('id');
            var duration = $('#SelPauseCodes option:selected').data('duration');
            var auto     = $('#SelPauseCodes option:selected').data('auto');

            if (notEmpty(id)) {
                spinnerShow();
                $.ajax({
                    url: "../Home/SavePauseCodeUser/",
                    type: "post",
                    data: { username: User.Name, pausecode: id },
                    success: function (resp) {
                        $('#modal-PauseCodes').modal('hide');
                        var timeout_id = setTimeout(
                            function () {
                                stopCounter('#pause-counter');
                                if (auto) {
                                    $('#ReadyToWork').trigger('click');
                                    infoNoty(PauseTimeOutReady, false, {timeout: null});
                                }
                                else {
                                    $('#LogOutCore').trigger('click');
                                    infoNoty(Resources.PauseTimeOutLoggout, false, { timeout: null });
                                }
                            },
                            duration * 1000
                        );
                        startCounter('#pause-counter', duration);
                        localStorage.setItem('timeout_id', timeout_id);
                        agent.server.sendPause(deviceId, reason);
                    },
                    error: function () {
                        errorNoty(Resources.ErrorSavePauseCode, false);
                    },
                    complete: function () {
                        spinnerHide();
                    }
                });
            }
            else
                errorNoty(Resources.SelectValidPauseC, false);
        });

        $('#BtnCampaignCall').click(function () {
            var IdCampaign = $('#SelCampaigns').val();
            if (notEmpty(IdCampaign))
                localStorage.setItem('campaignId', IdCampaign);

            $('#modal-Campaigns').modal('hide');
            var toDevice = $('#inputPhone').val();
            if (notEmpty(deviceId) && notEmpty(toDevice)) {
                agent.server.sendCTIMakeCallRequest(deviceId, toDevice, "*99");
            }
            else {
                errorNoty(Resources.NotDevice, false);
            }
        });
    });
});
