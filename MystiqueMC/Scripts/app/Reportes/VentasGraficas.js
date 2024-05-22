
let chart;
var dataTables;
var config;

function InitChart($canvas, type, title) {

    var ctx = $canvas[0].getContext('2d');
    dataTables = $('#chart-data-content').find('table');

    config = {
        type: type,
        data: {
            labels: window.labels,
            datasets: GetData(dataTables)
        },
        options: {
            responsive: true,
            title: {
                display: true,
                text: title,
                fontStyle: 'normal',
                fontSize: 20
            },
            maintainAspectRatio: false,
            scales: {
                yAxes: [{
                    display: true,
                    ticks: {
                        beginAtZero: true,
                        suggestedMin: 10,
                        userCallback: function (value, index, values) {
                            value = value.toString();
                            value = value.split(/(?=(?:...)*$)/);
                            value = value.join(',');
                            return value;
                        }
                    }
                }]
            },
            tooltips: {
                callbacks: {
                    label: GetToolTips
                }
            },
        }
    };
    GetData(dataTables);
    chart = new Chart(ctx, config);

    if (config.data.datasets.length > 0) {
        $("#chart-ventas").show();
        $("#chart-container-div").removeClass('hide');
    } else {
        $("#chart-ventas").hide();
        $("#chart-container-div").addClass('hide');
    }

}

function ParseLabels($tbl) {
    var labels = [];
    $tbl.find('tbody').children('tr').each((idx, val) => {
        console.log(val);
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

function GetData(dataTable) {

    var data = [];
    
    dataTable.each((index, val) => {

        console.log(val);
       
        var newColor = window.chartColors[index];

            data.push({
                label: $(val).attr('title'),
                data: ParseData($(val)),
                borderColor: newColor,
                backgroundColor: newColor,
                lineTension: 0,
                fill: false
        });
      
    });
    return data;
}
function GetLabels(dataTable) {

    var labels = [];
    dataTable.each((index, val) => {

        var parsedLabels = ParseLabels($(val));
        console.log(parsedLabels);
        labels.push(parsedLabels);
    });
    return labels[0];

}
function GetToolTips(item, data) {
    var meta = data.datasets[0]._meta[0] || data.datasets[0]._meta[1] || data.datasets[0]._meta[2];
    output = `${data.datasets[item.datasetIndex].label}  ${addCommas(item.yLabel)}`;

    return output;
}

function getRandomColor() {
    var letters = '0123456789ABCDEF'.split('');
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}
function addCommas(decimal) {
    decimal = `${decimal.toFixed(0)}`.replace(',', '');
    const segments = decimal.split('.');
    let integerRegion = segments[0];
    const decimalRegion = segments.length > 1 ? '.' + segments[1] : '';
    while (/(\d+)(\d{3})/.test(integerRegion))
        integerRegion = integerRegion.replace(/(\d+)(\d{3})/, `$1,$2`);
    return integerRegion + decimalRegion;
}