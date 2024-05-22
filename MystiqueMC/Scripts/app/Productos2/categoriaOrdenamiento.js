$(document).ready(function () {
    $('.botton-ac').on('click', function (evento) {
        var $contenedor = $(evento.target).parent('.control-orden');
        var id = $contenedor.find('input[name = "id"]').val();
        var orden = $contenedor.find('input[name = "indice"]').val();
        var parametro = { id: id, ordenamiento: orden };
        console.log(parametro);
        $.ajax({
            type: "POST",
            url: $('#ordenObtenido').val(),
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
});