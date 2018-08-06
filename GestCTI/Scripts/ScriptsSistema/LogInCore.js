$(function () {
    // Reference the auto-generated proxy for the hub.
    var agent = $.connection.websocket;
    // Function to get response for login
    agent.client.LogInAgent = function (message) {
        // is success
        agent.server.sendInitialize("deviceId");
        // is false
        // stop spinner message error
    }

    agent.client.addInitialize = function (message) {
        // is success
        // authenticate wit web app
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#LogInCore').click(function () {
            // run spinner
            agent.server.sendLogInAgent(deviceId, ucid, passwor);
        });

    });
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}