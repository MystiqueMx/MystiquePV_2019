var ChartSexos;
var ChartEdades;
var ChartUbicacion;

function OnScriptsLoad() {
    $('#widget-compras').on('click', () => { $('#form-detalle-compras').submit() });
    $('#widget-canje').on('click', () => { $('#form-detalle-canje').submit() });
    $('#widget-beneficios').on('click', () => { $('#form-detalle-beneficios').submit() });

    InitCharts();
}

function InitCharts() {

    $canvasSexo = $("#chart-sexos");
    $canvasEdades = $("#chart-edades");
    $canvasUbicacion = $("#chart-ubicacion");

    $datasourceGrafica1 = $("#datasource-chart-1");
    $datasourceGrafica2 = $("#datasource-chart-2");
    $datasourceGrafica3 = $("#datasource-chart-3");

    $titleSexo = $("#title-grafica-1");
    $titleEdades = $("#title-grafica-2");
    $titleUbicacion = $("#title-grafica-3");

    InitChart($canvasSexo, $datasourceGrafica1,'doughnut', $titleSexo.text());
    InitChart($canvasEdades, $datasourceGrafica2, 'doughnut', $titleEdades.text());
    InitChart($canvasUbicacion, $datasourceGrafica3, 'doughnut', $titleUbicacion.text());
}

function InitChart($canvas, $tbl, type, title) {
    var c = new Chart($canvas[0].getContext('2d'), {
        type: type,
        data: {
            labels: ParseLabels($tbl),
            datasets: [{
                data: ParseData($tbl),
                backgroundColor: ParseColors($tbl)
            }],
        },
        options: {
            title: {
                display: true,
                text: title
            },
            tooltips: {
                callbacks: {
                    label: CalcularPorcentajeChart 
                }
            },
        }
    });

}

function ParseLabels($tbl) {
    var labels = [];
    $tbl.find('tbody').children('tr').each((idx, val) => {
        labels.push($(val).find('td:nth-child(1)').text());
    });
    return labels;
}

function ParseData($tbl) {
    var data = [];
    $tbl.find('tbody').children('tr').each((idx, val) => {
        data.push(parseFloat($(val).find('td:nth-child(2)').text()));
    });
    return data;
}

function ParseColors($tbl) {
    var colors = [];
    $tbl.find('tbody').children('tr').each((idx, val) => {
        colors.push($(val).find('td:nth-child(3)').text());
    });
    return colors;
}

function CalcularPorcentajeChart(item, data) {
    var universo = data.datasets[0].data;
    var meta = data.datasets[0]._meta[0] || data.datasets[0]._meta[1] || data.datasets[0]._meta[2];
    var total = meta.total;
    var porcentaje = (universo[item.index] * 100) / total
    output = `${data.labels[item.index]} : ${porcentaje.toFixed(2)}%`;
    return output;
}