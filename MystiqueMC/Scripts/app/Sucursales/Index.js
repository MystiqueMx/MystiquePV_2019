var $image;
var $imageEdit;
var $form;
var $formEdit;
var cropper;
var filetype = "image/png";

window.onload =  function OnScriptsLoad() {
    $form = $("#modal-configuracion-caja #form-configuracion-caja");
    $formEdit = $("#modal-configuracion-caja-edit #form-configuracion-caja-edit");
    SetUpImageInput();  
    SetUpImageInputEdit();
    SetUpImageCropper(); 

    $("#btn-guardar").on("click", EnviarForma);
    $("#btn-guardar-edit").on("click", EnviarFormaEdit);

    $('#modal-eliminar').on('show.bs.modal', OnDeleteModalOpen);

    $("#modal-configuracion-caja").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var sucursal = $button.data("id");
            var nombreSucursal = $button.data("nombre");

            $("#modal-configuracion-caja #sucursalId").val(sucursal);
            $("#modal-configuracion-caja #nombre").text(nombreSucursal);
        });

    $("#modal-configuracion-caja-edit").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var sucursal = $button.data("id");
            var montoMaximo = $button.data("maximo");
            var costoEnvio = $button.data("costo");
            var Imagen = $button.data("imagen");
            var Mensaje = $button.data("mensaje");
            var Iva = $button.data("iva");
            var nombreSucursal = $button.data("nombre");

            $("#modal-configuracion-caja-edit #sucursalId").val(sucursal);
            $("#modal-configuracion-caja-edit #MaximoEfectivo").val(montoMaximo);
            $("#modal-configuracion-caja-edit #CostoEnvio").val(costoEnvio);
            $("#modal-configuracion-caja-edit #Imagen").val(Imagen);
            $("#modal-configuracion-caja-edit #imagesEditar").attr("src", Imagen);
            $("#modal-configuracion-caja-edit #Iva").val(Iva);
            $("#modal-configuracion-caja-edit #MensajeTicket").val(Mensaje);
            $("#modal-configuracion-caja-edit #nombre").text(nombreSucursal);

        });
   
}

function OnDeleteModalOpen(event) {
    var sender = $(event.relatedTarget);
    var modal = $(this);

    var id = sender.data('id');
    var header = sender.data('nombre');

    modal.find('#IdItemEliminar').val(id);
    modal.find('#confirm-item').text(header);
}

function SetUpImageInput() {
    var $inputImage = $("#modal-configuracion-caja #inputImages");
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
}

function SetUpImageInputEdit() {
    var $inputImage = $("#modal-configuracion-caja-edit #inputImagesEditar");
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
                    $("#imagesEditar")
                        .cropper("reset", true)
                        .cropper("replace", this.result);
                };
            } else {
                console.error("Error SetUpImageInput");
            }
        });
    } else {
        $inputImage.addClass("hide");
    }
}

function SetUpImageCropper() {
    $image = $("#modal-configuracion-caja #images");
    $image.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
    cropper = $image.data("cropper");

    $imageEdit = $("#modal-configuracion-caja-edit #imagesEditar");
    $imageEdit.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });

    cropper = $imageEdit.data("cropper");
}



function EnviarForma(evt) {
    $form.parsley().validate();
    if (!$form.parsley().isValid()) return;
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
    $("#modal-configuracion-caja #Imagen").val(image);
    $form.submit();
}

function EnviarFormaEdit(evt) {
    if (!$formEdit.parsley().isValid()) return;
    var image = $imageEdit.cropper("getCroppedCanvas", {
        width: 200,
        height: 200,
        minWidth: 200,
        minHeight: 200,
        maxWidth: 200,
        maxHeight: 200,
        fillColor: "#fff",
        imageSmoothingEnabled: false
    }).toDataURL(filetype);
    $("#modal-configuracion-caja-edit #Imagen").val(image);
    $formEdit.submit();
}


