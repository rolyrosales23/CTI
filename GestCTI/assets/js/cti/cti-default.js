function notEmpty(value) {
    return value != undefined && value !== null && value != "";
}

function pintarListaEspera(lista) {
    var panel = $('#lista_espera');
    panel.find('ul').remove();

    if (notEmpty(lista)) {
        panel.append("<ul class='list-unstyled'></ul>");

        for (var i in lista) {
            var ind = Number(i) + 1;
            panel.find('ul').append("<li><a href='#' class='list-group-item form-group'><label class='check contacts-title'><input type='radio' class='icheckbox' name='hold_list' value='" + lista[i].ucid + "' />" + Resources.Llamada + " " + ind + "</label><p>" + Resources.Device + ": " + lista[i].toDevice + "</p></a></li>");
        }

        //reinicializar el plugin icheck
        if ($(".icheckbox").length > 0) {
            $(".icheckbox,.iradio").iCheck({ checkboxClass: 'icheckbox_minimal-grey', radioClass: 'iradio_minimal-grey' });
        }
    }
}

function loadTooltip() {
    $.each($('.info'), function (pos, obj) {
        var placement = 'bottom';
        if ($(obj).attr('data-placement') !== undefined) placement = $(obj).attr('data-placement');
        $(obj).tooltip({ placement: placement, html: true });
    });
}