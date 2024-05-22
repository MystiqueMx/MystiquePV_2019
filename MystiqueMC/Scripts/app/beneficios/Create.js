var UploadImageUrl = ''
var $image;
var $form;
var cropper;
function OnScriptsLoad() {
    $('#btn-guardar').on('click', EnviarForma)
    $form = $('#Forma');
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
        preview: ".img-preview"
    });
    cropper = $image.data('cropper');
}

function SetUpDualSelectList() {
    $('.dual_select').bootstrapDualListbox({
        nonSelectedListLabel: 'Sucursales',
        selectedListLabel: 'Sucursales asignados',
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

    StartSpinner();

    $image.cropper('getCroppedCanvas', {
        width: 1200,
        height: 900,
        minWidth: 1200,
        minHeight: 900,
        maxWidth: 1200,
        maxHeight: 900,
        imageSmoothingQuality: 'high',
    }).toBlob(function (blob) {
        SendCroppedImage(blob)
    });
    

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
    function dataURItoBlob(dataURI) {
        // convert base64/URLEncoded data component to raw binary data held in a string
        var byteString;
        if (dataURI.split(',')[0].indexOf('base64') >= 0)
            byteString = atob(dataURI.split(',')[1]);
        else
            byteString = unescape(dataURI.split(',')[1]);

        // separate out the mime component
        var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];

        // write the bytes of the string to a typed array
        var ia = new Uint8Array(byteString.length);
        for (var i = 0; i < byteString.length; i++) {
            ia[i] = byteString.charCodeAt(i);
        }

        return new Blob([ia], { type: mimeString });
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


