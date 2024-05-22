var $form;
var $formVariedad;
var $imageVariedad;
var $imageEditar;
var $image;
var cropper;
var filetype = "image/png";

function OnScriptsLoad() {
    window.Parsley.on('field:error', () => ShutDownLoader());

    filetype = obtenerFormatoDataImg($("#Imagen").val()) || filetype;
    $form = $("#form-create");
    $formVariedad = $("#form-agregar-varidad");
    SetUpImageInput();
    SetUpImageCropper();
    SetUpImageInputVariedad();
    //$("#TipoProducto").on("change", HideVariedades);
    HideVariedades();
    $("#btn-guardar").on("click", EnviarForma);

    $("#btn-guardar-variedad").on("click", EnviarFormaVariedad);
    $("#btn-agregar-insumo-receta").on("click", () => AgregarInsumoReceta());
    $("#btn-guardar-edit").on("click", EnviarFormaVariedadEdit);

    $("#btn-on").on("click",
        () => {
            $("#btn-off").removeClass("hide");
            $("#btn-on").addClass("hide");
            $("#ArmarCobro").val("False");
        });

    $("#btn-off").on("click",
        () => {
            $("#btn-off").addClass("hide");
            $("#btn-on").removeClass("hide");
            $("#ArmarCobro").val("True");
        });

    $("#TipoProducto").on("change", () => {
        if ($("#TipoProducto").val() === "2" || $("#TipoProducto").val() === "3") {
            $("#es-ensalada-div").removeClass("hide");


        } else {
            $("#btn-off").removeClass("hide");
            $("#btn-on").addClass("hide");
            $("#es-ensalada-div").removeClass("hide");
            $("#ArmarCobro").val("False");


        }
    });
    $("#tabla-detalle-receta .trigger-delete-insumo-receta")
        .on("click", event => EliminarDetalleReceta({
            idReceta: $(event.currentTarget).data("receta"),
            insumoId: $(event.currentTarget).data("insumo"),
            cantidad: $(event.currentTarget).data("cantidad"),
            unidad: $(event.currentTarget).data("unidad")
        }));
    $(".trigger-edit-insumo-receta")
        .on("click", event => AbrirModalInsumoReceta({
            receta: $(event.currentTarget).data("receta"),
            insumo: $(event.currentTarget).data("insumo"),
            cantidad: $(event.currentTarget).data("cantidad"),
            unidad: $(event.currentTarget).data("unidad")
        },true));
    $("#btn-agregar-insumo-receta").on("click", () => AgregarInsumoReceta());
    $("#btn-guardar-insumo-producto").on("click", () => AgregarDetalleReceta());
    $(".trigger-editar-receta").on("click", ev => {
        modaltitle = $(ev.currentTarget).data("desc");
        modaltitle2 = $(ev.currentTarget).data("desc2");
        EditarReceta(null, ev);
    });
    $("#insumoDetalle").on("changed.bs.select", () => SearchUnidadMinima());
    SetUpDualSelect();
    SetUpModal();
    $("#TipoProducto").on("change", HandleTipoChange);
    $("#btn-confirmar-cambio").on("click", ConfirmarCambioTipo);
    $("#btn-revertir-cambio").on("click", RevertirCambioTipo);
    $("#es-ensalada-div").removeClass("hide");
    if (tipo === 1 || tipo == 3) {
        $("#row-recetas").removeClass("hide");
    }
}
function EliminarDetalleReceta(payload) {
    InitLoader();
    var url = $("#url-delete-detalle-receta").val();
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed);

    function handleResponse(response) {
        window.location.reload();
    }
    function notifyFailed() {
        ShutDownLoader();
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}
function AbrirModalInsumoReceta(insumo, isEditar) {
    $("#idReceta").val($("#id").val());
    $("#modal-receta").modal("hide");
    $("#modal-detalle-receta").modal();
    if (insumo.insumo) {
        $("#insumoDetalle").selectpicker("val", insumo.insumo);
        $(`#insumoDetalle option[value='${insumo.insumo}']`).attr('selected', true);
        $("#row-insumo").addClass("hide");

        $("#unidad").val(insumo.unidad);

        $("#cantidad").val(insumo.cantidad);
        $("#insumoId").val(insumo.insumo);
    } else {
        $("#insumoDetalle").selectpicker("val", "");
        $("#insumoDetalle").selectpicker("refresh");
        $("#row-insumo").removeClass("hide");

        $("#medida-descripcion").text("");
        $("#unidad").val("");
        $("#cantidad").val("");

    }
    SearchUnidadMinima(isEditar, insumo.insumo);
    //DeshabilitarInsumos("#insumoDetalle", window.InsumoReceta);
    
}
function AgregarDetalleReceta() {
    InitLoader();
    var cantidad = $("#cantidad");
    cantidad.val(cantidad.val().replace(',', ''));

    var url = $("#form-insumo-detalle").attr("action");
    var payload = getFormData("#form-insumo-detalle");
    payload.insumoDetalle = $('#insumoId').val();
    //payload.insumoDetalle = $(`#insumoDetalle option:selected`).val();
    $.post(url, payload)
        .done(res => handleResponse(res))
        .fail(notifyFailed);

    function handleResponse() {
        $("#modal-detalle-receta").modal("hide");
        window.location.reload();
    }
    function notifyFailed() {
        ShutDownLoader();
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}
function getFormData(form) {
    const unindexedArray = $(form).serializeArray();
    const indexedArray = {};

    $.map(unindexedArray, (n) => {
        indexedArray[n.name] = n.value;
    });

    return indexedArray;
}
function AgregarInsumoReceta() {
    if (idRecetaAbierta !== null) {
        $("#idReceta").val(idRecetaAbierta);
        InitLoader();
        var url = $("#url-submit-receta").val();
        var payload = getFormData("#form-receta-producto");
        $.post(url, payload)
            .done(res => handleResponse(res))
            .fail(notifyFailed)
            .always(ShutDownLoader);

    } else {
        AbrirModalInsumoReceta({}, false);
    }

    function handleResponse(response) {
        $("#id").val(response.id);
        idRecetaAbierta = response.id;
        $("#idReceta").val($("#id").val());
        $("#id-eliminar").val($("#id").val());
        AbrirModalInsumoReceta({}, false);
    }
    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la receta por favor intente de nuevo", "error", false);
    }
}
function DeshabilitarInsumos(idSelect, insumosSeleccionados) {
    $(idSelect).find("option").prop("disabled", false);
    if (insumosSeleccionados && insumosSeleccionados.some(c => c)) {
        insumosSeleccionados.map(c => $(idSelect).find(`option[value="${c}"]`).prop("disabled", true));
    }
    $(idSelect).selectpicker("refresh");
}
function obtenerFormatoDataImg(dataUri) {
    const noData = dataUri.replace("data:", "");
    const stopIndex = noData.indexOf(";");
    return noData.substring(0, stopIndex);
}
function SetUpImageInput() {
    var $inputImage = $("#inputImages");
    if (window.FileReader) {
        $inputImage.change(function () {
            var fileReader = new FileReader(),
                files = this.files,
                file;

            if (!files.length) {
                return;
            }

            file = files[0];

            if (/^image\/\w+$/.test(file.type)) {
                fileReader.readAsDataURL(file);
                filetype = file.type;
                fileReader.onload = function () {
                    $inputImage.val("");
                    $image.cropper("reset", true).cropper("replace", this.result);
                };
            } else {
                console.error("Error SetUpImageInput");
            }
        });
    } else {
        $inputImage.addClass("hide");
    }

    var $inputImageVariedad = $("#inputImages-variedad");
    if (window.FileReader) {
        $inputImageVariedad.change(function () {
            var fileReader = new FileReader(),
                files = this.files,
                file;

            if (!files.length) {
                return;
            }

            file = files[0];

            if (/^image\/\w+$/.test(file.type)) {
                fileReader.readAsDataURL(file);
                filetype = file.type;
                fileReader.onload = function () {
                    $inputImageVariedad.val("");
                    $image.cropper("reset", true).cropper("replace", this.result);
                };
            } else {
                console.error("Error SetUpImageInput");
            }
        });
    } else {
        $inputImageVariedad.addClass("hide");
    }

    var $inputImageVariedadEditar = $("#inputImagesEditar");
    if (window.FileReader) {
        $inputImageVariedadEditar.change(function () {
            var fileReader = new FileReader(),
                files = this.files,
                file;

            if (!files.length) {
                return;
            }

            file = files[0];

            if (/^image\/\w+$/.test(file.type)) {
                fileReader.readAsDataURL(file);
                filetype = file.type;
                fileReader.onload = function () {
                    $inputImageVariedadEditar.val("");
                    $imageEditar.cropper("reset", true).cropper("replace", this.result);
                };
            } else {
                console.error("Error SetUpImageInput");
            }
        });
    } else {
        $inputImageVariedadEditar.addClass("hide");
    }
}
function SetUpImageCropper() {
    $image = $("#images");
    $image.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
    cropper = $image.data("cropper");

    $imageVariedad = $("#images-variedad");
    $imageVariedad.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
    cropper = $imageVariedad.data("cropper");

    $imageEditar = $("#imagesEditar");
    $imageEditar.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
    cropper = $imageEditar.data("cropper");
}
function EnviarForma(evt) {
    if (!$form.parsley().validate()) return;
    StartSpinner();
    var image = $image.cropper("getCroppedCanvas", {
        width: 200,
        height: 200,
        minWidth: 200,
        minHeight: 200,
        maxWidth: 200,
        maxHeight: 200,
        fillColor: "#fff",
        imageSmoothingEnabled: false
    }).toDataURL(filetype);
    $("#Imagen").val(image);
    $form.submit();
}
function EnviarFormaVariedad(evt) {
    if (!$form.parsley().validate()) return;
    StartSpinner();
    var image = $imageVariedad.cropper("getCroppedCanvas", {
        width: 200,
        height: 200,
        minWidth: 200,
        minHeight: 200,
        maxWidth: 200,
        maxHeight: 200,
        fillColor: "#fff",
        imageSmoothingEnabled: false
    }).toDataURL(filetype);
    $("#ImagenAgregarVariedad").val(image);
    $formVariedad.submit();
}
function EnviarFormaVariedadEdit(evt) {
    if (!$form.parsley().validate()) return;
    StartSpinner();
    var image = $imageEditar.cropper("getCroppedCanvas", {
        width: 200,
        height: 200,
        minWidth: 200,
        minHeight: 200,
        maxWidth: 200,
        maxHeight: 200,
        fillColor: "#fff",
        imageSmoothingEnabled: false
    }).toDataURL(filetype);
    $("#ImagenEdit").val(image);
    $formVariedad.submit();
}
function StartSpinner() {
}
function StopSpinner() {
}
function HideVariedades() {
    if ($("#TipoProducto").val() === "1") {
        $("#row-variedades").removeClass("hide");
        $("#row-configuracion").addClass("hide");
    } else {
        $("#row-variedades").addClass("hide");
        $("#row-configuracion").removeClass("hide");
    }
}

