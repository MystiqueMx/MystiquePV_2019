$(document).ready(function () {
    //ACTUALIZAR RECIBIDO
    $('.botton-recibido').on('click', function (evento) {
        var $contenedor = $(evento.target).parent('.control-recibido');
        var id = $contenedor.find('input[name = "id"]').val();
        var valor = $contenedor.find('input[name = "indice"]').val();
        var parametro = { id: id, Total: valor };
        console.log(parametro);
        $.ajax({
            type: "POST",
            url: $('#ActualizarTotal').val(),
            dataType: 'json',
            data: parametro
        }).done(function (response) {
            if (response != null && response.exito) {
                try {
                    window.location.reload();
                }
                catch (err) {
                    console.error(err);
                }
            }
            else {
                alert("Ocurrio un error, contacte a su administrador.");
            }
        });
    });
    //ACTUALIZAR OBSERVACION
    $('.botton-observacion').on('click', function (evento) {
        var $contenedor = $(evento.target).parent('.control-observacion');
        var id = $contenedor.find('input[name = "id"]').val();
        var valor = $contenedor.find('textarea[name = "indice"]').val();
        var parametro = { id: id, Observacion: valor };
        console.log(parametro);
        $.ajax({
            type: "POST",
            url: $('#ActualizarObservacion').val(),
            dataType: 'json',
            data: parametro
        }).done(function (response) {
            if (response != null && response.exito) {
                try {
                    window.location.reload();
                }
                catch (err) {
                    console.error(err);
                }
            }
            else {
                alert("Ocurrio un error, contacte a su administrador.");
            }
        });
    });
    //CERRAR ARQUEO:
    $("#modal-cerrar-arqueo").on("show.bs.modal", OnModalCerrarArqueoOpen);

});

function OnModalCerrarArqueoOpen(event) {

    var $sender = $(event.relatedTarget);
    var idArqueo = $sender.data("id");
    var totalRecibido = $sender.data("total");
    var observacion = $sender.data("observacion");
    var concepto = $sender.data("concepto");
    var fecha = $sender.data("fecha");

    var $modal = $("#modal-cerrar-arqueo");

    $modal.find('#titulo-modal').text(fecha);
    $modal.find('#concepto-modal').text(concepto);
    $modal.find('#id').val(idArqueo);
    $modal.find('#totalRecibido').val(totalRecibido);
    $modal.find('#observacion').val(observacion);

}