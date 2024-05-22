function OnScriptsLoad() {

    var currentTime = new Date();

    $('#fechaGasto').datepicker({
        language: 'es',
        dateFormat: "yy/mm/dd",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+0",
        maxDate: 0,
        minDate: new Date(currentTime.getFullYear(), currentTime.getMonth() - 1, 1)
    });

    $('#calendarIcon').click(function () {
        $("#fechaGasto").focus();
    });
}