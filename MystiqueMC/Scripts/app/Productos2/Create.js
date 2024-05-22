var $form;
var $image;
var cropper;
var filetype = "image/png";

function OnScriptsLoad() {
    $form = $("#form-create");
    SetUpImageInput();
    SetUpImageCropper();
    SetUpImageInputVariedad();
    //HideVariedades();
    //$("#TipoProducto").on("change", HideVariedades);
    $("#btn-guardar-variedad").on("click", SaveVariedad);
    $("#btn-guardar").on("click",
        () => {
            EnviarForma();
        });

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
    $("#es-ensalada-div").removeClass("hide");
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

function SetUpImageCropper() {
    $image = $("#images");
    $image.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
    cropper = $image.data("cropper");
}

function EnviarForma(evt) {
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
    $("#Imagen").val(image);
    $form.submit();
}

function HideVariedades() {
    if ($("#TipoProducto").val() === "1") {
        $("#row-variedades").removeClass("hide");
    } else {
        $("#row-variedades").addClass("hide");
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

function SaveVariedad() {
    $("#modal-agregar-variedad").modal("hide");
    //$("#form-agregar-varidad")
}