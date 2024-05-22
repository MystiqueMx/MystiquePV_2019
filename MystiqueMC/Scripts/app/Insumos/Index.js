
$("#modal-estatus-activo").on("show.bs.modal", OnActivarModalOpenInsumo);
$("#modal-estatus-inactivo").on("show.bs.modal", OInactivarModalOpenInsumo);

function OnActivarModalOpenInsumo(event) {
    var sender = $(event.relatedTarget);
    var modal = $(this);

    var idProyecto = sender.data('idinsumo');
    var nombre = sender.data('nombre');


    modal.find("#insumo").text(nombre);
    modal.find("#idInsumo").val(idProyecto);
}


function OInactivarModalOpenInsumo(event) {
    var sender = $(event.relatedTarget);
    var modal = $(this);

    var idProyecto = sender.data('idinsumo');
    var nombre = sender.data('nombre');


    modal.find("#insumo").text(nombre);
    modal.find("#idInsumo").val(idProyecto);
}


function OnScriptsLoad() {

    $('#ContentTable').dataTable({
        "dom": 'T<"clear">lrtip',
        "pageLength": 100,
        "tableTools": {
            "sSwfPath": "../scripts/plugins/dataTables/swf/copy_csv_xls_pdf.swf"
        },
        "language": {
            "search": "Buscar ",
            "lengthMenu": "Elementos por página:  _MENU_",
            "info": "Mostrando _START_ - _END_ de _TOTAL_ elementos",
            "emptyTable": "No hay información",
            "paginate": {
                "first": "Primera",
                "last": "Ultima",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        }
    });

}
function OnClickEliminar(event) {

    var $button = $(event.currentTarget);
    var nombre = $button.data("nombre");
    var idProyecto = $button.data("idproyecto");
    var url = $('#UrlPuedeEliminar').val();

    $("#modal-eliminar #insumo").text(nombre);
    $("#modal-eliminar-no #insumo").text(nombre);
    $("#modal-eliminar-no #idInsumo").val(idProyecto);
    $("#modal-eliminar #idInsumo").val(idProyecto);

    consultarPuedeEliminar(idProyecto, url);

}

function consultarPuedeEliminar(Id, url) {

    payload = {
        idInsumo: Id
    };

    $.ajax({
        url: url,
        data: payload
    }).done(successHandler)
        .always(completeHandler);

    function successHandler(insumo) {

        if (insumo.success) {
            $("#modal-eliminar").modal('show');
            
        } else {
            $("#modal-eliminar-no").modal('show');
        }
    }

    console.log("eliminar pressed");

}

function completeHandler() {
    //$('.spinner').hide();
}

