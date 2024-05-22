const es_locale = {
    strings: {
        chooseFile: "Selecciona una imagen",
        youHaveChosen: "Has seleccionado: %{fileName}",
        orDragDrop: "o arrástralo aquí",
        filesChosen: {
            0: "%{smart_count} imagen seleccionada",
            1: "%{smart_count} imágenes seleccionadas"
        },
        filesUploaded: {
            0: "%{smart_count} imagen subida",
            1: "%{smart_count} imágenes subidas"
        },
        files: {
            0: "%{smart_count} imagen",
            1: "%{smart_count} imágenes"
        },
        uploadFiles: {
            0: "Subir %{smart_count} imagen",
            1: "Subir %{smart_count} imágenes"
        },
        selectToUpload: "Selecciona las imágenes a subir",
        closeModal: "Cerrar modal",
        upload: "Subir",
        importFrom: "Importar imagen desde",
        dashboardWindowTitle: "Panel de Uppy (Pulsa escape para cerrar)",
        dashboardTitle: "Panel de Uppy",
        copyLinkToClipboardSuccess: "Enlace copiado al portapapeles.",
        copyLinkToClipboardFallback: "Copiar la siguiente URL",
        done: "Hecho",
        localDisk: "Disco local",
        dropPasteImport: "Arrasta las imágenes aquí, pega, importa de alguno de los servicios de arriba o %{browse}",
        dropPaste: "Suelta o pega las imágenes aquí / %{browse}",
        browse: "explorar mis archivos",
        fileProgress: "Progreso: velocidad de subida y tiempo estimado",
        numberOfSelectedFiles: "Número de imágenes seleccionadas",
        uploadAllNewFiles: "Subir todos las nuevas imágenes",
        addMoreFiles: "Agregar imágenes",
        copyLink: "Copiar enlace",
        fileSource: "Origen de la imagen: %{name}",
        removeFile: "Remover imagen",
        editFile: "Editar imagen",
        editing: "Editando %{file}",
        edit: "Editar",
        finishEditingFile: "Terminar edición",
        myDevice: "Mi dispositivo",
        uploadComplete: "Carga completa",
        resumeUpload: "Resumir carga",
        pauseUpload: "Pausar carga",
        retryUpload: "Reintenter carga",
        xFilesSelected: {
            0: "%{smart_count} imagen seleccionada",
            1: "%{smart_count} imágenes seleccionadas"
        },
        uploading: "Cargando",
        complete: "Terminado",
        back: "Regresar",
        cancel: "Cancelar",
        title: "Agregar imagen",
        uploadFailed: "La carga ha fallado",
        pleasePressRetry: "Porfavor presiona reintentar para cargar de nuevo",
        paused: "Pausado",
        error: "Errir",
        retry: "Reintentar",
        pause: "Pausar",
        resume: "Resumir",
        pressToRetry: "Presiona para reintentar",
        filesUploadedOfTotal: {
            0: "%{complete} de %{smart_count} imagen completada",
            1: "%{complete} de %{smart_count} imágenes completadas"
        },
        dataUploadedOfTotal: "%{complete} de %{total}",
        xTimeLeft: "%{time} restante",
        uploadXFiles: {
            0: "Cargar %{smart_count} imagen",
            1: "Cargar %{smart_count} imágenes"
        },
        uploadXNewFiles: {
            0: "Cargar +%{smart_count} imagen",
            1: "Cargar +%{smart_count} imágenes"
        },
        xMoreFilesAdded: {
            0: "%{smart_count} imagen añadida",
            1: "%{smart_count} imágenes añadidas"
        },
        youCanOnlyUploadX: {
            0: "Solo puedes cargar %{smart_count} imagen",
            1: "Solo puedes cargar %{smart_count} imágenes"
        },
        youHaveToAtLeastSelectX: {
            0: "Debes seleccionar por lo menos %{smart_count} imagen",
            1: "Debes seleccionar por lo menos %{smart_count} imágenes"
        },
        exceedsSize: "Este archivo excede el peso maximo",
        youCanOnlyUploadFileTypes: "Solo puedes cargar imágenes JPG y PNG",
        companionError: "La conexión ha fallado",
        failedToUpload: "La carga de la imagen %{file} ha fallado",
        noInternetConnection: "No cuentas con conexión a Internet",
        connectedToInternet: "Conectado a internet",
        noFilesFound: "No existen imágenes en esta ubicación",
        selectXFiles: {
            0: "Selecciona %{smart_count} imagen",
            1: "Selecciona %{smart_count} imágenes"
        },
        logOut: "Cerrar sesión"
    }
};

function OnScriptsLoad() {
    ConfigureUppy7();

    $("#EditarButton_datosDoctor").click(function () {
        $('#mydiv').find('input, textarea, button, select, .bootstrap-select').prop('disabled', false);
        $('#mydiv').find('button').removeClass('disabled');
        $('#EditarButton_datosDoctor').prop('disabled', true);
    });

    $("#modal-eliminar-imagen").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var id = $button.data("id");
            var idcomercio = $button.data("idcomercio");
            var idempresa = $button.data("idempresa");
            var idanexo = $button.data("idanexo");

            $("#modal-eliminar-imagen #id").val(id);
            $("#modal-eliminar-imagen #idComercios").val(idcomercio);
            $("#modal-eliminar-imagen #idEmpresa").val(idempresa);
            $("#modal-eliminar-imagen #idAnexo").val(idanexo);
        });
}

function ConfigureUppy7() {
    if ($("#drag-drop-area").length === 0) return;
    var uppy = Uppy.Core({
        id: "anexos",
        autoProceed: false,
        allowMultipleUploads: true,
        debug: false,
        restrictions: {
            maxFileSize: 1e+7,
            maxNumberOfFiles: 5,
            allowedFileTypes: ["image/*", ".jpg", ".jpeg", ".png"]
        },
        meta: {},
        locale: es_locale
    });
    uppy.use(Uppy.Dashboard, {
        inline: true,
        width: 700,
        height: 300,
        target: "#drag-drop-area"
    });
    uppy.use(Uppy.XHRUpload,
        {
            endpoint: $("#url-imagenes-anexoDoctor").val(),
            timeout: 60 * 1000 * 1000,
            fieldName: "file",
            limit: 3,
            locale: {
                strings: {
                    timedOut: "La carga ha sido abortada despues de %{seconds} segundos"
                }
            }
        });

    uppy.on("file-added",
        (file) => {
            uppy.setFileMeta(file.id,
                {
                    idComercio: $("#idComercio").val()
                });
        });

    uppy.on("complete",
        (result) => {
            if (result.successful && result.successful.length > 0) {
                result.successful.map(c => {
                    updateList(c.response.body, c.meta.name);
                });
            }
        });

    function updateList(value, text) {
        if ($(".chosen-select").length > 0) {
            var $anexos = $("select[name='Anexos']");
            $anexos.append(`<option value="${value}" selected>${text}</option>`);
            $anexos.trigger("chosen:updated");
        } else {
            console.log(`throwing new doc for ${value} - ${text}`);
        }
    }
}