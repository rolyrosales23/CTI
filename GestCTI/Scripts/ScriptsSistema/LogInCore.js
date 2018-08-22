$(function () {
    // Delete localStorage Story
    localStorage.clear();
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;
    // Function to get response for login
    agent.client.logInAgent = function (message) {
        json = JSON.parse(message);
        var phone = $("#LoginPhoneExtension").val();
        var user = $("#LoginUsername").val();
        if (json['success'] === true) {
            if (phone === "") {
                spinnerHide();
                errorNoty(Resources.NotphoneError);
            } else {
                agent.server.sendInitialize(phone, user);
            }
        } else {
            localStorage.setItem('error', json.result);
            if (phone !== "") {
                // Check in if loggued
                agent.server.sendCTIGetAgentInfo(user);
            } else {
                // stop spinner and send message error
                spinnerHide();
                errorNoty(Resources.RequireDevice);
            }
        }
    };

    agent.client.errorCoreConnection = function (message) {
        spinnerHide();
        errorNoty(Resources.NotConnectCore);
    };

    agent.client.getAgentInfo = function (message) {
        json = JSON.parse(message);
        var user = $("#LoginUsername").val();
        var phone = $("#LoginPhoneExtension").val();
        var result_str = json['result'];
        var result = JSON.parse(result_str);
        var deviceId = result[0].AssociatedDeviceId;
        if (json['success'] === true && phone === deviceId && deviceId !== "") {
            agent.server.sendInitialize(phone, user);
        } else if (phone !== deviceId && deviceId !== "") {
            // Show login problem: Loggued with different deviceId
            // Do you want to lockout and sing in
            spinnerHide();
            localStorage.removeItem('error');
            errorNoty(Resources.LoggedOtherDevice + " " + deviceId);
        } else {
            /*var AS_LOGGED_OUT = 1;
            var State = result[0]['State'];
            if (State !== AS_LOGGED_OUT) {
                agent.server.sendLogOutCore(deviceId);
                spinnerHide();
                errorNoty('El sistema ha actualizado el estado del usuario ' + user + '. Intente nuevamente.');
            }
            else {*/
                // Show error
                spinnerHide();
                var error = localStorage.getItem('error');
                localStorage.removeItem('error');
                errorNoty('Error ' + error);
           // }
        }
    };

    agent.client.addInitialize = function (message) {
        json = JSON.parse(message);
        if (json['success'] === true) {
            console.log("Login Core sucess");
            // Save deviceId
            localStorage.setItem('deviceId', $("#LoginPhoneExtension").val());
            // agent.server.stop();
            $("#LogInForm").submit().done(function () {
                spinnerHide();
            });
        } else {
            spinnerHide();
            // send message error
            errorNoty(Resources.CanNotInitialize + " " + $("#LoginPhoneExtension").val());
        }
    };

    function runRoleAgent() {
        var phone = $("#LoginPhoneExtension").val();
        if (notEmpty(phone)) {
            agent.server.sendLogInAgent(phone, $("#LoginUsername").val(), $("#LoginPassword").val());
        } else {
            spinnerHide();
            errorNoty("Debes autenticarte con un dispositivo");
        }
    }

    function runRoleSupervisor() {
        var phone = $("#LoginPhoneExtension").val();
        var user = $("#LoginUsername").val()
        if (notEmpty(phone) && notEmpty(user)) {
            agent.server.initilizeSupervisorDevice(phone, user);
        } else {
            $("#LogInForm").submit().done(function () {
                spinnerHide();
            });
        }
    }

    function runRoleAdmin() {
        $("#LogInForm").submit().done(function () {
            spinnerHide();
        });
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#LogInCore').click(function () {
            var UserName = $("#LoginUsername").val();
            if (!notEmpty(UserName)) {
                errorNoty("Debe introducir un usuario");
                return;
            }

            spinnerShow();
            $.ajax({
                'url': path_get_roles,
                'type': 'POST',
                'data': { 'username': UserName },
                'success': function (role) {
                    switch (role) {
                        case "admin":
                            runRoleAdmin();
                            break;
                        case "supervisor":
                            runRoleSupervisor();
                            break;
                        case "agent":
                            runRoleAgent();
                            break;
                        default:
                            spinnerHide();
                            errorNoty("El rol del usuario no es válido");
                    }
                },
                'error': function () {
                    spinnerHide();
                    errorNoty("No se pudo obtener el rol del usuario");
                }
            });
        });
    });
});
