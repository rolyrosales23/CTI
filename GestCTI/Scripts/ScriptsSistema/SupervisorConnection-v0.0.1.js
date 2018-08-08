$(function () {
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;

    agent.client.Notification = function (response) {
        noty(
            {
                text: response,
                layout: 'topRight',
                type: 'success',
                maxVisible: 5,
                animation: {
                    open: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceInLeft'
                    close: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceOutLeft'
                    easing: 'swing',
                    speed: 500 // opening & closing animation speed
                },
                timeout: 2000
            });
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
