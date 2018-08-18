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

$(function () {
    var phoneExtension = localStorage.getItem('deviceId');
    if (notEmpty(phoneExtension)) {
        showPhoneView();
    }

    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;

    agent.client.Notification = function (response) {
    }

    agent.client.sendUserConnected = function (CtiAgentList) {
        // Show list of agents
    }

    // Logout from web app
    agent.client.logOutCore = function (response) {
        json = JSON.parse(response);
        if (json['success'] === true) {
            $('#LogOutForm').submit();
        } else {
            // Notificar error
        }
    }

    agent.client.addInitialize = function (message) {
        json = JSON.parse(message);
        if (json['success'] === true) {
            successNoty('Se ha inicializado el dispositivo correctamente');
            showPhoneView();
        } else {
            localStorage.removeItem('deviceId');
            errorNoty('No se ha inicializado el dispositivo. Razones ' + json('reason'));
        }
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#init-device-btn').click(function () {
            var deviceId = $('#deviceIdPhone');
            if (notEmpty(deviceId)) {
                localStorage.setItem('deviceId', deviceId);
                agent.server.initilizeSupervisorDevice($('#deviceIdPhone').val());
            } else {
                errorNoty('El dispositivo no debe ser vacio');
            }
        });

        $('#LogOutCore').click(function () {
            var deviceId = localStorage.getItem('deviceId');
            if (notEmpty(deviceId)) {
                agent.server.sendLogOutCore(deviceId);
            } else {
                $('#LogOutForm').submit();
            }
        });
    });
});
