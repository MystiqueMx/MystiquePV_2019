function OnScriptsLoad() {

    var currentTime = new Date();

    $('#fechaCompra').datepicker({
        language: 'es',
        dateFormat: "yy/mm/dd",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+0",
        maxDate: 0,
        minDate: new Date(currentTime.getFullYear(), currentTime.getMonth() - 1, 1)
    });

    $('#calendarIcon').click(function () {
        $("#fechaCompra").focus();
    });


    //$("#modal-agregar-insumos").on("show.bs.modal", limpiarModalAgregarInsumos);
    $("#modal-confirmar").on("show.bs.modal", ModalConfirmarCompraOpen);
    //$("#btn-guardar-insumos").on("click", ClickAgregarInsumos);
    $("#btn-capturar-compra").on("click", ClickCierreCompra);
    $("#btn-guardar-compra").on("click", ClickConfirmarCompra);
    $("#btn-registro-compra").on("click", ClickGuardarCompra);
    $("select[name='sucursal']").on("changed.bs.select", UpdateTitle);
    $("input[name='sucursal']").on("change", UpdateTitle);
    $("select[name='proveedor']").on("changed.bs.select", UpdateTitle);
    $("input[name='proveedor']").on("change", UpdateTitle);
    $("#btn-confirmar-compra-anterior").on("click", ClickConfirmarCompraAnterior);
    $("#btn-cancelar-cierre").on("click", ClickCancelarCierreCompra);
    
    $("#insumo").on("change", SearchUnidadMinimaMedida);
    $("#importe").on("change", ReformatInput);
    $(".convert-to-currency").on("change", ReformatInput);
    ShowButtons();
    UpdateTitle();
    RedrawTableDetalle(window.DetalleCompra);
        
    $('#btnAgregarInsumo').on("click", ClickAgregarInsumos);
    $('#btnEditarInsumo').on("click", ClickEditarInsumos);
    $('#btnCancelarEdicion').on("click", limpiarModalInsumo);
   

    //$('#btn-Confirmar-Reapertura').click(function (event) {
    //    $('#modal-confirmar-reapertura').modal('show')
    //});

    $('#btn-reabrir-compra').on("click", ClickReabrirCompra);
    
    
    cargarSelectInsumos();
    $('#idDetalleCompra').val('0');

    compraSetReadOnly();      
}

function compraSetReadOnly() {

    //Poner la compra en read only si el status es 2 cerrado
   
    var disabled = false;
    if (window.estatusCompraId === 2) {                
        disabled = true;    

        $("#btnAgregarInsumo").prop("onclick", null).off("click");
    }

    $("#divAgregarInsumo").attr("disabled", disabled);
    $("#btnAgregarInsumo").attr("disabled", disabled);
    $("#divEditarInsumo").attr("disabled", disabled);    
    $("#divGuardarCompra :input").attr("disabled", disabled);
    $("#card-insumos :input").attr("disabled", disabled);
    $("#divCerrarCompra :input").attr("disabled", disabled);   

}

function ClickReabrirCompra() {
    InitLoader();

    const url = $("#url-reabrir-compra").val();
       
    $.post(url, { idCompra: window.Compra })
        .done(notifyResponse)
        .fail(notifyError)
        .always(() => ShutDownLoader());

    function notifyResponse() {
        SendAlert("Compra reabierta exitosamente.", "success", true);
        window.location = $("#url-back-reabrir-compra").val();
    }
    function notifyError(error) {
        SendAlert("Ha ocurrido un error al reabrir la compra, por favor contacte a su administrador", "error", true);
    }

}


function limpiarModalAgregarInsumos() {
    const sucursal = ObtenerSucursal();
    const proveedor = ObtenerProveedor();
    $("#modal-agregar-insumos .modal-title").text(`Agregar insumos - Sucursal: ${sucursal} - Proveedor: ${proveedor}`);
    $("#importe").val("");
    $("#cantidad").val("");
    $("#label-unidad-medida").val("-");
    $("#warning-compra").addClass("hide");
    //$("#btn-confirmar-compra-anterior").addClass("hide");
    //$("#btn-guardar-insumos").removeClass("hide");
    cargarSelectInsumos();

}
function cargarSelectInsumos() {
    const insumosFiltrados = window.CatalogoInsumos.filter(c => !DetalleCompra.some(d => d.insumo === c.Value));
    $("#insumo option").remove();
    $("#insumo").append(`<option data-desc="" value="">Seleccionar</option>`);
    $("#insumo").append(insumosFiltrados.map(c => `<option data-desc="${c.Text}" value="${c.Value}">${c.Text}</option>`));
    $("#insumo").selectpicker('val', '');
    $("#insumo").selectpicker('refresh');
}

