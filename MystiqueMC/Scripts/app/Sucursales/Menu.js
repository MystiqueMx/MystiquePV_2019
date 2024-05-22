function OnScriptsLoad() {

    $("#modal-estatus-activo").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var nombre = $button.data("nombre");
            var id = $button.data("id");
            var comercio = $button.data("comercio");
            var sucursal = $button.data("sucursal");

            $("#modal-estatus-activo #producto").text(nombre);
            $("#modal-estatus-activo #idSucursalProducto").val(id);
            $("#modal-estatus-activo #sucursalId").val(sucursal);
            $("#modal-estatus-activo #IdC").val(comercio);
        });

    $("#modal-estatus-inactivo").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var nombre = $button.data("nombre");
            var id = $button.data("id");
            var sucursal = $button.data("sucursal");
            var comercio = $button.data("comercio");

            $("#modal-estatus-inactivo #producto").text(nombre);
            $("#modal-estatus-inactivo #idSucursalProducto").val(id);
            $("#modal-estatus-inactivo #sucursalId").val(sucursal);
            $("#modal-estatus-inactivo #IdC").val(comercio);
        });

    $(document).ready(function () {
        $('.botton-ac').on('click', function (evento) {
            var $contenedor = $(evento.target).parent('.control-orden');
            var id = $contenedor.find('input[name = "id"]').val();
            var precios = $contenedor.find('input[name = "precio"]').val();
            var parametro = { id: id, precio: precios };
            console.log(parametro);
            $.ajax({
                type: "POST",
                url: $('#guardarPrecio').val(),
                dataType: 'json',
                data: parametro
            }).done(function (response) {
                if (response != null && response.exito) {
                    try {
                        window.location.reload();
                    }
                    catch (err) {
                        console.error(err);
                    }
                }
                else {
                    alert("Ocurrio un error, contacte a su administrador.");
                }
            });
        });
    });

    $(document).ready(function () {

        $('#ContentTable').dataTable({
            "dom": 'T<"clear">lfrtip',
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

    });
}