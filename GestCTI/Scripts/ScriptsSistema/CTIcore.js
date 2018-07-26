window.AvayaWebSocket = function (ip, port) {
    this.connected = false;
    this.sendDisconnect = true;
    this.ip = ip;
    this.port = port;
    this.cdeArray = new Array();
    this.socket = null;
    this.SentRequests = {};
}

var HEARTBEAT_INTERVAL = 5000;

/*** PRIVATE FUNCTIONS ***/
AvayaWebSocket.prototype.connect = function () {
    try {
        this.socket = new WebSocket("ws://" + this.ip + ":" + this.port + "/");
        var me = this;
        this.socket.onopen = function (e) { me.onConnect(e); };
        this.socket.onmessage = function (e) { me.onReceive(e); };
        this.socket.onclose = function (e) { me.onDisconnect(e); };
        this.socket.onerror = function (e) { me.onError(e); };
    } catch (e) {
        alert("Error on connection. Please try later : " + e.message);
    }
}

AvayaWebSocket.prototype.onConnect = function (e) {
    //ClearTimeOut
    alert('onconnect');
    this.connected = true;
    var me = this.StartHeartbeat();
    return me;
}

AvayaWebSocket.prototype.onReceive = function (e) {
    var data;
    alert('llego mensaje');
    var me = this;
    try {
        data = JSON.parse(e.data);
    } catch (e) {
        return;
    }

    if (data.request) {
        ManageEvent(me, data);
    } else {
        ManageRequestResponse(me, data);
    }
}

AvayaWebSocket.prototype.onDisconnect = function (e) {
    this.connected = false;
    alert('The connection with the core has finished!');
}

AvayaWebSocket.prototype.onError = function (e) {
    alert('The connection with the core has failed, try it later.');
}

AvayaWebSocket.prototype.StartHeartbeat = function () {
    var me = this;
    this.heartbeat = setInterval(function () {
        CTIrequest(me, { request: "CTIHeartbeatRequest" });
    }, HEARTBEAT_INTERVAL);
    return this;
}

AvayaWebSocket.prototype.InitializeDevice = function (deviceId) {
    CTIrequest(this, { request: "Initialize", args: [deviceId] });
}

function CTIrequest(me, data) {
    alert('CTIrequest ' + me.connected + ' request: ' + data.request);
    if (!me.connected) {
        throw new Error('You must be connected to send request to the core.');
    }
    var deferred = Q.defer();
    var invokedId = GetRandomId();
    me.SentRequests[invokedId] = deferred;

    data.invokedId = invokedId;
    data.args = data.args || [];
    me.socket.send(JSON.stringify(data));
    alert('Request enviado');
    return deferred.promise;
}

