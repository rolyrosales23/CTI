﻿$(function () {
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;
    // Function to get response for login
    agent.client.logInAgent = function (message) {
        json = JSON.parse(message);
        var phone = $("#LoginPhoneExtension").val();
        var user = $("#LoginUsername").val();
        if (json['success'] === true) {
            if (phone === "") {
                console.error("Not phone specify");
                spinnerHide();
            } else {
                agent.server.sendInitialize(phone, user);
            }
        } else {
            if (phone !== "") {
                // Check in if loggued
                agent.server.sendCTIGetAgentInfo(user);
            } else {
                console.error("Error");
                // stop spinner and send message error
                spinnerHide();
            }
        }
    }

    agent.client.getAgentInfo = function (message) {
        json = JSON.parse(message);
        var user = $("#LoginUsername").val();
        var phone = $("#LoginPhoneExtension").val();
        var tmp = json['result'];
        var tmp2 = JSON.parse(tmp);
        var deviceId = tmp2[0].AssociatedDeviceId;
        if (json['success'] === true && phone === deviceId && deviceId !== "") {
            agent.server.sendInitialize(phone, user);
        } else if (phone !== deviceId && deviceId !== "") {
            // Show login problem: Loggued with different deviceId
            // Do you want to lockout and sing in 
            spinnerHide();
        } else {
            // Show error
            spinnerHide();
        }
    }

    agent.client.addInitialize = function (message) {
        json = JSON.parse(message);
        if (json['success'] === true) {
            console.log("Login Core sucess");
            // Save deviceId
            localStorage.setItem('deviceId', $("#LoginPhoneExtension").val());
            $("#LogInForm").submit();
        } else {
            // send message error
            console.error("Error");
        }
        spinnerHide();
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#LogInCore').click(function () {
            spinnerShow();
            var phone = $("#LoginPhoneExtension").val();
            if (phone !== null && phone !== undefined && phone !== "") {
                agent.server.sendLogInAgent(phone, $("#LoginUsername").val(), $("#LoginPassword").val());
            } else {
                $("#LogInForm").submit();
                spinnerHide();
            }
        });
    });
});
