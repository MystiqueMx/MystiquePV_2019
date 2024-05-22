var $image;
var $imageEditar;
var $form;
var $formEditar;
var cropper;
var filetype = "image/png";

function OnScriptsLoad() {
    
    $form = $("#form-agregar-varidad");
    $formEditar = $("#form-editar-varidad");
    SetUpImageInput();
    SetUpImageInputEditar();
    SetUpImageCropper();

    $("#btn-guardar-variedad").on("click", EnviarForma);
    $("#btn-guardar-edit").on("click", EnviarFormaEditar);
    $(".trigger-edit-variedad").on("click", showModalEditVariedad);
    $(".trigger-eliminar-variedad").on("click", showModalEliminar);
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
}

function SetUpImageInputEditar() {
    var $inputImage = $("#inputImagesEditar");
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
    $image = $("#images");
    $image.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
    cropper = $image.data("cropper");

    $imageEditar = $("#imagesEditar");
    $imageEditar.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
}


function EnviarForma() {
    if (!$form.parsley().validate()) return;
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
    $("#ImagenAgregar").val(image);
    $form.submit();
}
function showModalEditVariedad(event) {
    var variedad = $(event.currentTarget).data("id");
    var descripcion = $(event.currentTarget).data("descripcion");
    var imagen = $(`#imagen-${variedad}`).attr("src");

    $("#IdVariedadProductoEditar").val(variedad);
    $("#DescripcionEditar").val(descripcion);
    $imageEditar
        .cropper("reset", true)
        .cropper("replace", imagen);
    $("#modal-editar-variedad").modal();
}

function EnviarFormaEditar() {
    if (!$formEditar.parsley().validate()) return;
    var image = $("#imagesEditar").cropper("getCroppedCanvas", {
        width: 200,
        height: 200,
        minWidth: 200,
        minHeight: 200,
        maxWidth: 200,
        maxHeight: 200,
        fillColor: "#fff",
        imageSmoothingEnabled: false
    }).toDataURL(filetype);
    $("#ImagenEditar").val(image);
    $formEditar.submit();
}

function showModalEliminar(event) {
    var $current = $(event.currentTarget);
    $("#idProducto").val($current.data("idproducto"));
    $("#idVariedad").val($current.data("id"));
    $("#producto").text($current.data("descripcion"));
    $("#modal-eliminar").modal();
}