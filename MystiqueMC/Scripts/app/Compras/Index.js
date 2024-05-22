function OnScriptsLoad() {
    $(".trigger-detalle").on("click", ev => VerDetalle($(ev.currentTarget).data("id")));
}

function VerDetalle(id) {
    InitLoader();
    const payload = { id };
    const url = $("#url-detalle-compra").val();
    $.post(url, payload)
        .done(res => ShowModalDetalle(res.result))
        .fail(handleFailedRequest)
        .always(ShutDownLoader);

    function handleFailedRequest() {
        SendAlert("Ha ocurrido un error al consultar el detalle de la compra, por favor contacte a su administrador", "error", true);
    }
}
function ShowModalDetalle(compra) {
    var tituloModal = "";
    $("#table-detalle-compra tbody").empty();
    //$("#modal-detalle-compra .modal-title").text(`Compra a ${compra.proveedor}`);
    tituloModal = 'Sucursal ' + compra.sucursal + " Proveedor " + compra.proveedor
        + (compra.remision && compra.remision !== null ? " Remisión " + compra.remision : "")
        + (compra.factura && compra.factura !== null ? " Factura " + compra.factura : "")
        + (compra.fechaCompra && compra.fechaCompra !== null ? " Fecha " + compra.fechaCompra : "");
    $("#modal-detalle-compra .modal-title").text(tituloModal);
    $("#table-detalle-compra tbody").append(compra.detalle.map(c => `<tr><td>${c.insumo}</td><td class="text-right">${c.importe}</td></tr>`));
    $("#iva").text(compra.iva);
    $("#descuentos").text(compra.descuentos);
    $("#total").text(compra.total);
    if (compra.remision && compra.remision !== null) {
        $("#remision").text(`Remisión: ${compra.remision}`);
    } else {
        $("#remision").text("Sin remisión capturada");
    }

    if (compra.factura && compra.factura !== null) {
        $("#factura").text(`Factura: ${compra.factura}`);
    } else {
        $("#factura").text("Sin factura capturada");
    }

    if (compra.observacion && compra.observacion !== null) {
        $("#observacion").text(`Observaciones: ${compra.observacion}`);
    } else {
        $("#observacion").text("Sin observaciones");
    }
    
    $("#modal-detalle-compra").modal();
}