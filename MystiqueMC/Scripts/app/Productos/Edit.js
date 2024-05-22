var UploadImageUrl = '';
var hasFile = false;
var $image;
var $form;
var cropper;
function OnScriptsLoad() {
    $('#btn-guardar').on('click', EnviarForma)
    $form = $('#form-editar-recompensa');
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
        aspectRatio: 4 / 3,
        preview: ".img-preview",
        autoCropArea: 1
    });
    cropper = $image.data('cropper');
}

function SetUpDualSelectList() {
    $('.dual_select').bootstrapDualListbox({
        nonSelectedListLabel: 'Comercios',
        selectedListLabel: 'Comercios asignados',
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
    if (!$form.parsley().isValid()) return;
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
    
    function SendCroppedImage(blob) {
        var formData = new FormData();
        formData.append('image.png', blob);
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
            $('#ImageUrl').val(res.fileUrl);
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
