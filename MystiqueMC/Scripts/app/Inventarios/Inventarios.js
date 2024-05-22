$(document).ready(function () {
    //ACTUALIZAR MAXIMO
    $('.botton-aa').on('click', function (evento) {
        var $contenedor = $(evento.target).parent('.control-orden');
        var id = $contenedor.find('input[name = "id"]').val();
        var valor = $contenedor.find('input[name = "indice"]').val();
        var parametro = { id: id, Valor: valor, campo: 1 };

        var minimo = $('#txtMinimo').val();
        if (parseInt(minimo) <= parseInt(valor)) {
            $.ajax({
                type: "POST",
                url: $('#CambiarValor').val(),
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
        } else {
            //mostrar mensaje
            alert("El valor maximo debe ser mayor o igual que el valor minimo.");
        }
    });
    //ACTUALIZAR MINIMO
    $('.botton-ab').on('click', function (evento) {
        var $contenedor = $(evento.target).parent('.control-orden-b');
        var id = $contenedor.find('input[name = "id"]').val();
        var valor = $contenedor.find('input[name = "indice"]').val();
        var parametro = { id: id, Valor: valor, campo: 2 };

        var maximo = $('#txtMaximo').val();
        if (parseInt(maximo) >= parseInt(valor)) {
            $.ajax({
                type: "POST",
                url: $('#CambiarValor').val(),
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
        } else {
            //mostrar mensaje
            alert("El valor minimo debe ser menor o igual que el valor maximo.");
        }
    });
    //ACTUALIZAR PRECIO
    $('.botton-ac').on('click', function (evento) {
        var $contenedor = $(evento.target).parent('.control-orden-c');
        var id = $contenedor.find('input[name = "id"]').val();
        var valor = $contenedor.find('input[name = "indice"]').val();
        var parametro = { id: id, Valor: valor, campo: 3 };
        console.log(parametro);
        $.ajax({
            type: "POST",
            url: $('#CambiarValor').val(),
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