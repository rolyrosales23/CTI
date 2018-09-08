function merge(defaults, options) {
    if (options !== null)
        for (var property in options)
            defaults[property] = options[property];

    return defaults;
}

function notyBase(message, options = null) {
    var defaults = {
        text: message,
        layout: 'topRight',
        type: 'success',
        maxVisible: 5,
        animation: {
            open: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceInLeft'
            close: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceOutLeft'
            easing: 'swing',
            speed: 500 // opening & closing animation speed
        },
        timeout: 3000
    };

    noty( merge(defaults, options) );
}

function tempNoty(message, options = null) {
    if (!IsDebugMode)
        return;

    notyBase(message, merge({
        layout: 'topLeft',
        timeout: null,
        maxVisible: 20
    }, options));
}

function successNoty(message, MsgDebugMode = true, options = null) {
    if (!IsDebugMode && MsgDebugMode) {
        return;
    }
    notyBase(message, options);
}

function errorNoty(message, MsgDebugMode = true, options = null) {
    if (!IsDebugMode && MsgDebugMode) {
        return;
    }
    notyBase(message, merge({
        type: 'error',
        timeout: null
    }, options));
}

function warningNoty(message, MsgDebugMode = true, options = null) {
    if (!IsDebugMode && MsgDebugMode) {
        return;
    }
    notyBase(message, merge({
        type: 'warning',
        timeout: 4000
    }, options));
}

function infoNoty(message, MsgDebugMode = true, options = null) {
    if (!IsDebugMode && MsgDebugMode) {
        return;
    }
    notyBase(message, merge({
        type: 'information',
        timeout: 5000
    }, options));
}

function notify(message, type = 'success', MsgDebugMode = true) {
    if (!IsDebugMode && MsgDebugMode) {
        return;
    }
    switch (type) {
        case 'success': successNoty(message); break;
        case 'info': infoNoty(message); break;
        case 'error': errorNoty(message); break;
        case 'warning': warningNoty(message); break;
        default: successNoty(message); break;
    }
}