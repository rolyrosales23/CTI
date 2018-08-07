$(function () {
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;
    // Function to get response for login
    agent.client.logInAgent = function (message) {
        json = JSON.parse(message);
        if (json['success'] === true) {
            var phone = $("#LoginPhoneExtension").val();
            if (phone === "") {
                console.error("Not phone specify");
                spinnerHide();
            } else {
                agent.server.sendInitialize(phone);
            }
        } else {
            // stop spinner and send message error
            console.error("Error");
            spinnerHide();
        }
    }

    agent.client.addInitialize = function (message) {
        json = JSON.parse(message);
        spinnerHide();
        if (json['success'] === true) {
            console.log("Login Core sucess");
            localStorage.setItem('deviceId', $("#LoginPhoneExtension").val());
            $("#LogInForm").submit();
        } else {
            // send message error
            console.error("Error");
        }
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#LogInCore').click(function () {
            spinnerShow();
            var phone = $("#LoginPhoneExtension").val();
            if (phone !== null && phone !== undefined && phone !== "") {
                agent.server.sendLogInAgent(phone, $("#LoginUsername").val(), $("#LoginPassword").val());
            } else
            {
                spinnerHide();
                $("#LogInForm").submit();
            }
        });
    });
});
