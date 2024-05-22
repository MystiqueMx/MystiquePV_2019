var selectize;
var isDeleting;

function OnScriptsLoad() {
    SetUpSelectize();
    SetUpDualSelectList();
    new EmojiPicker();
}

function SetUpDualSelectList() {
    $('select[name="sucursales"]').bootstrapDualListbox({
        nonSelectedListLabel: 'Sucursales',
        selectedListLabel: 'Sucursales seleccionadas',
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

function SetUpSelectize() {
    $('select[name="sector"]').selectize({
        maxItems: null,
        hideSelected: true,
        placeholder: 'Ninguno seleccionado',
        selectOnTab: true,
        render: {
            item: function (item) {
                if (item.value.includes('az'))
                    return `<span class="badge badge-danger" style="margin:1px 2px 1px 2px;">${item.text}</span>`
                else if (item.value.includes('sx'))
                    return `<span class="badge badge-primary" style="margin:1px 2px 1px 2px;">${item.text}</span>`
                else if (item.value.includes('su'))
                    return `<span class="badge badge-info" style="margin:1px 2px 1px 2px;">${item.text}</span>`
                else
                    return `<span class="badge badge-success" style="margin:1px 2px 1px 2px;">${item.text}</span>`
            },
            option: function (option) {
                if (option.value.includes('az'))
                    return `<span class="badge badge-danger m-xs">${option.text}</span>`
                else if (option.value.includes('sx'))
                    return `<span class="badge badge-primary m-xs">${option.text}</span>`
                else if (option.value.includes('su'))
                    return `<span class="badge badge-info m-xs">${option.text}</span>`
                else
                    return `<span class="badge badge-success m-xs">${option.text}</span>`
            }
        },
        plugins: ['remove_button'],
        onItemAdd: function (value, $item) {
            if (!isDeleting) {
                if (value === 'az-0') {
                    isDeleting = true;
                    selectize.clear(true);
                    selectize.addItem(value, true);
                    isDeleting = false;
                } else {
                    if (selectize.items.filter(c => c === 'az-0').length >0) {
                        selectize.removeItem(value, true);
                        selectize.refreshItems();
                    }
                }
            }
            
        },
        onItemRemove: function (value) {
        }
    });
    selectize = $('select[name="sector"]')[0].selectize;
}