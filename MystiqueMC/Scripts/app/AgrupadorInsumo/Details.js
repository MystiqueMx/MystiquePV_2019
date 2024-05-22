function OnScriptsLoad() {
    $("#modal-activar").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var id = $button.data("id");
            var idagrupador = $button.data("idagrupador");
            var descripcion = $button.data("descripcion");

            $("#modal-activar #id").val(id);
            $("#modal-activar #idAgrupador").val(idagrupador);
            $("#modal-activar #descripcion").text(descripcion);
        });

    $("#modal-inactivar").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var id = $button.data("id");
            var idagrupador = $button.data("idagrupador");
            var descripcion = $button.data("descripcion");

            $("#modal-inactivar #id").val(id);
            $("#modal-inactivar #idAgrupador").val(idagrupador);
            $("#modal-inactivar #descripcion").text(descripcion);
        });
}