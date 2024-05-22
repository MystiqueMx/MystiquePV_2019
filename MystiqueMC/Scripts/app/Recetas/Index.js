window.InsumoReceta = [];
window.InsumoRecetaProcesado = [];

function OnScriptsLoad() {
    SetUpEvents();
    //SetUpDataTableCatalogos();
}

function SetUpEvents() {
    // Recetas
    //$("#modal-procesado").on("hidden.bs.modal", () => location.reload());
    $("#button-agregar-receta").on("click", () => AbrirModalReceta({}));
    $("#btn-agregar-insumo-receta").on("click", () => AgregarInsumoReceta());
    $("#btn-eliminar-receta").on("click",
        () => {
            $("#modal-receta").modal("hide");
            $("#modal-eliminar-receta").modal();
        });
    $("#btn-cancelar-insumo-producto").on("click",
        () => {
            $("#modal-detalle-receta").modal("hide");
            $("#modal-receta").modal();
        });
    $("#btn-guardar-insumo-producto").on("click", () => AgregarDetalleReceta());
    $(".trigger-editar-receta").on("click", event => EditarReceta($(event.currentTarget).data("id")));
    $("#btn-guardar-receta").on("click", () => ActualizarReceta());
    // Procesados
    $("#button-agregar-procesado").on("click", () => AbrirModalProcesado({}));
    $("#btn-agregar-insumo-procesado").on("click", () => AgregarInsumoProcesado());
    //$("#btn-eliminarProcesado").on("click",
    //    () => {
    //        $("#modal-procesado").modal("hide");
    //        $("#modal-eliminar-procesado").modal();
    //    });
    $("#btn-cancelar-insumo-procesado").on("click",
        () => {
            $("#modal-detalle-procesado").modal("hide");
            $("#modal-procesado").modal();
        });
    $("#btn-guardar-insumo-procesado").on("click", () => AgregarDetalleProcesado());
    $(".trigger-editar-procesado").on("click", event => EditarProcesado($(event.currentTarget).data("id")));
    $("#btn-guardar-procesado").on("click", () => ActualizarProcesado());

    $("#modal-activar-receta").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var receta = $button.data("receta");
            var id = $button.data("id");

            $("#modal-activar-receta #receta").text(receta);
            $("#modal-activar-receta #idReceta").val(id);

        });

    $("#modal-inactivar-receta").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var receta = $button.data("receta");
            var id = $button.data("id");
            $("#modal-inactivar-receta #receta").text(receta);
            $("#modal-inactivar-receta #idReceta").val(id);

        });

    $("#modal-eliminar-procesado").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var receta = $button.data("receta");
            var id = $button.data("id");

            $("#modal-eliminar-procesado #descProcesado").text(receta);
            $("#modal-eliminar-procesado #id-eliminarProcesado").val(id);

        });

    $('#insumoDetalleProcesado').on('change', obtenerMedida);
    $('.collapse').collapse('hide');
    $('.collapse-recetas').collapse('show');
}

function AbrirModalReceta(receta) {
    window.InsumoReceta = [];
    $("#form-receta-producto").psly().reset();
    if (receta.id) {
        $("#id").val(receta.id);
        $("#id-eliminar").val(receta.id);
        $("#merma").val(receta.mermaPermitida);
        $("#caducidad").val(receta.diasCaducidad);
        $("#descripcion").val(receta.descripcion);
        $("#desc").text(receta.descripcion);
        $("#producto").selectpicker("val", receta.productoId);

        window.InsumoReceta = receta.insumos;
        $("#btn-eliminar-receta").removeClass("hide");
        $("#modal-receta .modal-title").text("Editar Receta Producto");
    } else {
        $("#id").val("");
        $("#id-eliminar").val("");
        $("#merma").val("");
        $("#caducidad").val("");
        $("#descripcion").val("");
        $("#desc").text("");
        $("#producto").selectpicker("val", "");

        $("#btn-eliminar-receta").addClass("hide");
        $("#modal-receta .modal-title").text("Agregar Receta Producto");
    }
    RedrawInsumosReceta(window.InsumoReceta);
    $("#modal-receta").modal();
}

function ActualizarReceta() {
    $("#idReceta").val($("#id").val());
    InitLoader();
    var url = $("#url-submit-receta").val();
    var payload = getFormData("#form-receta-producto");
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed)
        .always(ShutDownLoader);
    function handleResponse(response) {
        $("#modal-receta").modal("hide");
        location.reload();
    }
    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}

