function OnScriptsLoad() {
    $('#summernote').summernote({
        lang: 'es-ES',
        toolbar: [
            ['para', ['style', 'ul', 'ol']],
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['misc', ['fullscreen', 'undo', 'redo']],
        ],
        disableDragAndDrop: true,
        minHeight: 200,
        height: 400
    });
    var original = document.createElement('blockquote');
    var t1 = document.createTextNode($('#encabezado-respuesta').val());
    var t2 = document.createTextNode($('#mensaje-original').val());
    var lineBreak = document.createElement('br');
    original.appendChild(t1);
    original.appendChild(lineBreak);
    original.appendChild(t2);
    $('#summernote').summernote('insertNode', original);
}