function ObtenerSucursal() {
    let result = "-";
    if ($("select[name='sucursal']").length === 0) {
        var value = $("input[name='sucursal']:checked").val();
        result = $(`input[name='sucursal'][value='${value}']`).data('text');
    } else {
        result = $("select[name='sucursal'] option:selected").data('text');
    }
    return result;
}
function ObtenerProveedor() {
    let result = "-";
    if ($("select[name='proveedor']").length === 0) {
        var value = $("input[name='proveedor']:checked").val();
        result = $(`input[name='proveedor'][value='${value}']`).data('text');
    } else {
        result = $("select[name='proveedor'] option:selected").data('text');
    }
    return result;
}

function ModalConfirmarCompraOpen() {
    $("#descuentos").val("");
    $("#iva").val("");
    $("#total").val("");
    $("#form-confirmar").psly().reset();
}

function ClickAgregarInsumos() {
    if (!$('#form-agregar-insumo').psly().validate()) return;
    let hasErrors = false;
    const $row = $('#form-agregar-insumo');
    const $selectInsumos = $row.find(".selectpicker");
    const insumo = $selectInsumos.val();
    if (!insumo) return;
    const descripcion = $row.find(`select[name='insumo'] option[value='${insumo}']`).data('desc');
    const importe = StripFormat($row.find("input[name='importe']").val());
    const cantidad = StripFormat($row.find("input[name='cantidad']").val());
    const unidad = $("#label-unidad-medida").text();
    if (importe === "") {
        $(`#error-importe`).text("Este valor es requerido");
        hasErrors = true;
        return;
    }
    if (Number(importe) <= 0) {
        $(`#error-importe`).text("Este valor debe ser mayor a cero");
        hasErrors = true;
        return;
    }
    if (cantidad === "") {
        $(`#error-cantidad`).text("Este valor es requerido");
        hasErrors = true;
        return;
    }
    if (Number(cantidad) <= 0) {
        $(`#error-cantidad`).text("Este valor debe ser mayor a cero");
        hasErrors = true;
        return;
    }
    $(`#error-importe`).text("");

    ValidarInsumo({
        insumo: insumo,
        descInsumo: descripcion,
        importe: Number(importe),
        cantidad: Number(cantidad),
        unidad: unidad
    });
}

function RedrawTableDetalle(detalles) {
    $("#tabla-detalle-compra tbody").empty();
    $("#tabla-detalle-compra tbody").append(detalles.map((c, i) => `<tr>
            <td>
                ${c.desc || c.descInsumo}
            </td>
            <td>
                ${c.cantidad} ${c.unidad}
            </td>
            <td class="text-right">
                ${FormatAsCurrency(c.importe, true)}
            </td>
            <td style="width:60px;">
                <button type="button" data-unidadmedida="${c.unidad}" data-iddetallecompra="${c.idDetalleCompra}" data-index="${i}" class="btn btn-white edit-row-detalle">
                    <img class="botton-ac" src="../../Content/Images/editar.png" style="cursor:pointer; height: 1.3em; width:1.175em;" />
                </button>
            </td>
            <td style="width:60px;">
                <button type="button" data-index="${i}" class="btn btn-white delete-row-detalle"><i class="fa fa-trash"></i> </button>
            </td>
        </tr>`));


    if (detalles && detalles.some(c => c)) {
        $("#tabla-detalle-compra tfoot").addClass("hide");
    } else {
        $("#tabla-detalle-compra tfoot").removeClass("hide");
    }

    $(".delete-row-detalle").on("click", (ev) => EliminarDetalle(Number($(ev.currentTarget).data('index'))));
    $(".edit-row-detalle").on("click", (ev) => EditarDetalle(Number($(ev.currentTarget).data('index'))));
}

function ClickCierreCompra() {
    if (!$("#form-compra").psly().validate()) {
        return;
    }
    if (!DetalleCompra || !DetalleCompra.some(c => c)) {
        SendAlert("No has agregado insumos a esta compra", "error", false);
        return;
    }
    if ($("#factura").val().trim() === "") {
        SendAlert("Esta compra no tiene factura registrada", "warning", false);
    }
    $('#divCerrarCompra').removeClass('hide');
    //$("#modal-confirmar").modal();
}

