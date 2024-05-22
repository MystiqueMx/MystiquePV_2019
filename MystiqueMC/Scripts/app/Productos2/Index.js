var UploadImageUrl = '';
var hasFile = false;
var $image;
var $form;
var cropper;
var idRecetaAbierta;
window.InsumoReceta = [];
var modaltitle = "Agregar Receta Producto";
var modaltitle2 = "Agregar Insumo Receta";
function OnScriptsLoad()
{
    $("#btn-agregar-insumo-receta").on("click", () => AgregarInsumoReceta());
    $("#btn-cancelar-insumo-producto").on("click",
        () => {
            $("#modal-detalle-receta").modal("hide");
            $("#modal-receta").modal();
        });
    $("#btn-guardar-insumo-producto").on("click", () => AgregarDetalleReceta());
    $(".trigger-editar-receta").on("click", ev => {
        modaltitle = $(ev.currentTarget).data("desc");
        modaltitle2 = $(ev.currentTarget).data("desc2");
        EditarReceta(null, ev);
    });
    $("#btn-guardar-receta").on("click", () => ActualizarReceta());

    SetUpDataTableCatalogos();
    $("#modal-estatus-activo").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var nombre = $button.data("nombre");
            var idProyecto = $button.data("idproyecto");

            $("#modal-estatus-activo #producto").text(nombre);
            $("#modal-estatus-activo #idProducto").val(idProyecto);
        });

    $("#modal-estatus-inactivo").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var nombre = $button.data("nombre");
            var idProyecto = $button.data("idproyecto");

            $("#modal-estatus-inactivo #producto").text(nombre);
            $("#modal-estatus-inactivo #idProducto").val(idProyecto);
        });

    $("#modal-agregar-detalle").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var idProyecto = $button.data("id");

            $("#modal-agregar-detalle #idProducto").val(idProyecto);
        });

    $('#btn-guardar').on('click', EnviarForma);
    $form = $('#form-agregar-detalle');
    SetUpImageInput();
    SetUpImageCropper();
    SetUpDualSelectList();
}

function SetUpImageInput() {
    var $inputImage = $("#inputImage");
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
                hasFile = true;
                fileReader.onload = function () {
                    $inputImage.val("");
                    $image.cropper("reset", true).cropper("replace", this.result);
                };
            } else {
                showMessage("Please choose an image file.");
            }
        });
    } else {
        $inputImage.addClass("hide");
    }
}

function SetUpImageCropper() {
    $image = $('#image');
    $image.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
    cropper = $image.data('cropper');
}

function SetUpDualSelectList() {
    $('.dual_select').bootstrapDualListbox({
        nonSelectedListLabel: 'Clientes',
        selectedListLabel: 'Clientes asignados',
        preserveSelectionOnMove: 'moved',
        infoTextEmpty: 'Sin información',
        filterTextClear: 'Ver todos',
        filterPlaceHolder: 'Buscar...',
        moveSelectedLabel: 'Mover',
        moveAllLabel: 'Mover todos',
        removeSelectedLabel: 'Remover',
        removeAllLabel: 'Remover todos',
        infoText: 'Opciones: {0}',
        infoTextFiltered: '<span class="label label-info">Filtrando</span> {0} de {1}',
        infoTextEmpty: 'Ninguna opción'
    });
}

function EnviarForma(evt) {
    $form.parsley().validate();
    $form.parsley().whenValid().then(function () {
        if (hasFile) { // Si no se cambiara la imagen enviar vacio
            StartSpinner()
            $image.cropper('getCroppedCanvas', {
                width: 1024,
                height: 768,
                minWidth: 1024,
                minHeight: 768,
                maxWidth: 1024,
                maxHeight: 512,
                imageSmoothingQuality: 'high',
            }).toBlob(function (blob) {
                SendCroppedImage(blob)
            });
        } else {
            $form.submit();
        }
    });


    function SendCroppedImage(blob) {
        var formData = new FormData();
        formData.append('logo_menu.png', blob);
        $.ajax({
            url: $('#image-upload-url').val(),
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
        }).fail(SendFailImageAlert)
            .done(SendFormData);
    }
    function SendFormData(res) {
        if (res.success) {
            $('#FotoCliente').val(res.fileUrl);
            $form.submit();
        } else {
            SendFailImageAlert(res.message);
            StopSpinner();
        }
    }
    function SendFailImageAlert(e) {
        console.error('error ' + e);
        StopSpinner();
    }
}
function StartSpinner() {
    $('#spinner').show()
    $('#btn-guardar').hide()
}
function StopSpinner() {
    $('#spinner').hide()
    $('#btn-guardar').show()
}

function AbrirModalReceta(receta) {
    window.InsumoReceta = [];
    $("#form-receta-producto").psly().reset();
    if (receta.id) {
        idRecetaAbierta = receta.id;
        $("#id").val(receta.id);
        $("#id-eliminar").val(receta.id);
        $("#merma").val("0");
        $("#caducidad").val("0");
        $("#descripcion").val(receta.descripcion);
        $("#desc").text(receta.descripcion);
        $("#productoId").val(receta.productoId);

        window.InsumoReceta = receta.insumos;
        $("#btn-eliminar-receta").removeClass("hide");
        $("#title-1").text(modaltitle);
    } else {
        $("#id").val("");
        $("#id-eliminar").val("");
        $("#merma").val("0");
        $("#caducidad").val("0");
        $("#descripcion").val(receta.desc);
        $("#desc").text(receta.desc);
        $("#productoId").val(receta.producto);

        $("#btn-eliminar-receta").addClass("hide");
        $("#title-1").text(modaltitle);
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
    if (idRecetaAbierta <= 0) {
        $("#idReceta").val(idRecetaAbierta);
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
        idRecetaAbierta = response.id;
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

        $("#modal-detalle-receta .modal-title").text(modaltitle2);
    } else {
        $("#insumoDetalle").selectpicker("val", "");
        $("#row-insumo").removeClass("hide");

        $("#unidad").selectpicker("val", "");
        $("#cantidad").val("");

        $("#modal-detalle-receta .modal-title").text(modaltitle2);
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

function EditarReceta(id, event) {
    if (id) {
        InitLoader();
        var url = $("#url-get-receta").val();
        var payload = {
            id: id
        };
        idRecetaAbierta = id;
        $.post(url, payload)
            .done(res => handleResponse(res))
            .fail(notifyFailed)
            .always(ShutDownLoader);
    } else {
        const $target = $(event.currentTarget);
        const producto = $target.data("id");
        const desc = $target.data("desc");
        const receta = $target.data("receta");
        idRecetaAbierta = receta;
        if (receta > 1) {
            InitLoader();
            var url2 = $("#url-get-receta").val();
            var payload2 = {
                id: receta
            };
            $.post(url2, payload2)
                .done(res => handleResponse(res))
                .fail(notifyFailed)
                .always(ShutDownLoader);

        } else {
            AbrirModalReceta({ producto, desc });
        }
    }
    
    
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
        EditarReceta(idRecetaAbierta);
    }
    function notifyFailed() {
        $("#modal-detalle-receta").modal("hide");
        $("#modal-receta").modal();
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
function DeshabilitarInsumos(idSelect, insumosSeleccionados) {
    $(idSelect).find("option").prop("disabled", false);
    if (insumosSeleccionados && insumosSeleccionados.some(c => c)) {
        insumosSeleccionados.map(c => $(idSelect).find(`option[value="${c.insumoId}"]`).prop("disabled", true));
    }
    $(idSelect).selectpicker("refresh");
}
