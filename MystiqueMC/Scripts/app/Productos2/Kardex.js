var UploadImageUrl = '';
var hasFile = false;
var $image;
var $images;
var $imagesDetalle;
var $form;
var $form1;
var $formDetalle;
var cropper;

function OnScriptsLoad() {
    $('#modal-agregar-variedad #btn-guardar').on('click', EnviarForma);
    $form = $('#form-agregar-varidad');
    SetUpImageInput();
    SetUpImageCropper();
    SetUpDualSelectList();

    $('#modal-editar-variedad #btn-guardar-edit').on('click', EnviarForma1);
    $form1 = $('#form-editar-varidad');
    SetUpImageInput1();
    SetUpImageCropper1();
    SetUpDualSelectList1();

    $("#modal-editar-variedad").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var id = $button.data("id");
            var imagen = $button.data("imagen");
            var descripcion = $button.data("descripcion");
            var url = "http://localhost:50060/";
            var img = url + imagen;
            $("#modal-editar-variedad #IdVariedadProducto").val(id);
            $("#modal-editar-variedad #Descripcion").val(descripcion);
            $("#modal-editar-variedad #image").attr("src", img);
            $("#modal-editar-variedad #FotoCliente").val(img);
        });

    $("#modal-eliminar").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var idvariedad = $button.data("id");
            var idProyecto = $button.data("idproducto");
            var descripcion = $button.data("descripcion");

            $("#modal-eliminar #producto").text(descripcion);
            $("#modal-eliminar #idProducto").val(idProyecto);
            $("#modal-eliminar #idVariedad").val(idvariedad);
        });

    $("#modal-editar-detalle").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var id = $button.data("id");
            var indice = $button.data("indice");
            var principal = $button.data("principal");
            var idDetalle = $button.data("detalle");

            $("#modal-editar-detalle #IdProducto").val(id);
            $("#modal-editar-detalle #Indice").val(indice);
            $("#modal-editar-detalle #Principal").val(principal);
            $("#modal-editar-detalle #IdDetalle").val(idDetalle);
        });
}

function SetUpImageInput() {
    var $inputImage = $("#modal-agregar-variedad #inputImages");
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
    $image = $('#modal-agregar-variedad #images');
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
            StartSpinner();
            $image.cropper('getCroppedCanvas', {
                width: 1024,
                height: 768,
                minWidth: 1024,
                minHeight: 768,
                maxWidth: 1024,
                maxHeight: 512,
                imageSmoothingQuality: 'high',
            }).toBlob(function (blob) {
                SendCroppedImage(blob);
            });
        } else {
            $form.submit();
        }
    });


    function SendCroppedImage(blob) {
        var formData = new FormData();
        formData.append('logo_menu.png', blob);
        $.ajax({
            url: $('#modal-agregar-variedad #image-upload-url-agregar').val(),
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
        }).fail(SendFailImageAlert)
            .done(SendFormData);
    }
    function SendFormData(res) {
        if (res.success) {
            $('#modal-agregar-variedad #Fotoproducto').val(res.fileUrl);
            $('#modal-agregar-variedad #bases').val(cropper.url);
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
    $('#spinner').show();
    $('#btn-guardar').hide();
}
function StopSpinner() {
    $('#spinner').hide();
    $('#btn-guardar').show();
}

function SetUpImageInput1() {
    var $inputImage = $("#modal-editar-variedad #inputImagenes");
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
                    $images.cropper("reset", true).cropper("replace", this.result);
                };
            } else {
                showMessage("Please choose an image file.");
            }
        });
    } else {
        $inputImage.addClass("hide");
    }
}

function SetUpImageCropper1() {
    $images = $('#modal-editar-variedad #image');
    $images.cropper({
        aspectRatio: 1 / 1,
        preview: ".img-preview",
        autoCropArea: 1
    });
    cropper = $images.data('cropper');
}

function SetUpDualSelectList1() {
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

function EnviarForma1(evt) {
    $form1.parsley().validate();
    $form1.parsley().whenValid().then(function () {
        if (hasFile) { // Si no se cambiara la imagen enviar vacio
            StartSpinner1();
            $images.cropper('getCroppedCanvas', {
                width: 1024,
                height: 768,
                minWidth: 1024,
                minHeight: 768,
                maxWidth: 1024,
                maxHeight: 512,
                imageSmoothingQuality: 'high',
            }).toBlob(function (blob) {
                SendCroppedImage1(blob);
            });
        } else {
            $form1.submit();
        }
    });


    function SendCroppedImage1(blob) {
        var formData = new FormData();
        formData.append('logo_menu.png', blob);
        $.ajax({
            url: $('#modal-editar-variedad #image-upload-url-agregar').val(),
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
        }).fail(SendFailImageAlert1)
            .done(SendFormData1);
    }
    function SendFormData1(res) {
        if (res.success) {
            $('#modal-editar-variedad #FotoCliente').val(res.fileUrl);
            $form1.submit();
        } else {
            SendFailImageAlert1(res.message);
            StopSpinner1();
        }
    }
    function SendFailImageAlert1(e) {
        console.error('error ' + e);
        StopSpinner1();
    }
}

function StartSpinner1() {
    $('#spinner').show();
    $('#btn-guardar-edit').hide();
}
function StopSpinner1() {
    $('#spinner').hide();
    $('#btn-guardar-edit').show();
}