function ClickConfirmarCompra() {
    if (($("#descuentos").val() !== "" && !$("#descuentos").parsley().validate())
        || ($("#iva").val() !== "" && !$("#iva").parsley().validate())
        || ($("#total").val() !== "" && !$("#total").parsley().validate())) {
        return;
    }
    InitLoader();

    let descuento = $("#descuentos").val() === "" ? "0.00" : $("#descuentos").val();
    let iva = $("#iva").val() === "" ? "0.00" : $("#iva").val();
    let total = $("#total").val() === "" ? "0.00" : $("#total").val();

    const totalCalculado = DetalleCompra.reduce((a, c) => a + c.importe, 0);
    const totalDescuento = Number(StripFormat(descuento));
    const totalIva = Number(StripFormat(iva));
    const totalIngresado = Number(StripFormat(total));
    
    // TODO: Revisar
    if (totalCalculado !== (totalIngresado + totalIva + totalDescuento)) {
        ShutDownLoader();
       // $("#modal-confirmar").modal("hide");
        $("#mensajeWarning").text("El total que ingresaste no coincide con los insumos capturados, por favor verifica los montos antes de continuar.");
        SendAlert("El total que ingresaste no coincide con los insumos capturados, por favor verifica los montos antes de continuar", "warning", true,$("#divAlertaCompra"));
    } else {
        $("#mensajeWarning").text("");
        const url = $("#form-confirmar").attr("action");
        const compra = {};
        Object.assign(compra, SerializeForm("#form-confirmar"));
        compra.iva = StripFormat(compra.iva);
        compra.descuentos = StripFormat(compra.descuentos);
        compra.total = StripFormat(compra.total);
        compra.observacion = $('#observacion').val();

        $.post(url, compra)      
            .done(notifyResponse)
            .fail(notifyError)
            .always(() => ShutDownLoader());
    }

    function notifyResponse() {
        window.location = $("#url-back").val();
    }
    function notifyError() {
        SendAlert("Ha ocurrido un error al cerrar la compra, por favor contacte a su administrador", "error", true);
    }
}

function ShowButtons() {
    if (window.Compra > 0) {

        if (window.estatusCompraId === 2) {
            ShowReabrirCompra();
        }
        else {
                ShowCierreCompra();
        }

    }
    else {

        ShowRegistroCompra();
    }
}
function ShowRegistroCompra () {
    $("#btn-capturar-compra").addClass("hide");
    $("#card-insumos").addClass("hide");
    $("#btn-Confirmar-Compra").addClass("hide");    

    $("#btn-registro-compra").removeClass("hide");
    $('#divCerrarCompra').addClass('hide');
}

function ShowCierreCompra () {
    $("#btn-capturar-compra").removeClass("hide");
    $("#card-insumos").removeClass("hide");
    $("#btn-Confirmar-Compra").addClass("hide");  

    $("#btn-registro-compra").addClass("hide");
    $('#divCerrarCompra').removeClass('hide');
    $('#btn-guardar-compra').removeClass('hide');
}

function ShowReabrirCompra() {

    $("#btn-capturar-compra").removeClass("hide");
    $("#card-insumos").removeClass("hide");

    $("#btn-registro-compra").addClass("hide");
    $('#divCerrarCompra').removeClass('hide');
    $('#btn-Confirmar-Reapertura').removeClass('hide');

}


function ClickGuardarCompra() {
    var isFechaCompraValid = $("#fechaCompra").parsley().whenValidate();
    isFechaCompraValid.done(() => { 
    if (!$("#sucursal").parsley().validate() || !$("#proveedor").parsley().validate()
        || !$("#remision").parsley().validate() || !$("#factura").parsley().validate()
        || $("#sucursal").val() === ""
        || $("#proveedor").val() === "" ) {
        return;
    }
    const url = $("#hUrlRegistrar").val();
    //const compra = {};
    //Object.assign(compra, SerializeForm("#form-compra"));
    const compra =
    {
        factura : $('#factura').val(),
        remision : $('#remision').val(),
        sucursal : $('#sucursal').val(),
        proveedor: $('[name=proveedor]').val(),
        fechaCompra: $('#fechaCompra').val(),
    };

    $.post(url, compra)
        .done(notifyResponse)
        .fail(notifyError)
        .always(() => ShutDownLoader());

    function notifyResponse(res) {
        window.Compra = res.id;
        $("#idCompra").val(res.id);
        ShowButtons();
        SendAlert("La compra ha sido guardada con éxito", "success", true);

        $('#divCerrarCompra').removeClass('hide');
    }
    function notifyError() {
        SendAlert("Ha ocurrido un error al guardar la compra, por favor contacte a su administrador", "error", true);
        }
    });
}

