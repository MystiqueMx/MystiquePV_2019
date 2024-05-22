function OnScriptsLoad() {
    $('#btn-marcar-leido').on('click', MarcarLeidos)
    $('#btn-marcar-importante').on('click', MarcarImportantes)
    $('#btn-marcar-eliminado').on('click', MarcarEliminados)
}
function ObtenerIdSeleccionados() {
    var $rowsMarcadas = $('.email-row').has('input[name="IsSelected"]:checked')
        .find('input[name="IdComentario"]');
    var Ids = []
    $rowsMarcadas.each(function (idx, el) {
        Ids.push($(el).val())
    })
    return Ids;
}

function MarcarEliminados() {
    var IdsSeleccionados = ObtenerIdSeleccionados();
    if (IdsSeleccionados.length > 0) {
        console.log(IdsSeleccionados);
        $.ajax({
            url: $('#url-marcar-eliminados').val(),
            method: "POST",
            data: { IdComentarios: IdsSeleccionados },
            dataType: "json",
        })
        .fail(SendFailAlert)
        .done(function (res) {
            if (res.success) {
                window.location.reload();
            } else {
                SendFailAlert();
            }
        });
    }
}

function MarcarImportantes() {
    var IdsSeleccionados = ObtenerIdSeleccionados();
    if (IdsSeleccionados.length > 0) {
        console.log(IdsSeleccionados);
        $.ajax({
            url: $('#url-marcar-importantes').val(),
            method: "POST",
            data: { IdComentarios: IdsSeleccionados },
            dataType: "json",
        })
            .fail(SendFailAlert)
            .done(function (res) {
                if (res.success) {
                    window.location.reload();
                } else {
                    SendFailAlert();
                }
            });
    }
}

function MarcarLeidos() {
    var IdsSeleccionados = ObtenerIdSeleccionados();
    if (IdsSeleccionados.length > 0) {
        console.log(IdsSeleccionados);
        $.ajax({
            url: $('#url-marcar-leidos').val(),
            method: "POST",
            data: { IdComentarios: IdsSeleccionados },
            dataType: "json",
        })
            .fail(SendFailAlert)
            .done(function (res) {
                if (res.success) {
                    window.location.reload();
                } else {
                    SendFailAlert();
                }
            });
    }
}

function SendFailAlert() {
    console.error('error al guardar los comentarios')
}