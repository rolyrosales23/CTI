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
        $('#right-side').removeClass('col-md-12').addClass('col-md-7 col-md-offset-1');
        $("#loginExtension").remove();
        showPhoneView();
    }

    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;

    agent.client.Notification = function (response) {
    }

    agent.client.sendUserConnected = function (CtiAgentList) {
        // Show list of agents
    }

    agent.client.addInitialize = function (message) {
        json = JSON.parse(message);
        if (json['success'] === true) {
            successNoty('Se ha inicializado el dispositivo correctamente');
            $('#right-side').removeClass('col-md-12').addClass('col-md-7 col-md-offset-1');
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