function ReformatInput(ev) {
    const $input = $(ev.currentTarget);
    const value = StripFormat($input.val());
    const formattedInput = FormatAsCurrency(value, true);
    $input.val(formattedInput.replace(",.", "."));
}

function InsertarNuevoInsumo(insumo) {
    const url = $("#form-agregar-insumo").attr("action");
    InitLoader();

    insumo.compra = window.Compra;

    insumo.idDetalleCompra = $('#idDetalleCompra').val();

    $.post(url, insumo)
        .done(notifyResponse)
        .fail(notifyError)
        .always(() => ShutDownLoader());

    function notifyResponse(res) {
        if (insumo.idDetalleCompra > 0) {
            window.DetalleCompra = window.DetalleCompra.filter(f => f.idDetalleCompra != res.idDetalleCompra);
        }
        window.DetalleCompra.push(res);
        window.DetalleCompra = window.DetalleCompra.sort((a, b) => (a.desc > b.desc) ? 1 : ((b.desc > a.desc) ? -1 : 0))

        SendAlert("El insumo ha sido guardada con éxito", "success", false);
        RedrawTableDetalle(window.DetalleCompra);
        limpiarModalInsumo();
    }
    function notifyError() {
        SendAlert("Ha ocurrido un error al guardar el insumo, por favor intente de nuevo más tarde", "error", true);
    }

}
function ValidarInsumo(insumo) {
    const url = $("#form-compra-anterior").attr("action");
    InitLoader();

    $.post(url, insumo)
        .done(notifyResponse)
        .fail(notifyError)
        .always(() => ShutDownLoader());

    function notifyResponse(res) {
        if (res.success) {
            //$("#modal-agregar-insumos").modal("hide");
            InsertarNuevoInsumo(insumo);
            $("#warning-compra").addClass("hide");
            //$("#btn-confirmar-compra-anterior").addClass("hide");
            //$("#btn-guardar-insumos").removeClass("hide");
        } else {
            $('#modal-agregar-insumos').modal();
            $("#item").val(JSON.stringify(insumo));
            $("#compra-anterior").text(FormatAsCurrency(res.anterior));
            $("#compra-actual").text(FormatAsCurrency(res.actual));
            //$("#modal-compra-anterior").modal();
            $("#warning-compra").removeClass("hide");
            //$("#btn-confirmar-compra-anterior").removeClass("hide");
            //$("#btn-guardar-insumos").addClass("hide");
        }
    }
    function notifyError() {
        SendAlert("Ha ocurrido un error al guardar el insumo, por favor intente de nuevo más tarde", "error", true);
    }
}

function ClickConfirmarCompraAnterior() {
    console.log('ClickConfirmarCompraAnterior');
    //$("#modal-agregar-insumos").modal("hide");

    $("#modal-compra-anterior").modal("hide");
    const insumo = JSON.parse($("#item").val());

    if (!$('#form-agregar-insumo').psly().validate()) return;
    let $row = $('#form-agregar-insumo');
    let $selectInsumos = $row.find(".selectpicker");
    let insumoSelectedDesc = $selectInsumos.children("option:selected").text();
    let insumoSelected = $selectInsumos.val();
    if (!insumoSelected) return;
    let importe = StripFormat($row.find("input[name='importe']").val());
    let cantidad = StripFormat($row.find("input[name='cantidad']").val());
    let unidad = StripFormat($row.find("#label-unidad-medida").val());

    let insumoAgregar = {
        importe: importe,
        cantidad: cantidad,
        insumo: insumoSelected,
        desc: insumoSelectedDesc,
        unidad: unidad
    };

    InsertarNuevoInsumo(insumoAgregar);
    $('#modal-agregar-insumos').modal('hide');
    /*
    if (insumo.insumo === insumoSelected && insumo.importe === importe && insumo.cantidad === cantidad) {
        InsertarNuevoInsumo(insumoAgregar);
        $("#warning-compra").addClass("hide");
        //$("#btn-confirmar-compra-anterior").addClass("hide");
        //$("#btn-guardar-insumos").removeClass("hide");

    } else{
        ClickAgregarInsumos();
    }
    */
}

