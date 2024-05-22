var ESTATUS_DATOS = {
    Vacio: 0,
    Catalogo: 1,
    Nuevos: 2,
};

window.onload = function OnScriptsLoad() {
    $("#FacturacionIdEstado").on('change', onEstadoChange);
    $(".ibox-content").removeClass("sk-loading");
    //$("#row-estatus input").on("ifChanged", OnEstatusChanged);
    //if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
    //    $(".selectpicker").selectpicker("mobile");
    //}
    //$("#row-estatus input:checked")
    //    .trigger("ifChanged");
    //InitSelectEstados();
};

function onEstadoChange(event) {
    var idestado = $(FacturacionIdEstado).val();
    obtenerCiudad(idestado);
}

function obtenerCiudad(idestado) {
    $("#FacturacionIdCiudad").empty();
     var $selectCiudad = $("#FacturacionIdCiudad");
    $.ajax({
        type: "POST",
        url: $('#Url-Ciudades').val(),
        dataType: 'json',
        data: { id: idestado },
    }).done(function (response) {
        if (response != null) {
            if (response.any) {
                updateSelect($selectCiudad, response.data);
            } else {
                SendAlert("No existen ciudades cargadas para este estado, por favor contacte a su administrador", "warning", true);
            }
        }
        else {
            alert("Ocurrio un error al obtener las ciudades, contacte a su administrador.");
        }
    });
}

function resetSelect(target) {
    target.empty();
    target.append(`<option value="">Seleccione...</option>`);
    target.prop("disabled", true);
    target.selectpicker("refresh");
}
function updateSelect(target, options) {
    target.append(
        options.map(c =>
            `<option value="${c.Value}" ${c.Selected ? "selected" : ""}>${c.Text}</option>`
        ));
    target.prop("disabled", false);
    target.selectpicker("refresh");
}
function notifyCiudadesFailed() {
    SendAlert("Ocurrió un error al obtener las ciudades, por favor intente de nuevo", "error", false);
}


//function OnEstadoChanged() {
//    var url = $("#Url-Ciudades").val();
//    var $selectCiudad = $("#FacturacionIdCiudad");
//    var payload = {
//        estado: parseInt($(this).val())
//    };
//    if (window.CiudadSeleccionada) {
//        payload.seleccion = window.CiudadSeleccionada;
//        window.CiudadSeleccionada = null;
//    }
//    resetSelect($selectCiudad);
//    if (Number.isNaN(payload.estado)) return;

//    $(".ibox-content").addClass("sk-loading");
//    $.post(url, payload)
//        .done(function (res) {
//            if (res.any) {
//                updateSelect($selectCiudad, res.data);
//            } else {
//                SendAlert("No existen ciudades cargadas para este estado, por favor contacte a su administrador", "warning", true);
//            }
//        })
//        .fail(notifyCiudadesFailed)
//        .always(function () {
//            $(".ibox-content").removeClass("sk-loading");
//        });


//}

//function InitSelectEstados() {
//    $("#facturacionidestado").on("change", OnEstadoChanged);
//    if (!number.isnan(parseint($("#facturacionidestado").val()))
//        && !number.isnan(parseint($("#idciudadanterior").val()))) {
//        window.ciudadseleccionada = parseint($("#idciudadanterior").val());
//        $("#facturacionidestado").trigger("change");
//    }
//}

//function OnEstatusChanged() {
//    var estatus = parseInt($(this).val());
//    switch (estatus) {
//        case ESTATUS_DATOS.Catalogo:
//            $("#row-seleccion-catalogo").removeClass("hide");
//            $("#row-nuevos-datos-fiscales").addClass("hide");
//            break;
//        case ESTATUS_DATOS.Nuevos:
//            $("#row-seleccion-catalogo").addClass("hide");
//            $("#row-nuevos-datos-fiscales").removeClass("hide");
//            break;
//        default:
//            $("#row-seleccion-catalogo").addClass("hide");
//            $("#row-nuevos-datos-fiscales").removeClass("hide");
//            break;
//        //default:
//        //    $("#row-seleccion-catalogo").addClass("hide");
//        //    $("#row-nuevos-datos-fiscales").addClass("hide");
//        //    break;
//    }
//}