function AgregarInsumoReceta() {
    if (!$("#form-receta-producto").psly().validate()) return;
    
    if ($("#id").val() === "") {
        $("#idReceta").val($("#id").val());
        InitLoader();
        var url = $("#url-submit-receta").val();
        var payload = getFormData("#form-receta-producto");
        $.post(url, payload)
            .done(res => handleResponse(res))
            .fail(notifyFailed)
            .always(ShutDownLoader);

    } else {
        AbrirModalInsumoReceta({});
    }

    function handleResponse(response) {
        $("#id").val(response.id);
        $("#idReceta").val($("#id").val());
        $("#id-eliminar").val($("#id").val());
        AbrirModalInsumoReceta({});
    }
    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}

function AbrirModalInsumoReceta(insumo) {
    $("#idReceta").val($("#id").val());
    if (insumo.id) {
        $("#insumoDetalle").selectpicker("val", insumo.insumoId);
        $(`#insumoDetalle option[value='${insumo.insumoId}']`).attr('selected', true);
        $("#row-insumo").addClass("hide");

        $("#unidad").selectpicker("val", insumo.unidadMedidaId);

        $("#cantidad").val(insumo.cantidad);

        $("#modal-detalle-receta .modal-title").text("Editar insumo receta");
    } else {
        $("#insumoDetalle").selectpicker("val", "");
        $("#row-insumo").removeClass("hide");

        $("#unidad").selectpicker("val", "");
        $("#cantidad").val("");

        $("#modal-detalle-receta .modal-title").text("Agregar insumo receta");
    }
    DeshabilitarInsumos("#insumoDetalle", window.InsumoReceta);
    $("#modal-receta").modal("hide");
    $("#modal-detalle-receta").modal();
}

function getFormData(form) {
    const unindexedArray = $(form).serializeArray();
    const indexedArray = {};

    $.map(unindexedArray, (n) => {
        indexedArray[n.name] = n.value;
    });

    return indexedArray;
}

function EditarReceta(id) {
    InitLoader();
    var url = $("#url-get-receta").val();
    var payload = {
        id
    };
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed)
        .always(ShutDownLoader);

    function handleResponse(response) {
        AbrirModalReceta(response);
    }
    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}

function RedrawInsumosReceta(insumos) {
    $("#tabla-detalle-receta tbody").empty();
    if (insumos && insumos.some(c => c)) {
        $("#tabla-detalle-receta tbody").append(insumos.map((c, i) => `<tr><td>${c.insumo}</td><td>${c.cantidad} ${c.unidadMedida} </td><td class="text-right"><div class="btn-group">
          <button type="button" class="btn btn-sm btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
            Editar <span class="caret"></span>
          </button>
          <ul class="dropdown-menu">
            <li><a href="#" class="trigger-edit-insumo-receta" data-index="${i}">Cambiar cantidades</a></li>
            <li><a href="#" class="trigger-delete-insumo-receta" data-index="${i}">Remover insumo</a></li>
          </ul>
        </div></td></tr>`));
        $("#tabla-detalle-receta .trigger-edit-insumo-receta")
            .on("click", event => EditarDetalleReceta($(event.currentTarget).data("index")));
        $("#tabla-detalle-receta .trigger-delete-insumo-receta")
            .on("click", event => EliminarDetalleReceta($(event.currentTarget).data("index")));
    }
}

function AgregarDetalleReceta() {
    InitLoader();
    var url = $("#form-insumo-detalle").attr("action");
    var payload = getFormData("#form-insumo-detalle");
    payload.insumoDetalle = $(`#insumoDetalle option:selected`).val();
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed)
        .always(ShutDownLoader);

    function handleResponse() {
        $("#modal-detalle-receta").modal("hide");
        EditarReceta($("#id").val());
    }
    function notifyFailed() {
        $("#modal-detalle-receta").modal("hide");
        $("#modal-receta").modal();
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}

