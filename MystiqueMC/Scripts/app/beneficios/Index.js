var UploadImageUrl = ''
var $image;
var $form;
var cropper;
function OnScriptsLoad() {
    $('#modal-eliminar').on('show.bs.modal', OnDeleteModalOpen);
    $('#modal-eliminara').on('show.bs.modal', OnDeleteModalOpen);
}
function OnDeleteModalOpen(event) {
    var sender = $(event.relatedTarget);
    var modal = $(this);

    var id = sender.data('id');
    var header = sender.data('nombre');

    modal.find('#IdItemEliminar').val(id);
    modal.find('#confirm-item').text(header);
}