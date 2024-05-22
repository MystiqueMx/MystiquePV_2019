$(document).ready(function () {
    $("#button-search").on("click", SearchFacturas);
    if (window.UrlFacturacion === "" || window.HeaderFacturación === "") SendAlert("No se encuentra configurada facturación");
});

function OnScriptsLoad() {
    $("#button-search").on("click", SearchFacturas);
    if (window.UrlFacturacion === "" || window.HeaderFacturación === "") SendAlert("No se encuentra configurada facturación");
}

function SearchFacturas() {
    //if (!$("#form-filtros").psly().validate()) return;
    $("#error-receptor").empty();
    var form = getFormData("#form-filtros");
    if (form.nombre.trim() === "" && form.receptor.trim() === "") {
        $("#error-receptor").append(`<ul class="parsley-errors-list filled" id="parsley-id-5"><li class="parsley-pattern">Por favor ingresa el R.F.C. ó la razon social</li></ul>`);
        return;
    }
    var url = `${window.UrlFacturacion}api/v1/restaurantes/facturas/${form.emisor}`;
    if (form.nombre.trim() !== "") {
        url += `?nombre=${encodeURI(form.nombre.trim())}`;
    }
    if (form.receptor.trim() !== "") {
        var separator = url.indexOf("?") !== -1 ? "&" : "?";
        url += `${separator}receptor=${encodeURI(form.receptor.trim())}`;
    }
    $("#tabla-facturas tbody").empty();
    $("#main-ibox .ibox-content").addClass("sk-loading");
    $.get({
        url: url,
        headers: {
            "X-Api-Secret": window.HeaderFacturación
        }
    }).done(handleResponse)
        .fail(notifyError)
        .always(() => $("#main-ibox .ibox-content").removeClass("sk-loading"));

    function isValidResponse(res) {
        return res.estatusPeticion.responseCode === 2000;
    }
    function handleResponse(res) {
        if (isValidResponse(res)) {
            DrawTable(res.Facturas);
        } else {
            console.error(res.estatusPeticion);
            notifyError();
        }
    }
    function notifyError() {
        SendAlert("Ocurrió un error al buscar las facturas por favor intente de nuevo más tarde", "error", false);
    }
}

function getFormData(form) {
    const unindexedArray = $(form).serializeArray();
    const indexedArray = {};

    $.map(unindexedArray, (n) => {
        indexedArray[n.name] = n.value;
    });

    return indexedArray;
}

function DrawTable(facturas) {
    if (facturas && facturas.some(c => c)) {
        $("#row-no-items").addClass("hide");
        //
        $("#tabla-facturas tbody").append(facturas.map(c => `<tr>
            <td>${formatDate(c.FechaTimbradoIso)}</td>
            <td>${c.RazonSocial} (${c.Rfc})</td>
            <td class="text-right">$${c.Total.toFixed(2)} MXN</td>
            <td class="text-right">
                <div class="btn-group">
                    <button type="button" class="trigger-reenviar btn btn-sm btn-primary" data-id="${c.Uuid}">Reenviar<i class="fa fa-share m-l-sm"></i></button>
                    <button data-toggle="dropdown" class="btn btn-sm btn-white dropdown-toggle">Descargar<i class="fa fa-download m-l-sm"></i></button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item" href="${UrlFacturacion}api/v1/comprobantes/comprobantes/${c.Uuid}/xml" download>XML</a></li>
                        <li><a class="dropdown-item" href="${UrlFacturacion}api/v1/comprobantes/comprobantes/${c.Uuid}/pdf" download>PDF</a></li>
                    </ul>
                </div>
            </td>
        </tr>`));
        $(".trigger-reenviar").on("click", ev => ReenviarFactura($(ev.currentTarget).data("id")));
    } else {
        $("#row-no-items").removeClass("hide");
    }
    function formatDate(date) {
        if (date) {
            return moment(date).format("LLL");
        } else {
            return "Sin fecha registrada";
        }
    }
}

function ReenviarFactura(id) {
    $("#main-ibox .ibox-content").addClass("sk-loading");
    var url = `${window.UrlFacturacion}api/v1/restaurantes/facturas/${id}/reenviar`;
    $.post({
        url: url,
        headers: {
            "X-Api-Secret": window.HeaderFacturación
        }
    }).done(handleResponse)
        .fail(notifyError)
        .always(() => $("#main-ibox .ibox-content").removeClass("sk-loading"));

    function isValidResponse(res) {
        return res.estatusPeticion.responseCode === 2000;
    }
    function handleResponse(res) {
        if (isValidResponse(res)) {
            SendAlert("La factura ha sido reenviada con éxito", "success", true);
        } else {
            console.error(res.estatusPeticion);
            notifyError();
        }
    }
    function notifyError() {
        SendAlert("Ocurrió un error al enviar la factura por favor intente de nuevo más tarde", "error", false);
    }
}