function SearchUnidadMinimaMedida() {
    const insumo = $("#insumo").val();
    const url = $("#url-search-unidad-minima").val();
    if (!insumo || insumo === "")
    {
        $("#label-unidad-medida").text("-");
        return;
    }
    
    InitLoader();

    $.post(url, { id : insumo })
        .done(notifyResponse)
        .fail(notifyError)
        .always(() => ShutDownLoader());

    function notifyResponse(res) {
        $("#label-unidad-medida").text(`${res.Text}`);
    }
    function notifyError() {
        SendAlert("Ha ocurrido un error al obtener la unidad mínima de medida, por favor intente de nuevo más tarde", "error", true);
    }
}
function UpdateTitle() {
    const sucursal = ObtenerSucursal();
    const proveedor = ObtenerProveedor();
    $("#title-compra").text(`Agregar compra - Sucursal: `);
    $("#title-compra-proveedor").text(` - Proveedor: ${proveedor ? proveedor : "-"}`);
    
}

function EliminarDetalle(index) {
    var detalle = DetalleCompra[index];
    const url = $("#url-detalle-eliminar").val();
    InitLoader();
    $.post(url, { insumo: detalle.insumo, compra: detalle.compra })
        .done(notifyResponse)
        .fail(notifyError)
        .always(() => ShutDownLoader());

    function notifyResponse(res) {
        DetalleCompra.splice(index, 1);
        RedrawTableDetalle(DetalleCompra);
    }
    function notifyError() {
        SendAlert("Ha ocurrido un error al eliminar el insumo, por favor intente de nuevo más tarde", "error", true);
    }
}

function EditarDetalle(index) {
    var detalle = DetalleCompra[index];

    $("#insumo option").remove();
    $("#insumo").append(`<option data-desc="${detalle.desc}" value="${detalle.insumo}">${detalle.desc}</option>`);

    $('#cantidad').val(detalle.cantidad);
    $('#importe').val(detalle.importe);
    $('#label-unidad-medida').val(detalle.importe);

    $('#insumo').val(detalle.insumo);
    $('.selectpicker').selectpicker('refresh');
    $('#idDetalleCompra').val(detalle.idDetalleCompra);

    $('#divAgregarInsumo').addClass("hidden");
    $('#divEditarInsumo').removeClass("hidden");

    //limpiar y cargar el puro insumo
}

function ClickEditarInsumos() {
    if (!$('#form-agregar-insumo').psly().validate()) return;
    let hasErrors = false;
    const $row = $('#form-agregar-insumo');
    const $selectInsumos = $row.find(".selectpicker");
    const insumo = $selectInsumos.val();
    if (!insumo) return;
    const descripcion = $row.find(`select[name='insumo'] option[value='${insumo}']`).data('desc');
    const importe = StripFormat($row.find("input[name='importe']").val());
    const cantidad = StripFormat($row.find("input[name='cantidad']").val());
    const unidad = $("#label-unidad-medida").text();
    if (importe === "") {
        $(`#error-importe`).text("Este valor es requerido");
        hasErrors = true;
        return;
    }
    if (Number(importe) <= 0) {
        $(`#error-importe`).text("Este valor debe ser mayor a cero");
        hasErrors = true;
        return;
    }
    if (cantidad === "") {
        $(`#error-cantidad`).text("Este valor es requerido");
        hasErrors = true;
        return;
    }
    if (Number(cantidad) <= 0) {
        $(`#error-cantidad`).text("Este valor debe ser mayor a cero");
        hasErrors = true;
        return;
    }
    $(`#error-importe`).text("");

    ValidarInsumo({
        insumo: insumo,
        descInsumo: descripcion,
        importe: Number(importe),
        cantidad: Number(cantidad),
        unidad: unidad
    });
}

function limpiarModalInsumo() {
    $("#importe").val('');
    $("#cantidad").val('');
    $("#insumo").val('');
    $("#insumo").selectpicker("refresh");
    $('#idDetalleCompra').val('0');

    $('#divAgregarInsumo').removeClass("hidden");
    $('#divEditarInsumo').addClass("hidden");

    cargarSelectInsumos();
}

function ClickCancelarCierreCompra() {
    $('#divCerrarCompra').addClass('hide');
}