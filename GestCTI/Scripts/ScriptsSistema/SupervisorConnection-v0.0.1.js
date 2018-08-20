function pintarAgentList(agents, selector) {
    var wrapper = $(selector);
    wrapper.find('*').remove();
    if (notEmpty(agents)) {
        var table = wrapper.append('<div class="panel panel-default"></div>').find('div')
            .append('<div class="panel-body panel-body-table"></div>').find('div')
            .append('<div class="table-responsive"></div>').find('div')
            .append('<table class="table table-bordered table-striped table-actions"></table>').find('table');

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

function getAgents(connection) {
    agent.server.getAllUserConnected();
}

$(function () {
    var phoneExtension = localStorage.getItem('deviceId');
    if (notEmpty(phoneExtension)) {
        $('#right-side').removeClass('col-md-12').addClass('col-md-8');
        $("#loginExtension").remove();
        showPhoneView();
    }

    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;

    agent.client.listOfAgent = function (agents) {
        ///////////
        successNoty(agents);
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
            successNoty('Se ha inicializado el dispositivo correctamente');
            $('#right-side').removeClass('col-md-12').addClass('col-md-8');
            showPhoneView();
            $("#loginExtension").remove();
            spinnerHide();
        } else {
            localStorage.removeItem('deviceId');
            spinnerHide();
            errorNoty('No se ha inicializado el dispositivo. Razones ' + json('reason'));
        }
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
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

        $('#LogOutCore').click(function () {
            var deviceId = localStorage.getItem('deviceId');
            if (notEmpty(deviceId)) {
                localStorage.removeItem('deviceId');
            }
            $('#LogOutForm').submit();
        });
    });
});
