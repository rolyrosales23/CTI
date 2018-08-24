﻿function pintarAgentList(agents, selector) {
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
            .append('<th width="100">actions</th>').end();

        var body = $('<tbody></tbody>');

        for (var i in agents) {
            var fila = $('<tr></tr>')
                .append('<td>' + agents[i].user_name + '</td>')
                .append('<td>' + agents[i].user_name + '</td>')
                .append('<td>' + agents[i].user_name + '</td>')
                .append('<td>' + agents[i].user_name + '</td>');
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
            var waitTime = (new Date(queueCalls[i].QueuedTime)) - (new Date(queueCalls[i].Time));
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

function showPhoneView() {
    $.ajax({
        url: 'GetTelephone',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html'
    }).then(function (result) {
        $("#showPhone").html(result);
    }).fail(function (xhr, status) {
        errorNoty("Error charging phone partial");
    })
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
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;

    agent.client.listOfAgent = function (agents) {
        pintarAgentList(agents, "#table-agents-connected");
        var panel = $('#get-agent-list').parents(".panel:last");
        if (panel.hasClass("panel-refreshing")) {
            panel_refresh(panel);
        }
    }

    agent.client.Notification = function (response, type = "success") {
        notify(response, type);
    }

    agent.client.sendUserConnected = function (CtiAgentList) {
        // Show list of agents
    }

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