function AgregarDetalleProcesado() {
    InitLoader();
    var url = $("#form-procesado-detalle").attr("action");
    var payload = getFormData("#form-procesado-detalle");
    payload.insumoDetalleProcesado = $(`#insumoDetalleProcesado option:selected`).val();
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed)
        .always(ShutDownLoader);

    function handleResponse() {
        $("#modal-detalle-procesado").modal("hide");
        EditarProcesado($("#idProcesado").val());
    }
    function notifyFailed() {
        $("#modal-detalle-procesado").modal("hide");
        $("#modal-procesado").modal();
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}
function EditarDetalleReceta(index) {
    AbrirModalInsumoReceta(window.InsumoReceta[index]);
}
function EliminarDetalleReceta(index) {
    InitLoader();
    var url = $("#url-delete-detalle-receta").val();
    var payload = window.InsumoReceta[index];
    payload.idReceta = $("#id").val();
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed)
        .always(ShutDownLoader);

    function handleResponse(response) {
        AbrirModalReceta(response);
        EditarReceta(payload.idReceta);
    }
    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}

function AbrirModalProcesado(receta) {
    window.InsumoRecetaProcesado = [];
    $("#form-receta-procesado").psly().reset();
    if (receta.id) {
        $("#idProcesado").val(receta.id);
        $("#id-eliminarProcesado").val(receta.id);
        $("#mermaProcesado").val(receta.mermaPermitida);
        $("#caducidadProcesado").val(receta.diasCaducidad);
        $("#descripcionProcesado").val(receta.descripcion);
        $("#descProcesado").text(receta.descripcion);
        $("#productoProcesado").selectpicker("val", receta.insumoId);
        $("#tipoProcesado").selectpicker("val", receta.tipoRecetaId);
        $("#equivalencia").val(receta.equivalencia);
        $("#unidadMedidaIdCompra").val(receta.unidadMedidaIdCompra);       
        $("#unidadMedidaIdMinima").val(receta.unidadMedidaIdMinima);
        $("#cantidadProcesados").val(receta.cantidadProcesado);
        $("#insumoFamilia").selectpicker("val", receta.categoriaInsumoId);
        window.InsumoRecetaProcesado = receta.insumos;
       // $("#btn-eliminarProcesado").removeClass("hide");
        $("#modal-procesado .modal-title").text("Editar Procesado");
    } else {
        $("#idProcesado").val("");
        $("#id-eliminarProcesado").val("");
        $("#mermaProcesado").val("");
        $("#caducidadProcesado").val("");
        $("#descripcionProcesado").val("");
        $("#descProcesado").text("");
        $("#productoProcesado").selectpicker("val", "");
        $("#tipoProcesado").selectpicker("val", "");       
        $("#equivalencia").val("");
        $("#unidadMedidaIdCompra").val("");
        $("#unidadMedidaIdMinima").val("");
        $("#cantidadProcesados").val("");
        $("#insumoFamilia").selectpicker("val", "");
       // $("#btn-eliminarProcesado").addClass("hide");
        $("#modal-procesado .modal-title").text("Agregar Procesado");
    }
    RedrawInsumosProcesado(window.InsumoRecetaProcesado);
    $("#modal-procesado").modal();
}

function ActualizarProcesado() {
    $("#idProcesado").val($("#idProcesado").val());
    InitLoader();
    var url = $("#url-submit-procesado").val();
    var payload = getFormData("#form-receta-procesado");
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed)
        .always(ShutDownLoader);
    function handleResponse(response) {
        $("#modal-procesado").modal("hide");
        location.reload();
    }
    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}

function AgregarInsumoProcesado() {
    if (!$("#form-receta-procesado").psly().validate()) return;

    if ($("#idProcesado").val() === "") {
        InitLoader();
        var url = $("#url-submit-procesado").val();
        var payload = getFormData("#form-receta-procesado");
        $.post(url, payload)
            .done(res => handleResponse(res))
            .fail(notifyFailed)
            .always(ShutDownLoader);

    } else {
        AbrirModalInsumoProcesado({});
    }

    function handleResponse(response) {
        $("#idProcesado").val(response.id);
        $("#idProcesado").val($("#idProcesado").val());
        $("#id-eliminarProcesado").val($("#idProcesado").val());
        AbrirModalInsumoProcesado({});
    }
    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}

