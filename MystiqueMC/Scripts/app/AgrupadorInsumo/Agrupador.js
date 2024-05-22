function OnScriptsLoad() {
    $("#modal-activar-agrupador").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var id = $button.data("id");
            var idagrupador = $button.data("idagrupador");
            var descripcion = $button.data("descripcion");

            $("#modal-activar-agrupador #id").val(id);
            $("#modal-activar-agrupador #descripcion").text(descripcion);
        });

    $("#modal-inactivar-agrupador").on("show.bs.modal",
        function (event) {
            var $button = $(event.relatedTarget);
            var id = $button.data("id");
            var descripcion = $button.data("descripcion");

            $("#modal-inactivar-agrupador #id").val(id);
            $("#modal-inactivar-agrupador #descripcion").text(descripcion);
        });

    $(".delete-insumo-agrupador").on("click", (ev) => EliminarAgrupadorPost(Number($(ev.currentTarget).data('id'))));
}
function EliminarAgrupadorPost(id) {
    const url = $("#url-eliminar-insumo").val();

    InitLoader();

    $.post(url, { idConteoFisicoAgrupadorInsumos: id})
        .done(notifyResponse)
        .fail(notifyError)
        .always(() => ShutDownLoader());

    function notifyResponse(res) {
        $('#row-' + id).remove();
    }
    function notifyError() {
        SendAlert("Ha ocurrido un error al eliminar el insumo, por favor intente de nuevo más tarde", "error", true);
    }
}