function SetUpImageInputVariedad() {
    var $inputImage = $("#inputImages-variedad");
    if (window.FileReader) {
        $inputImage.change(function () {
            var fileReader = new FileReader(),
                files = this.files,
                file;

            if (!files.length) {
                return;
            }

            file = files[0];

            if (/^image\/\w+$/.test(file.type)) {
                fileReader.readAsDataURL(file);
                filetypeVariedad = file.type;
                fileReader.onload = function () {
                    $inputImage.val("");
                    $imageVariedad.cropper("reset", true).cropper("replace", this.result);
                };
            } else {
                console.error("Error SetUpImageInput");
            }
        });
    } else {
        $inputImage.addClass("hide");
    }

    $imageVariedad = $("#images-variedad");
    $imageVariedad.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
}



$('[name="eliminarVariedad"]').click(function () {
    debugger
    var data = $.parseJSON($(this).attr('data-button'));
    
    $("#idVariedadEliminar").val(data.Id);
    $("#modal-eliminar").modal();
});


$('[name="editarVariedad"]').click(function () {
    debugger
    var data = $.parseJSON($(this).attr('data-button'));

    $("#IdVariedadProducto").val(data.Id);
    $("#DescripcionEditar").val(data.Nombre);
    $("#imagesEditar").attr("src", data.Imagen);
    $("#modal-editar-variedad").modal();
});

