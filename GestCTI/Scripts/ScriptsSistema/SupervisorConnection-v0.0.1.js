﻿$(function () {
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

    // Start the connection.
    $.connection.hub.start().done(function () {
        var deviceId = localStorage.getItem('deviceId');

        $('#LogOutCore').click(function () {
            var deviceId = localStorage.getItem('deviceId');
            agent.server.sendLogOutCore(deviceId);
        });
    });
});
