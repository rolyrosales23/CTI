function pintarAgentList(agents, selector) {
    var wrapper = $(selector);
    wrapper.find('.panel-body').remove();
    wrapper.find('h3').remove();
    if (notEmpty(agents)) {
        var panelbody = $('<div class="panel-body panel-body-table"></div>')
            .append('<div class="table-responsive"></div>').find('div')
            .append('<table class="table table-bordered table-striped table-actions"></table>').end();

        var table = wrapper.append(panelbody).find('table');

        var header = $('<thead><tr></tr></thead>').find('tr')
            .append('<th width="50">UserName</th>')
            .append('<th width="100">status</th>')
            .append('<th width="100">phone_extension</th>')
            .append('<th width="100">action</th>')
            .end();

        var body = $('<tbody></tbody>');

        for (var i in agents) {
            var flag = (agents[i].CurrentUCID === null) ? "false" : "true";
            var check = $('<td><label class="check"></label></td>').find('label')
                .append('<input type="checkbox" class="icheckbox" id="' + i + '-check"/>')

            var buttom = "";
            if (flag === "true") {
                var btn1 = "<a type='button' onclick=listener('" + agents[i].CurrentUCID + "') class='btn btn-info btn-md fa fa-headphones info' title='" + Resources.Listen + "'></a>";
                var btn2 = '<a type="button" onclick=whisper("' + agents[i].CurrentUCID + '","' + agents[i].DeviceId + '") class="btn btn-warning btn-md fa fa-bullhorn info" title="' + Resources.Whisper + '"></a>';
                buttom = '<td><div class="row"><div class="col-md-12">' + btn1 + btn2 + '</div></div></td>';
            } else {
                var btn = '<a type=button" onclick=makeCall("' + agents[i].DeviceId + '") class="btn btn-success btn-md fa fa-phone info" title="' + Resources.Call + '"></a>';
                buttom = '<td><div class="row"><div class="col-md-6">' + btn + '</div></div></td>'
            }

            var fila = $('<tr></tr>')
                .append('<td>' + agents[i].user_name + '</td>')
                .append('<td>' + "connected" + '</td>')
                .append('<td id="' + i + '-deviceId">' + agents[i].DeviceId + '</td>')
                .append(buttom);
            body.append(fila);
        }

        table.append(header);
        table.append(body);
    }
    else
        wrapper.append('<h3 class="text-muted">No hay agentes conectados</h3>');
}

function pintarQueueCalls(queueCalls, selector) {
    var wrapper = $(selector);
    wrapper.find('.panel-body').remove();
    wrapper.find('h3').remove();
    if (notEmpty(queueCalls)) {
        var panelbody = $('<div class="panel-body panel-body-table"></div>')
            .append('<div class="table-responsive"></div>').find('div')
            .append('<table class="table table-bordered table-striped table-actions"></table>').end();

        var table = wrapper.append(panelbody).find('table');

        var header = $('<thead><tr></tr></thead>').find('tr')
            .append('<th width="50">ucid</th>')
            .append('<th width="100">call id</th>')
            .append('<th width="100">vdn</th>')
            .append('<th width="100">split</th>')
            .append('<th width="100">tiempo en espera</th>').end();

        var body = $('<tbody></tbody>');

        for (var i in queueCalls) {
            var waitTime = new Date(queueCalls[i].QueuedTime) - new Date(queueCalls[i].Time);
            var fila = $('<tr></tr>')
                .append('<td>' + queueCalls[i].UCID + '</td>')
                .append('<td>' + queueCalls[i].CallId + '</td>')
                .append('<td>' + queueCalls[i].VDN + '</td>')
                .append('<td>' + queueCalls[i].Split + '</td>')
                .append('<td>' + waitTime + '</td>');
            body.append(fila);
        }

        table.append(header);
        table.append(body);
    }
    else
        wrapper.append('<h3 class="text-muted">No hay llamadas en cola</h3>');
}



