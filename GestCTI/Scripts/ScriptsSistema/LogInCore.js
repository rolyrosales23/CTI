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
            } else {
                agent.server.sendInitialize(phone);
            }
        } else {
            // stop spinner and send message error
            console.error("Error");
            console.log("Stop spinner");
        }
    }

    agent.client.addInitialize = function (message) {
        json = JSON.parse(message);
        console.log("Stop spinner");
        if (json['success'] === true) {
            console.log("Login Core sucess");
            $("#LogInForm").submit();
        } else {
            // send message error
            console.error("Error");
        }
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#LogInCore').click(function () {
            // run spinner
            var phone = $("#LoginPhoneExtension").val();
            if (phone !== null && phone !== undefined && phone !== "") {
                agent.server.sendLogInAgent(phone, $("#LoginUsername").val(), $("#LoginPassword").val());
            } else
            {
                $("#LogInForm").submit();
            }
        });
    });
});
