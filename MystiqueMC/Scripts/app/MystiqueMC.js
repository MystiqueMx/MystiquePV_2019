$(document).ready(BootstrapScripts)

function BootstrapScripts() {
    $(":input").inputmask();
    SetUpGlobalPlugins();
    if (typeof OnScriptsLoad !== "undefined")
        OnScriptsLoad();
}

Date.prototype.getWeek = function (start) {
    //Calcing the starting point
    start = start || 0;
    var today = new Date(this.setHours(0, 0, 0, 0));
    var day = today.getDay() - start;
    var date = today.getDate() - day;

    // Grabbing Start/End Dates
    var StartDate = new Date(today.setDate(date));
    var EndDate = new Date(today.setDate(date + 6));
    return [StartDate, EndDate];
}


function SetUpGlobalPlugins() {
    //SetUpCheckboxes();
    //SetUpInputMask();
    //SetUpAlerts();
    //SetUpIdleTimer();
    BindLoader();
}
function SetUpAlerts() {
    $("#alert").fadeTo(5000, 500).slideUp(500, function () {
        $("#alert").slideUp(500);
    });
}

function SetUpInputMask() {
    $("input[type='text'], textarea").inputmask();
}

function SetUpDataTableCatalogos(selector) {
    if (!selector) selector = ".tabla-catalogos";
    if ($(selector).length > 0) {
        //StartSpinner();
        $(selector).dataTable({
            "dom": '<"html5buttons"B>lTgtip',
            "ordering": false,
            "pageLength": 100,
            "buttons": [
                {
                    extend: "excelHtml5",
                    text: "Exportar a Excel"
                },
            ],
            "pageLength": 100,
            "language": {
                "emptyTable": "No hay información",
                "info": "Mostrando _START_ - _END_ de _TOTAL_ elementos",
                "infoEmpty": "Mostrando 0 elementos ",
                "infoFiltered": "- filtrado de _MAX_ registros)",
                "lengthMenu": "Elementos por página:  _MENU_ ",
                "loadingRecords": "Cargando ... ",
                "paginate": {
                    "first": "Primera",
                    "last": "Ultima",
                    "next": "Siguiente",
                    "previous": "Anterior"
                },
                "processing": "Procesando ... ",
                "search": "Buscar",
            },
            "fixedHeader": true
        });
        $(selector).DataTable()
            .on("preDraw", StartSpinner)
            .on("draw", StopSpinner);
        setTimeout(StopSpinner, 100);

    }
}

function StartSpinner() {
    if ($(".ibox-content").hasClass("sk-loading")) return;
    $(".ibox-content").addClass("sk-loading");
}

function StopSpinner() {
    if (!$(".ibox-content").hasClass("sk-loading")) return;
    $(".ibox-content").removeClass("sk-loading");
}

function SendAlert(mensaje, tipo, longitudIndefinida, customDivSelector, customId) {
    var dismissableAttrubute = longitudIndefinida ? "" : "alert-dismissable";
    // warning  error success
    var tipoAlerta = null;

    switch (tipo) {
        case "warning":
            tipoAlerta = "warning";
            break;
        case "error":
            tipoAlerta = "danger";
            break;
        default:
            tipoAlerta = "success";
            break;
    }

    var numero = Math.floor(Math.random() * 1000) + 1;
    var idAlerta = customId ? customId : "div-fg-" + numero;
    var alerta = $(`<div class="row" id="${idAlerta}" ><div class="col-md-12"><div class="m-md alert alert-${tipoAlerta} ${dismissableAttrubute}">
                        <button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>${mensaje}<br>
                    </div></div></div>`)
        .clone();
    if (customDivSelector) {
        $(customDivSelector).append(alerta);

    } else {
        $("#top-navbar").append(alerta);

    }
    if (!longitudIndefinida) {
        setTimeout(function () {
            $("#" + idAlerta).fadeTo(300, 0).slideUp(300, function () {
                $("#" + idAlerta).remove();
            });
        }, 2000);
    }

}

function SetUpIdleTimer() {
    if ($("#logoutForm input[name='auto-logout-enabled']").val() !== "1") return;
    idleTimeout = parseInt($("#logoutForm input[name='auto-logout-timeout'").val());
    if (idleTimeout < 2) {
        idleTimeout = 2;
    }
    $.idleTimer(3 * 1000 * 60 * idleTimeout / 4);
    $(document).on("idle.idleTimer", IdleWarning);
    $(document).on("active.idleTimer", IdleReset);
    window.setInterval(AutoSessionRefresh, 60000 * idleTimeout / 4);
}

function IdleWarning() {
    SendAlert("Te encuentras inactivo en el sistema, realiza alguna acción para prevenir que tu sesión expire",
        "warning",
        true,
        null,
        "alert-inactive");
    idleTimer = window.setTimeout(IdleLogout, 1 * 1000 * 60 * idleTimeout / 4);
}

function IdleLogout() {
    $("#logoutForm input[name='autoLogout']").val("1");
    $("#logoutForm").submit();
}

function IdleReset() {
    $("#alert-inactive .alert").append(`<div class="m-t-xs text-navy"><span class="loading dots"></span></div>`);
    $.post($("#logoutForm input[name='refresh-session-url']").val()) //intenta conservar sesión con ajax
        .done(function () {
            $("#alert-inactive").fadeTo(300, 0).slideUp(300, function () {
                $("#alert-inactive").remove();
            });
            window.clearTimeout(idleTimer);
        }).fail(function () {
            //window.location.reload(); // si falla recarga la pagina para conservar la sesión @TODO
        });
}

function AutoSessionRefresh() {
    if ($(document).idleTimer("isIdle")) return;
    $.post($("#logoutForm input[name='refresh-session-url']").val(), function () {
        console.info(`Session refreshed at ${new Date().toTimeString()}`);
    });
}


function BindLoader() {

    $(".one-click-button").on("click", ()=> InitLoader());

    $(".one-click-form").on("submit", ()=> InitLoader());
}

function InitLoader(options) {
    if (!options) options = {
        effect: "rotateplane",
        text: "Espere por favor...",
        bg: "rgba(255, 255, 255, 0.8)",
        color: "#2F4050"
    };
    //@TODO
    $("body").waitMe(options);
}

function ShutDownLoader() {
    //@TODO
    $("body").waitMe("hide");
}

function SetUpCheckboxes() {
    $(".icheck").iCheck({
        checkboxClass: "icheckbox_minimal-blue",
        radioClass: "iradio_minimal-blue",
        increaseArea: "20%"
    });
}

function SerializeForm(form) {
    const unindexedArray = $(form).serializeArray();
    const indexedArray = {};

    $.map(unindexedArray, (n) => {
        indexedArray[n.name] = n.value;
    });

    return indexedArray;
}

function AddCommas(decimal) {
    decimal = `${decimal.toFixed(2)}`.replace(',', '');
    const segments = decimal.split('.');
    let integerRegion = segments[0];
    const decimalRegion = segments.length > 1 ? '.' + segments[1] : '';
    while (/(\d+)(\d{3})/.test(integerRegion))
        integerRegion = integerRegion.replace(/(\d+)(\d{3})/, `$1,$2`);
    return integerRegion + decimalRegion;
}
function StripFormat(text) {
    text = text.replace(',', '').replace('$', '');
    return parseFloat(text);
}

function FormatAsCurrency(amount, ignoreDollarSign) {
    if (!Number.isFinite(amount)) {
        amount = parseFloat(amount);
    }
    const amountWithCommas = AddCommas(amount);
    if (ignoreDollarSign) return amountWithCommas;
    return `${amountWithCommas}`;
}