function ManageEvent(AvayaWebSocket, data) {
    var eventName = eventDictionary[data.request.request];
    var eventArgs = data.request.args;

    alert('ManageEvent() -> EventName: ' + data.request.request);

    var call, ivr, eventInfo;
    switch (eventName) {
        case 'callStarted':
            //eventInfo = {
            //    status: 'started',
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    appName: eventArgs[3]
            //};
            //call = upsertCall(core, eventInfo);
            //call.emit('started');
            //call.device.emit(eventName, call);
            //core.emit(eventName, call, call.device);

            alert('case callStarted');
            break;
        case 'callOriginated':
            //eventInfo = {
            //    status: 'originated',
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    callingDeviceId: eventArgs[3],
            //    calledDeviceId: eventArgs[4],
            //    appName: eventArgs[5]
            //};
            //call = upsertCall(core, eventInfo);
            //call.emit('originated');
            //call.device.emit(eventName, call);
            //core.emit(eventName, call);

            alert('case callOriginated');
            break;
        case 'callDelivered':
            //eventInfo = {
            //    status: 'delivered',
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    alertingDeviceId: eventArgs[3],
            //    callingDeviceId: eventArgs[4],
            //    calledDeviceId: eventArgs[5],
            //    trunkGroupId: eventArgs[6],
            //    trunkMember: eventArgs[7],
            //    splitDeviceId: eventArgs[8],
            //    lastRedirectionDeviceId: eventArgs[9],
            //    callerId: eventArgs[10],
            //    appName: eventArgs[11]
            //};
            //call = upsertCall(core, eventInfo);
            //call.emit('delivered');
            //call.device.emit(eventName, call);
            //core.emit(eventName, call);

            alert('case callDelivered');
            break;
        case 'callDiverted':
            //eventInfo = {
            //    status: 'diverted',
            //    active: false,
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    undocumentedProperty: eventArgs[3],
            //    appName: eventArgs[4]
            //};
            //call = upsertCall(core, eventInfo);
            //call.emit('diverted');
            //call.device.emit(eventName, call);
            //core.emit(eventName, call);

            alert('case callDiverted');
            break;
        case 'callEstablished':
            //eventInfo = {
            //    status: 'established',
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    answeringDeviceId: eventArgs[3],
            //    callingDeviceId: eventArgs[4],
            //    calledDeviceId: eventArgs[5],
            //    trunkGroupId: eventArgs[6],
            //    trunkMember: eventArgs[7],
            //    splitDeviceId: eventArgs[8],
            //    cause: eventArgs[9],
            //    lastRedirectionDeviceId: eventArgs[10],
            //    callerId: eventArgs[11],
            //    appName: eventArgs[12]
            //};
            //call = upsertCall(core, eventInfo);
            //call.emit('established');
            //call.device.emit(eventName, call);
            //core.emit(eventName, call);

            alert('case callEstablished');
            break;
        case 'callFailed':
            //eventInfo = {
            //    status: 'failed',
            //    active: false,
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    alertingDeviceId: eventArgs[3],
            //    callingDeviceId: eventArgs[4],
            //    calledDeviceId: eventArgs[5],
            //    cause: eventArgs[6],
            //    appName: eventArgs[7]
            //};
            //call = upsertCall(core, eventInfo);
            //call.emit('failed');
            //call.device.emit(eventName, call);
            //core.emit(eventName, call);

            alert('case callFailed');
            break;
        case 'callHeld':
            //eventInfo = {
            //    status: 'held',
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    appName: eventArgs[3]
            //};
            //call = upsertCall(core, eventInfo);
            //call.emit('held');
            //call.device.emit(eventName, call);
            //core.emit(eventName, call);

            alert('case callHeld');
            break;
        case 'partyHeld':
            //eventInfo = {
            //    status: 'partyHeld',
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    appName: eventArgs[3]
            //};

            //call = core.findCall({
            //    ucid: eventInfo.ucid
            //});

            //if (call) {
            //    call.emit(eventName, eventInfo);
            //    call.device.emit(eventName, call, eventInfo);
            //}

            //core.emit(eventName, eventInfo, call);

            alert('case partyHeld');
            break;
        case 'callResumed':
            //eventInfo = {
            //    status: 'resumed',
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    appName: eventArgs[3]
            //};
            //call = upsertCall(core, eventInfo);
            //call.emit('resumed');
            //call.device.emit(eventName, call);
            //core.emit(eventName, call);

            alert('case callResumed');
            break;
        case 'partyResumed':
            //eventInfo = {
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    appName: eventArgs[3]
            //};
            //call = core.findCall({
            //    ucid: eventInfo.ucid
            //});

            //if (call) {
            //    call.emit(eventName, eventInfo);
            //    call.device.emit(eventName, call, eventInfo);
            //}

            //core.emit(eventName, eventInfo, call);

            alert('case partyResumed');
            break;
        case 'callDisconnection':
            //eventInfo = {
            //    status: 'disconnected',
            //    active: false,
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    releasingDeviceId: eventArgs[3],
            //    appName: eventArgs[4]
            //};
            //call = upsertCall(core, eventInfo);
            //_.remove(call.device.calls, call);
            //call.emit('disconnection');
            //call.device.emit(eventName, call);
            //core.emit(eventName, call);

            alert('case callDisconnection');
            break;
        case 'partyDisconnection':
            //eventInfo = {
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    releasingDeviceId: eventArgs[3],
            //    appName: eventArgs[4]
            //};
            //call = core.findCall({
            //    ucid: eventInfo.ucid
            //});

            //if (call) {
            //    call.emit(eventName, eventInfo);
            //    call.device.emit(eventName, call, eventInfo);
            //}

            //core.emit(eventName, eventInfo, call);

            alert('case partyDisconnection');
            break;
        case 'callEnded':
            //eventInfo = {
            //    status: 'done',
            //    active: false,
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    appName: eventArgs[2]
            //};
            //call = upsertCall(core, eventInfo);
            //if (call) {
            //    call.emit('ended');
            //    call.device.emit(eventName, call);
            //    core.emit(eventName, call);
            //}

            alert('case callEnded');
            break;
        case 'callTransferred':
            //eventInfo = {
            //    status: 'transferred',
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    activeUcid: eventArgs[2],
            //    activeCallId: eventArgs[3],
            //    resultUcid: eventArgs[4],
            //    transferringDeviceId: eventArgs[5],
            //    transferredDeviceId: eventArgs[6],
            //    appName: eventArgs[7]
            //};
            //var transferResult = upsertTransferredCall(core, eventInfo);
            //call = transferResult.call;
            //transferResult.heldCall.emit('transferred', call);
            //transferResult.activeCall.emit('transferred', call);
            //transferResult.heldCall.device.emit(eventName, call, transferResult.heldCall, transferResult.activeCall);
            //transferResult.activeCall.device.emit(eventName, call, transferResult.heldCall, transferResult.activeCall);
            //core.emit(eventName, call, transferResult.heldCall, transferResult.activeCall);

            alert('case callTransferred');
            break;
        case 'callConferenced':
            //eventInfo = {
            //    status: 'conferenced',
            //    ucid: eventArgs[0],
            //    callId: eventArgs[1],
            //    activeUcid: eventArgs[2],
            //    activeCallId: eventArgs[3],
            //    resultUcid: eventArgs[4],
            //    controllerDeviceId: eventArgs[5],
            //    addedDeviceId: eventArgs[6],
            //    appName: eventArgs[7]
            //};
            //var conferenceResult = upsertConferencedCall(core, eventInfo);
            //call = conferenceResult.call;
            //conferenceResult.heldCall.emit('conferenced', call);
            //conferenceResult.activeCall.emit('conferenced', call);
            //conferenceResult.heldCall.device.emit(eventName, call, conferenceResult.heldCall, conferenceResult.activeCall);
            //conferenceResult.activeCall.device.emit(eventName, call, conferenceResult.heldCall, conferenceResult.activeCall);
            //core.emit(eventName, call, conferenceResult.heldCall, conferenceResult.activeCall);

            alert('case callConferenced');
            break;
        case 'agentStateChanged':
            //eventInfo = {
            //    agentId: eventArgs[0],
            //    state: eventArgs[1],
            //    workMode: eventArgs[2],
            //    talkState: eventArgs[3],
            //    reason: eventArgs[4],
            //    notify: eventArgs[5],
            //    associatedDeviceId: eventArgs[6],
            //    appName: eventArgs[7]
            //};
            //var agent = upsertAgent(core, eventInfo);
            //agent.emit('stateChanged');
            //agent.device.emit(eventName, agent);
            //core.emit(eventName, agent);

            alert('case agentStateChanged');
            break;
        case 'recordingStarted':
            //eventInfo = {
            //    ivrId: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    identifier: eventArgs[3],
            //    appName: eventArgs[4]
            //};
            //core.emit(eventName, ivr, eventInfo.identifier);

            alert('case recordingStarted');
            break;
        case 'recordingEnded':
            //eventInfo = {
            //    ivrId: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    identifier: eventArgs[3],
            //    appName: eventArgs[4]
            //};
            //core.emit(eventName, ivr, eventInfo.identifier);

            alert('case recordingEnded');
            break;
        case 'dtmf':
            //eventInfo = {
            //    ivrId: eventArgs[0],
            //    callId: eventArgs[1],
            //    deviceId: eventArgs[2],
            //    value: eventArgs[3],
            //    appName: eventArgs[4]
            //};
            //core.emit(eventName, ivr, eventInfo.value);

            alert('case dtmf');
            break;
        default:
            break;
    }

}

