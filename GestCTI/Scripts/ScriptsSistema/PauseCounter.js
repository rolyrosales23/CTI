function startCounter(container, duration) {
    var x = setInterval(function () {
        // Time calculations for days, hours, minutes and seconds
        var days = Math.floor(duration / (60 * 60 * 24));
        var hours = Math.floor((duration % (60 * 60 * 24)) / (60 * 60));
        var minutes = Math.floor((duration % (60 * 60)) / 60);
        var seconds = Math.floor(duration % 60);

        // Display the result
        var time = "";
        if (days > 0)
            time = days + "d " + hours + "h " + minutes + "m " + seconds + "s";
        else if (hours > 0)
            time = hours + "h " + minutes + "m " + seconds + "s";
        else if (minutes > 0)
            time = minutes + "m " + seconds + "s";
        else
            time = seconds + "s";

        $(container + " span").html(time);
        duration--;
       
        if (duration < 0)
            clearInterval(x);

        $(container).css('display', 'block');
        localStorage.setItem('pause_interval_id', x);
    }, 1000);
}

function stopCounter(container) {
    clearInterval(localStorage.getItem('pause_interval_id'));
    localStorage.removeItem('pause_interval_id');
    $(container).css('display', 'none');
}