function SetUpDualSelect() {
    $("#Opciones").bootstrapDualListbox({
        nonSelectedListLabel: "No asignados",
        selectedListLabel: "Asignados",
        preserveSelectionOnMove: "moved",
        infoTextEmpty: "",
        filterTextClear: "Ver todos",
        filterPlaceHolder: "Buscar...",
        moveSelectedLabel: "Mover",
        moveAllLabel: "Mover todos",
        removeSelectedLabel: "Remover",
        removeAllLabel: "Remover todos",
        infoText: "",
        infoTextFiltered: '<span class="label label-info">Filtrando</span> {0} de {1}'
    });
}

function ShowModalAgrupador(agrupador) {

    $("#Opciones option").prop("selected", false);
    if (agrupador.Id) {
        $("#modal-add-agrupador .modal-title").text("Editar configuración");
        $("#Id").val(agrupador.Id);
        $("#Descripcion").val(agrupador.Descripcion);
        $("#Cantidad").val(agrupador.Cantidad);
        $("#Indice").val(agrupador.Indice);
        $("#CostoExtra").val(agrupador.CostoExtra);



        var ddlArrayText = new Array();
        var ddlArrayId = new Array();
        var ddl = document.getElementById('categoriaIngredienteId');
        if (ddl != null) {
            document.getElementById("Descripcion").disabled = true;
            $("#Descripcion").removeClass('hidden');
            $("#categoriaIngredienteId").addClass('hidden');
            //for (i = 1; i < ddl.options.length; i++) {
            //    ddlArrayText[i - 1] = ddl.options[i].innerText;
            //    ddlArrayId[i - 1] = ddl.options[i].value;
            //}
            //for (x = 0; x < ddlArrayId.length; x++) {
            //    if (ddlArrayText[x] == agrupador.Descripcion) {
            //        $("#categoriaIngredienteId").val(ddlArrayId[x]);
            //    }
            //}
            //$("#categoriaIngredienteId").val(agrupador.Descripcion);
            //document.getElementById("categoriaIngredienteId").disabled = true;
        }



        $(`input[name='PuedeAgregarExtra'][value='${agrupador.Extras}']`).prop("checked", true);
        $(`input[name='DebeConfirmarPorSeparado'][value='${agrupador.PorSeparado}']`).prop("checked", true);

        if (agrupador.Opciones && agrupador.Opciones.some(c => c)) {
            agrupador.Opciones.map(c => $(`#Opciones option[value='${c}']`).prop("selected", true));
        }
        $("#Opciones").bootstrapDualListbox("refresh");
        if ($('input[name="PuedeAgregarExtra"]:checked').val() === "1") {
            $("#row-precio").removeClass("hide");
        } else {
            $("#row-precio").addClass("hide");
        }

    } else {
        $("#modal-add-agrupador .modal-title").text("Agregar configuración");
        $("#Id").val("");
        $("#Descripcion").val("");
        $("#Cantidad").val("");
        $("#Indice").val("");
        $("#CostoExtra").val("0.00");



        $("input[name='PuedeAgregarExtra'][value='0']").prop("checked", true);
        $("input[name='DebeConfirmarPorSeparado'][value='0']").prop("checked", true);
        var ddl = document.getElementById('categoriaIngredienteId');
        if (ddl != null) {
            $('#categoriaIngredienteId').val('');
            document.getElementById("categoriaIngredienteId").disabled = false;
            document.getElementById("Descripcion").disabled = false;
            $("#Descripcion").addClass('hidden');
            $("#categoriaIngredienteId").removeClass('hidden');
            $("#Opciones").empty();
        }


    }
    $("#Opciones").bootstrapDualListbox("refresh");
    $("#modal-add-agrupador").modal();
}

