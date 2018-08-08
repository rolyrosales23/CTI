function successNoty(message) {
    noty({
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
    });
}

function errorNoty(message) {
    noty({
        text: message,
        layout: 'topRight',
        type: 'error',
        maxVisible: 5,
        animation: {
            open: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceInLeft'
            close: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceOutLeft'
            easing: 'swing',
            speed: 500 // opening & closing animation speed
        },
        //timeout: 2000
    });
}

function warningNoty(message) {
    noty({
        text: message,
        layout: 'topRight',
        type: 'warning',
        maxVisible: 5,
        animation: {
            open: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceInLeft'
            close: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceOutLeft'
            easing: 'swing',
            speed: 500 // opening & closing animation speed
        },
        timeout: 4000
    });
}

function infoNoty(message) {
    noty({
        text: message,
        layout: 'topRight',
        type: 'info',
        maxVisible: 5,
        animation: {
            open: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceInLeft'
            close: { height: 'toggle' }, // or Animate.css class names like: 'animated bounceOutLeft'
            easing: 'swing',
            speed: 500 // opening & closing animation speed
        },
        timeout: 5000
    });
}

function notify(message, type = 'success') {
    switch (type) {
        case 'success': successNoty(message); break;
        case 'info': infoNoty(message); break;
        case 'error': errorNoty(message); break;
        case 'warning': warningNoty(message); break;
        default: successNoty(message); break;
    }
}