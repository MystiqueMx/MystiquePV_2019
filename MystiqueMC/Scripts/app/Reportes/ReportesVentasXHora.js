function SetupPivotTable(arrayTemp) {
    var derivers = $.pivotUtilities.derivers;
    var sortAs = $.pivotUtilities.sortAs;
    var renderers = $.extend($.pivotUtilities.renderers,
        $.pivotUtilities.plotly_renderers);

    var utils = $.pivotUtilities;
    var heatmap = utils.renderers["Table"];

    var sum = $.pivotUtilities.aggregatorTemplates.sum;
    var numberFormat = $.pivotUtilities.numberFormat;
    var frFormat = numberFormat({ thousandsSep: ",", decimalSep: "." });
    var frFormat2 = numberFormat({ thousandsSep: ",", decimalSep: ".", digitsAfterDecimal: 0 });

    $.pivotUtilities.tipsData = arrayTemp;
    var rows = Object.keys(arrayTemp[0]);
    $("#output").pivotUI(
        $.pivotUtilities.tipsData, {
            rows: ["Hora"],
            cols: ["Dia"],
            vals: ["tickets"],
            aggregatorName: "Suma",
            aggregators: { "Suma": function () { return sum(frFormat2)(["tickets"]); } },
            sorters: {
                "dia": sortAs(["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes",
                    "Sábado"])
            },
            showUI: false,
            renderer: heatmap,
            rendererOptions: {
                plotly: { width: 900, height: 600 }
            }
        }, false, "es");

    $("#output2").pivotUI(
        $.pivotUtilities.tipsData, {
            rows: ["Hora"],
            cols: ["Dia"],
            vals: ["importe"],
            aggregatorName: "Suma",
            aggregators: { "Suma": function () { return sum(frFormat)(["importe"]); } },
            sorters: {
                "Dia": sortAs(["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes",
                    "Sábado"])
            },
            showUI: false,
            renderer: heatmap,
            rendererOptions: {
                plotly: { width: 1000, height: 600 }
            }
        }, false, "es");


}
