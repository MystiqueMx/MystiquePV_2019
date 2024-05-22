function OnScriptsLoad() {
    $("#modal-activar-rubro").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var id = $button.data("id");
            var idrubro = $button.data("idrubro");
            var descripcion = $button.data("descripcion");

            $("#modal-activar-rubro #id").val(id);
            $("#modal-activar-rubro #descripcion").text(descripcion);
        });

    $("#modal-inactivar-rubro").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var id = $button.data("id");
            var descripcion = $button.data("descripcion");

            $("#modal-inactivar-rubro #id").val(id);
            $("#modal-inactivar-rubro #descripcion").text(descripcion);
        });
}