function ManageRequestResponse(me, response) {
    alert('ManageRequestResponse() conn: ' + me.connected);
    var deferred = me.SentRequests[response.invokedId];
    if (deferred) {
        delete me.SentRequests[response.invokedId];
        if (response.success) {
            alert('ManageRequestResponse() -> Success -> Manejar respuesta');
            deferred.resolve(response.result);
        } else {
            alert('ManageRequestResponse() -> Notificar error ' + (response.reason || 'Unknown Core Error'));
            deferred.reject(new Error(response.reason || 'Unknown Core Error'));
        }
    }
}

var eventDictionary = {
    onCallStarted: 'callStarted',
    onCallOriginated: 'callOriginated',
    onCallDelivered: 'callDelivered',
    onCallDiverted: 'callDiverted',
    onCallFailed: 'callFailed',
    onEstablishedConnection: 'callEstablished',
    onHoldConnection: 'callHeld',
    onHoldPartyConnection: 'partyHeld',
    onResumeConnection: 'callResumed',
    onRetrievePartyConnection: 'partyResumed',
    onEndConnection: 'callDisconnection',
    onEndPartyConnection: 'partyDisconnection',
    onEndCall: 'callEnded',
    onTransferredCall: 'callTransferred',
    onConferencedCall: 'callConferenced',
    onAgentChangedState: 'agentStateChanged',
    onRecordingStartedPlaying: 'recordingStarted',
    onRecordingEndedPlaying: 'recordingEnded',
    onCollectedDigits: 'dtmf'
};

function s4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

function GetRandomId() {
    return (s4() + s4() + "-" + s4() + "-4" + s4().substr(0, 3) + "-" + s4() + "-" + s4() + s4() + s4()).toLowerCase();
}