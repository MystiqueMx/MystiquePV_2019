function OnScriptsLoad() {
    SetUpDualSelect();
    SetUpModal();
}

function SetUpDualSelect() {
    $("#Opciones").bootstrapDualListbox({
        nonSelectedListLabel: "No asignados",
        selectedListLabel: "Asignados",
        preserveSelectionOnMove: "moved",
        infoTextEmpty: "Sin información",
        filterTextClear: "Ver todos",
        filterPlaceHolder: "Buscar...",
        moveSelectedLabel: "Mover",
        moveAllLabel: "Mover todos",
        removeSelectedLabel: "Remover",
        removeAllLabel: "Remover todos",
        infoText: "Opciones: {0}",
        infoTextFiltered: '<span class="label label-info">Filtrando</span> {0} de {1}'
    });
}

function ShowModalAgrupador(agrupador) {
    
    $("#Opciones option").prop("selected", false);
    if (agrupador.Id) {
        $("#modal-add-agrupador .modal-title").text("Editar agrupador");
        $("#Id").val(agrupador.Id);
        $("#Descripcion").val(agrupador.Descripcion);
        $("#Cantidad").val(agrupador.Cantidad);
        $("#Indice").val(agrupador.Indice);
        $("#CostoExtra").val(agrupador.CostoExtra);

        
       
        var ddlArrayText = new Array();
        var ddlArrayId = new Array();
        var ddl = document.getElementById('categoriaIngredienteId');
        if (ddl != null) {
            document.getElementById("Descripcion").disabled = true;
            $("#Descripcion").removeClass('hidden');
            $("#categoriaIngredienteId").addClass('hidden');
            //for (i = 1; i < ddl.options.length; i++) {
            //    ddlArrayText[i - 1] = ddl.options[i].innerText;
            //    ddlArrayId[i - 1] = ddl.options[i].value;
            //}
            //for (x = 0; x < ddlArrayId.length; x++) {
            //    if (ddlArrayText[x] == agrupador.Descripcion) {
            //        $("#categoriaIngredienteId").val(ddlArrayId[x]);
            //    }
            //}
            //$("#categoriaIngredienteId").val(agrupador.Descripcion);
            //document.getElementById("categoriaIngredienteId").disabled = true;
        }


        
        $(`input[name='PuedeAgregarExtra'][value='${agrupador.Extras}']`).prop("checked", true);
        $(`input[name='DebeConfirmarPorSeparado'][value='${agrupador.PorSeparado}']`).prop("checked", true);
        
        if (agrupador.Opciones && agrupador.Opciones.some(c => c)) {
            agrupador.Opciones.map(c => $(`#Opciones option[value='${c}']`).prop("selected", true));
        }
        $("#Opciones").bootstrapDualListbox("refresh");
        if ($('input[name="PuedeAgregarExtra"]:checked').val() === "1") {
            $("#row-precio").removeClass("hide");
        } else {
            $("#row-precio").addClass("hide");
        }

    } else {
        $("#modal-add-agrupador .modal-title").text("Agregar agrupador");
        $("#Id").val("");
        $("#Descripcion").val("");
        $("#Cantidad").val("");
        $("#Indice").val("");
        $("#CostoExtra").val("0.00");

       

        $("input[name='PuedeAgregarExtra'][value='0']").prop("checked", true);
        $("input[name='DebeConfirmarPorSeparado'][value='0']").prop("checked", true);
        var ddl = document.getElementById('categoriaIngredienteId');
        if (ddl != null) {
            $('#categoriaIngredienteId').val('');
            document.getElementById("categoriaIngredienteId").disabled = false;
            document.getElementById("Descripcion").disabled = false;
            $("#Descripcion").addClass('hidden');
            $("#categoriaIngredienteId").removeClass('hidden');
            $("#Opciones").empty();
        }
      
       
    }
    $("#Opciones").bootstrapDualListbox("refresh");
    $("#modal-add-agrupador").modal();
}

function LoadEditarAgrupador(id, desc) {
    var url = $("#url-get-agrupador").val();
    var payload = {
        id
    };

    var ddl = document.getElementById('categoriaIngredienteId');
    if (ddl != null) {
        $.ajax({
            type: "POST",
            url: $('#datos-url').val(),
            dataType: 'json',
            data: { nombre: desc }
        }).done(function (response) {
            if (response != null && response.exito) {
                $("#Opciones").empty();
                var select = document.getElementById("Opciones");
                for (x = 0; x < response.data.length; x++) {
                    var option = document.createElement("option");
                    option.text = "" + response.data[x].nombre;
                    option.value = response.data[x].idInsumo;
                    select.add(option);
                }
                $("#Opciones").bootstrapDualListbox("refresh");
            }
            else {
                alert("Ocurrio un error, contacte a su administrador.");
            }
        });
    }
    $.post(url, payload)
        .done(res => ShowModalAgrupador(res))
        .fail(notifyFailed);

    function notifyFailed() {
        SendAlert("Ocurrió un error al actualizar el agrupador por favor intente de nuevo", "error", false);
    }
}

function ShowModalEliminar(id, desc) {
    $("#id-eliminar").val(id);
    $("#desc").text(desc);
    $("#modal-eliminar").modal();
}

function SetUpModal() {
    $("#button-add-agrupador").on("click", () => ShowModalAgrupador({}));
    $(".trigger-edit-agrupador").on("click", (ev) => LoadEditarAgrupador($(ev.currentTarget).data("id"), $(ev.currentTarget).data("desc")));
    $(".trigger-delete-agrupador").on("click", (ev) => ShowModalEliminar($(ev.currentTarget).data("id"), $(ev.currentTarget).data("desc")));
    $('input[type="radio"][name="PuedeAgregarExtra"]').on("change",
        function() {
            if ($('input[name="PuedeAgregarExtra"]:checked').val() === "1") {
                $("#row-precio").removeClass("hide");
            } else {
                $("#row-precio").addClass("hide");
            }
        });
}