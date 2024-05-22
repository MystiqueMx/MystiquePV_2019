function OnScriptsLoad() {
    SetUpDualSelectList();
}
function SetUpDualSelectList() {
    $('.dual_select').bootstrapDualListbox({
        nonSelectedListLabel: 'Comercios',
        selectedListLabel: 'Comercios asignados',
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