function AbrirModalInsumoProcesado(insumo) {
    $("#idProcesadoInsumo").val($("#idProcesado").val());
    if (insumo.id) {
        $("#insumoDetalleProcesado").selectpicker("val", insumo.insumoId);
        $(`#insumoDetalleProcesado option[value='${insumo.insumoId}']`).attr('selected', true);
        $("#row-insumoProcesado").addClass("hide");

        $("#unidadProcesado").val(insumo.unidadMedidaId);      
        $("#unidadDescripcion").val(insumo.unidadMedida);  
        $("#cantidadProcesado").val(insumo.cantidad);

        $("#title").text("Editar insumo receta procesado: ");
        $("#nombreReceta").text(insumo.descripcion);
    } else {
        $("#insumoDetalleProcesado").selectpicker("val", "");
        $("#row-insumoProcesado").removeClass("hide");

        $("#unidadProcesado").val("");
        $("#unidadDescripcion").val(""); 
        $("#cantidadProcesado").val("");

        $("#title").text("Agregar insumo receta procesado: ");
        //$("#nombreReceta").text(insumo.descripcion);

        var descripcion = $("#descripcionProcesado").val();
        $("#nombreReceta").text(descripcion);
    }
    DeshabilitarInsumos("#insumoDetalleProcesado", window.InsumoRecetaProcesado);
    $("#modal-procesado").modal("hide");
    $("#modal-detalle-procesado").modal();
}

function EditarProcesado(id) {
    InitLoader();
    var url = $("#url-get-procesado").val();
    var payload = {
        id
    };
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed)
        .always(ShutDownLoader);

    function handleResponse(response) {
        AbrirModalProcesado(response);
    }
    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}

function RedrawInsumosProcesado(insumos) {
    $("#tabla-detalle-procesado tbody").empty();
    if (insumos && insumos.some(c => c)) {
        $("#tabla-detalle-procesado tbody").append(insumos.map((c, i) => `<tr>
            <td>${c.insumo}</td>
            <td>${c.cantidad} ${c.unidadMedida} </td>
            <td class="text-right">
              <div class="btn-group">    
                <a href="#" class="trigger-edit-insumo-procesado" data-index="${i}"><i class="fa fa-edit" style="color:#696969"></i></a>
                <a href="#" class="trigger-delete-insumo-procesado" data-index="${i}"><i class="fa fa-trash" style="color:#696969"></i></a>
              </div>
            </td>
        </tr>`));
        $("#tabla-detalle-procesado .trigger-edit-insumo-procesado")
            .on("click", event => EditarDetalleProcesado($(event.currentTarget).data("index")));
        $("#tabla-detalle-procesado .trigger-delete-insumo-procesado")
            .on("click", event => EliminarDetalleProcesado($(event.currentTarget).data("index")));
    }
}

function EditarDetalleProcesado(index) {
    AbrirModalInsumoProcesado(window.InsumoRecetaProcesado[index]);
}

function EliminarDetalleProcesado(index) {
    InitLoader();
    var url = $("#url-delete-detalle-procesado").val();
    var payload = window.InsumoRecetaProcesado[index];
    payload.idProcesado = $("#idProcesado").val();
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed)
        .always(ShutDownLoader);

    function handleResponse(response) {
        AbrirModalProcesado(response);
        EditarProcesado(payload.idProcesado);
    }
    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}

function DeshabilitarInsumos(idSelect, insumosSeleccionados) {
    $(idSelect).find("option").prop("disabled", false);
    if (insumosSeleccionados && insumosSeleccionados.some(c => c)) {
        insumosSeleccionados.map(c => $(idSelect).find(`option[value="${c.insumoId}"]`).prop("disabled", true));
    }
    $(idSelect).selectpicker("refresh");
}

function obtenerMedida() {
    var insumoDetalleProcesado = $('#insumoDetalleProcesado').val();

    $("unidadProcesado").empty();
    $.ajax({
        type: "POST",
        url: $('#urlObtenerUnidadProcesado').val(),
        dataType: 'json',
        data: { idInsumo: insumoDetalleProcesado },
    }).done(function (response) {
        if (response !== null && response.exito) {
            try {
                for (unidadMedida of response.resultado) {
                    var id = unidadMedida.IdUnidadMedida;
                    var descripcion = unidadMedida.Descripcion;
                    $('#unidadProcesado').val(id);
                    $('#unidadDescripcion').val(descripcion);
                }
            }
            catch (err) {
                console.error(err);
            }
        }
        else {
            alert("Ocurrio un error al obtener la información, contacte a su administrador.");
        }
    });
}