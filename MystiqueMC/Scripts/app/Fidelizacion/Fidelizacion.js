

$(document).ready(function () {

    var selectize;
    var isDeleting;
    ////////////////
    ////TAB LIST////
    ////////////////

    $("ul > content a").click(async function () {

        await ValidarSesion();
        $('ul > content a').removeClass('greenbuton');
        $('ul > content a').addClass('btn-tab');
        $(this).addClass('greenbuton');
        $(this).removeClass('btn-tab');
    });

    function ValidarSesion() {

        $.ajax({
            type: "GET",
            url: $("#validar-sesion-url").val(),
            success: function (result) {
                if (result.success) {
                    return true;
                } else if (!result.success) {
                    $('#form_to_login').submit();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    }
    ////////////////
    ///AJAX CALLS///
    ////////////////

    //PROMOCIONES TAB
    $("#promociones_button").on("click", async function () {

        await ValidarSesion();

        $.ajax({
            type: "GET",
            url: $(this).data('request-url'),
            success: function (result) {
                if (result.success == null) {
                    $("#promociones_div").html(result);
                } else if (!result.success) {
                    // $('#form_to_login').submit();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });

    //CANJES TAB
    $("#canjes_button").on("click", function () {
        $.ajax({
            type: "GET",
            url: $(this).data('request-url'),
            success: function (result) {
                if (result.success == null) {
                    $("#canjes_div").html(result);
                } else if (!result.success) {
                    $('#form_to_login').submit();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });

    //COMENTARIOS TAB
    $("#comentarios_button").on("click", function () {
        $.ajax({
            type: "GET",
            url: $(this).data('request-url'),
            success: function (result) {
                if (result.success == null) {
                    $("#comentarios_div").html(result);

                    $('#btn-marcar-leido').on('click', MarcarLeidos);
                    $('#btn-marcar-importante').on('click', MarcarImportantes);
                    $('#btn-marcar-eliminado').on('click', MarcarEliminados);
                } else if (!result.success) {
                    $('#form_to_login').submit();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });

    //NOTIFICACIONES TAB
    $("#notificaciones_button").on("click", function () {
        $.ajax({
            type: "GET",
            url: $(this).data('request-url'),
            success: function (result) {
                if (result.success == null) {
                    $("#notificaciones_div").html(result);
                    SetUpDualSelectList();
                    SetUpSelectize();
                    new EmojiPicker();
                } else if (!result.success) {
                    $('#form_to_login').submit();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });

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
                        if (selectize.items.filter(c => c === 'az-0').length > 0) {
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


    //SOPORTE TAB
    $("#soporte_button").on("click", async function () {

        await ValidarSesion();

        $.ajax({
            type: "GET",
            url: $(this).data('request-url'),
            success: function (result) {
                if (result.success == null) {
                    $("#soporte_div").html(result);

                    $("#telefonoContacto").inputmask("mask", { "mask": "(999) 999-99-99" });

                    $("#EditarButton_soporte").click(function () {
                        $('#mydiv').find('input, textarea, button, select').prop('disabled', false);
                        $('#EditarButton_soporte').setAttribute("disabled", "disabled");
                    });
                } else if (!result.success) {
                    $('#form_to_login').submit();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });


    });

    ////////////////
    ////       /////
    ////////////////

    //Select tab nav
    if (tabID != '') {
        $('#' + tabID).click();
    } else {
        $('#fidelizacion_button').click();
    }

    ///////////////////
    //NOTIFICACIONES

    function ObtenerIdSeleccionados() {
        var $rowsMarcadas = $('.email-row').has('input[name="IsSelected"]:checked')
            .find('input[name="IdComentario"]');
        var Ids = []
        $rowsMarcadas.each(function (idx, el) {
            Ids.push($(el).val())
        })
        return Ids;
    }

    function MarcarEliminados() {
        var IdsSeleccionados = ObtenerIdSeleccionados();
        if (IdsSeleccionados.length > 0) {
            console.log(IdsSeleccionados);
            $.ajax({
                url: $('#url-marcar-eliminados').val(),
                method: "POST",
                data: { IdComentarios: IdsSeleccionados },
                dataType: "json",
            })
                .fail(SendFailAlert)
                .done(function (res) {
                    if (res.success) {
                        $("#comentarios_button").click();
                        // window.location.reload();
                    } else {
                        SendFailAlert();
                    }
                });
        }
    }

    function MarcarImportantes() {
        var IdsSeleccionados = ObtenerIdSeleccionados();
        if (IdsSeleccionados.length > 0) {
            console.log(IdsSeleccionados);
            $.ajax({
                url: $('#url-marcar-importantes').val(),
                method: "POST",
                data: { IdComentarios: IdsSeleccionados },
                dataType: "json",
            })
                .fail(SendFailAlert)
                .done(function (res) {
                    if (res.success) {
                        $("#comentarios_button").click();
                        //window.location.reload();
                    } else {
                        SendFailAlert();
                    }
                });
        }
    }

    function MarcarLeidos() {
        var IdsSeleccionados = ObtenerIdSeleccionados();
        if (IdsSeleccionados.length > 0) {
            console.log(IdsSeleccionados);
            $.ajax({
                url: $('#url-marcar-leidos').val(),
                method: "POST",
                data: { IdComentarios: IdsSeleccionados },
                dataType: "json",
            })
                .fail(SendFailAlert)
                .done(function (res) {
                    if (res.success) {
                        $("#comentarios_button").click();
                        //window.location.reload();
                    } else {
                        SendFailAlert();
                    }
                });
        }
    }

    function SendFailAlert() {
        console.error('error al guardar los comentarios')
    }






});