function LoadEditarAgrupador(id, desc) {
    var url = $("#url-get-agrupador").val();
    var payload = {
        id
    };

    var ddl = document.getElementById('categoriaIngredienteId');
    if (ddl != null) {
        $.ajax({
            type: "POST",
            url: $('#datos-url').val(),
            dataType: 'json',
            data: { nombre: desc }
        }).done(function (response) {
            if (response != null && response.exito) {
                $("#Opciones").empty();
                var select = document.getElementById("Opciones");
                for (x = 0; x < response.data.length; x++) {
                    var option = document.createElement("option");
                    option.text = "" + response.data[x].nombre;
                    option.value = response.data[x].idInsumo;
                    select.add(option);
                }
                $("#Opciones").bootstrapDualListbox("refresh");
            }
            else {
                alert("Ocurrio un error, contacte a su administrador.");
            }
        });
    }
    $.post(url, payload)
        .done(res => ShowModalAgrupador(res))
        .fail(notifyFailed);

    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar el agrupador por favor intente de nuevo", "error", false);
    }
}


function ShowModalEliminar(id, desc) {
    $("#id-eliminar-config").val(id);
    $("#desc").text(desc);
    $("#modal-eliminar-config").modal();
}

function SetUpModal() {
    $("#button-add-agrupador").on("click", () => ShowModalAgrupador({}));
    $(".trigger-edit-agrupador").on("click", (ev) => LoadEditarAgrupador($(ev.currentTarget).data("id"), $(ev.currentTarget).data("desc")));
    $(".trigger-delete-agrupador").on("click", (ev) => ShowModalEliminar($(ev.currentTarget).data("id"), $(ev.currentTarget).data("desc")));
    $('input[type="radio"][name="PuedeAgregarExtra"]').on("change",
        function () {
            if ($('input[name="PuedeAgregarExtra"]:checked').val() === "1") {
                $("#row-precio").removeClass("hide");
            } else {
                $("#row-precio").addClass("hide");
            }
        });
}

function SearchUnidadMinima(isEditar, insumoId) {
    var url = $("#url-search-unidad-minima").val();
    
    var payload = {};
    if (isEditar) {
        payload.insumo = insumoId;
    } else {
        payload.insumo = $("#insumoDetalle").val();
        $('#insumoId').val(payload.insumo);
    }

    if (!payload.insumo) return;
    InitLoader();
    $.post(url, payload).done(updateUnidad).fail(notifyFailed);

    function updateUnidad(res) {
        $('#unidad').val(res.data);
        $("#medida-descripcion").text(res.nombre);
        ShutDownLoader();
    }

    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar la unidad de medida mínima por favor intente de nuevo", "error", false);
        ShutDownLoader();
    }
}

function HandleTipoChange() {
    var $tipo = $("#TipoProducto");
    if (!$("#form-create").psly().validate()) {
        $tipo.val(tipo);
        return;
    }
    $("#modal-confirmar-cambio-tipo").modal();
}

function ConfirmarCambioTipo() {
    $("#ForceRefresh").val("True");
    $("#form-create").submit();
}

function RevertirCambioTipo() {
    $("#TipoProducto").val(tipo);
    $("#modal-confirmar-cambio-tipo").modal("hide");
}