function showListOfQueueCalls() {
    var user = localStorage.getItem('user');

    $.get('../CoreHttp/QueuedCalls/' + user, ).done(function (result, textStatus, jqXHR) {
        var listCallsQueue = JSON.parse(result);
        pintarQueueCalls(listCallsQueue, '#table-queue-call-list');
        var panel = $('#get-queue-call-list').parents(".panel:last");
        if (panel.hasClass("panel-refreshing")) {
            panel_refresh(panel);
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        errorNoty("Error charging list of queue calls");
    });
}

function whisper(ucid, selectedParty) {
    var deviceId = localStorage.getItem('deviceId');
    if (!notEmpty(deviceId)) {
        errorNoty('No se podrá realizar ninguna acción sobre este usuario pues el supervisor no se ha logueado con un dispositivo');
        return;
    }
    var agent = $.connection.websocket;
    agent.server.sendCtiWhisperRequest(deviceId, ucid, selectedParty);
}

function listener(ucid) {
    var deviceId = localStorage.getItem('deviceId');
    if (!notEmpty(deviceId)) {
        errorNoty('No se podrá realizar ninguna acción sobre este usuario pues el supervisor no se ha logueado con un dispositivo');
        return;
    }
    var agent = $.connection.websocket;
    agent.server.sendCTIListenHoldAllRequest(deviceId, ucid);
}

function makeCall(selectedParty) {
    var deviceId = localStorage.getItem('deviceId');
    if (!notEmpty(deviceId)) {
        errorNoty('No se podrá realizar ninguna acción sobre este usuario pues el supervisor no se ha logueado con un dispositivo');
        return;
    }
    var agent = $.connection.websocket;
    agent.server.sendCTIMakeCallRequest(deviceId, selectedParty, "*99");
}

$(function () {
    initDeviceAction();

    function initDeviceAction() {
        var phoneExtension = localStorage.getItem('deviceId');
        if (notEmpty(phoneExtension)) {
            $('#right-side').removeClass('col-md-12').addClass('col-md-8');
            $("#loginExtension").remove();
            showPhoneView();
        }
    }

    function showPhoneView() {
        $.ajax({
            url: 'GetTelephone',
            contentType: 'application/html; charset=utf-8',
            type: 'GET',
            dataType: 'html'
        }).then(function (result) {
            $("#showPhone").html(result);

            $("#acceptCallRequest").click(function () {
                acceptCallRequest(agent)
            });

            $("#hangoutCallRequest").click(function () {
                hangoutCallRequest(agent)
            });

            $("#doHoldConnection").click(function () {
                doHoldConnection(agent)
            });

            $('#doTransfer').click(function () {
                doTransfer(agent)
            });

            $('#doRetrieve').click(function () {
                doRetrieve(agent)
            });

            $('#doConference').click(function () {
                doConference(agent)
            });

            $('#doEndConference').click(function () {
                endCall(agent)
            });

            $("#doCallBtn").click(function () {
                var deviceId = $('#inputPhone').val();
                makeCall(deviceId);
            });

            $('#inputPhone').keyup(function () {
                console.log('put a value ' + $(this).val());
                if ($(this).val())
                    $("#doCallBtn").removeAttr("disabled");
                else
                    $("#doCallBtn").attr("disabled", "disabled");
            });
        }).fail(function (xhr, status) {
            errorNoty("Error charging phone partial");
        });
    }

    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;

    agent.client.listOfAgent = function (agents) {
        pintarAgentList(agents, "#table-agents-connected");
        var panel = $('#get-agent-list').parents(".panel:last");
        if (panel.hasClass("panel-refreshing")) {
            panel_refresh(panel);
        }
    };

    agent.client.Notification = function (response, type = "success") {
        notify(response, type);
    };

    agent.client.sendUserConnected = function (CtiAgentList) {
        // Show list of agents
    };

    agent.client.addInitialize = function (message) {
        json = JSON.parse(message);
        if (json['success'] === true) {
            initDeviceAction();
            spinnerHide();
        } else {
            localStorage.removeItem('deviceId');
            spinnerHide();
            errorNoty('No se ha inicializado el dispositivo. Razones ' + json('reason'));
        }
    };

    agent.client.onEventHandler = function (response, data) {
        handlingEvent(response, data);
    };

    agent.client.receiveWhisperRequest = function (message) {
        // do something
    };

    agent.client.addCTIMakeCallRequest = function (response) {
        addCTIMakeCallRequest(response);
    }

    agent.client.receiveAcceptCallRequest = function (response) {
        receiveAcceptCallRequest(response);
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        agent.server.getAllUserConnected();
        showListOfQueueCalls();
        $('#init-device-btn').click(function () {
            var deviceId = $('#deviceIdPhone').val();
            $('#modal-init-phone').modal('hide');
            if (notEmpty(deviceId)) {
                localStorage.setItem('deviceId', deviceId);
                spinnerShow();
                agent.server.initilizeSupervisorDevice($('#deviceIdPhone').val());
            } else {
                errorNoty('El dispositivo no debe ser vacio');
            }
        });

        $('#get-agent-list').click(function () {
            agent.server.getAllUserConnected();
        });

        $('#get-queue-call-list').click(function () {
            showListOfQueueCalls();
        });

        $('#LogOutCore').click(function () {
            var deviceId = localStorage.getItem('deviceId');
            if (notEmpty(deviceId)) {
                localStorage.removeItem('deviceId');
            }
            $('#LogOutForm').submit();
        });
    });
});
