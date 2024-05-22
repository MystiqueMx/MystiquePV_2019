var UploadImageUrl = ''
var $image;
var $form;
var cropper;
function OnScriptsLoad() {
    $('input[name="Monto"], input[name="Cantidad"], input[name="Equivalente"]')
        .on('change', ActualizarCalculoPuntos)
    ActualizarCalculoPuntos()
}
function ActualizarCalculoPuntos() {
    var Monto = $('input[name="Monto"]').val();
    var Cantidad = $('input[name="Cantidad"]').val();
    var Equivalente = $('input[name="Equivalente"]').val();
    if (!IsNumeric(Cantidad) || !IsNumeric(Monto) || !IsNumeric(Equivalente)) {
        $('#puntos-compra').val('0')
    } else {
        var total = parseFloat(Monto) / parseFloat(Equivalente);
        total = total * parseFloat(Cantidad);
        $('#puntos-compra').val(total);
    }
}
function IsNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n) && n !== '0';
}

function porcentajeKeyPress(elemento, e) {
    var maxlengthNumber = parseInt(elemento.maxlength);
    var inputValueLength = elemento.value.length + 1;
    var inputValue = parseFloat(elemento.value == "" || elemento.value == "0.00" ? "0" : elemento.value );

    var newValue = (inputValue != 0.00 ? inputValue : "") + e.key;

    if (parseFloat(newValue).toFixed(2) >= 100) {
        elemento.value = 100;
        e.preventDefault();
        return false;
    } else {
        if (e.which != 8 && e.which != 0 && e.which != 46 && (e.which < 48 || e.which > 57)) {
            return false;
        }
        if (maxlengthNumber < inputValueLength) {
            return false;
        }
    }
}