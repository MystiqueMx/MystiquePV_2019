
function OnScriptsLoad() {
    $('#modal-ver-puntos-compra').on('show.bs.modal', modalVerCompras);
    $('#modal-ver-puntos-canjeados').on('show.bs.modal', modalVerCanjeados);
    $('#modal-ver-beneficios-solicitados').on('show.bs.modal', modalVerBeneficios);
    $('#selectOrden').on('change', OrdenarPorCompra);
    $('#selectCanje').on('change', OrdenarPorCanje);
    $('#selectBeneficios').on('change', OrdenarPorBeneficios);

    $('#widget-compras').on('click', () => { $('#form-detalle-compras').submit() });
    $('#widget-canje').on('click', () => { $('#form-detalle-canje').submit() });
    $('#widget-beneficios').on('click', () => { $('#form-detalle-beneficios').submit() });

    InitDataTables();
    InitCharts();
}
function OrdenarPorCompra(event) {
  
    var botonVerCompras = $(event.relatedTarget);
    var IdCliente = $('#IdClienteCompras').val();
    var opcion = $('#selectOrden').val();

    var Url = $('#UrlDetailsPointsPurchases').val();

    consultarCompras(IdCliente, Url, opcion);

}

function OrdenarPorCanje(event) {

    var botonVerCompras = $(event.relatedTarget);
    var IdCliente = $('#IdClienteCanjes').val();
    var opcion = $('#selectCanje').val();

    var Url = $('#UrlDetailsPointsProducts').val();

    consultarCanjes(IdCliente, Url, opcion);

}

function OrdenarPorBeneficios(event) {

    var botonVerCompras = $(event.relatedTarget);
    var IdCliente = $('#IdClienteBeneficios').val();
    var opcion = $('#selectBeneficios').val();

    var Url = $('#UrlDetailsBenefits').val();

    consultarBeneficios(IdCliente, Url, opcion);

}

function modalVerCompras(event) {

    var botonVerCompras = $(event.relatedTarget);
    var modal = $(this);
    var IdCliente = botonVerCompras.data('idcliente');
    $('#IdClienteCompras').val(IdCliente)
    var opcion = $('#selectOrden').val();
    var Url = $('#UrlDetailsPointsPurchases').val();

    consultarCompras(IdCliente, Url, opcion);
}

function modalVerCanjeados(event) {

    var botonVerCompras = $(event.relatedTarget);
    var modal = $(this);
    var IdCliente = botonVerCompras.data('idcanje');
    $('#IdClienteCanjes').val(IdCliente)
    var opcion = $('#selectCanje').val();
    var Url = $('#UrlDetailsPointsProducts').val();

    consultarCanjes(IdCliente, Url,opcion);
}

function modalVerBeneficios(event) {

    var botonVerCompras = $(event.relatedTarget);
    var modal = $(this);
    var IdCliente = botonVerCompras.data('idsolicitado');
    $('#IdClienteBeneficios').val(IdCliente)
    var opcion = $('#selectBeneficios').val();
    var Url = $('#UrlDetailsBenefits').val();

    consultarBeneficios(IdCliente, Url, opcion);
}

function consultarCompras(Id, url, opcion) {
    payload = {
        id: Id,
        asc: opcion
    }

    $.ajax({
        url: url,
        data: payload,
    })
        .done(successHandler)
        .always(completeHandler)

    function successHandler(compra) {
        if (!compra.success) {
            console.error(compra.error)
            return
        }

        mostrarContenidoTabla(compra.results)
        mostrarTotalCompra(compra.total)
    }
    function mostrarTotalCompra(Total) {

        $('#totalPuntosCompras').empty();
        $('#totalPuntosCompras').text('Total: '+Total+' pts ');
    }
    

    function mostrarContenidoTabla(Puntoscompras) {

        $('#listado-puntos-compra tbody').empty();
        $('#listado-puntos-compra tbody').append(
            Puntoscompras.map(c => {
                return `<tr>
                            <td>${ToFormatoEspanyol(c.Fecha)}</td> 
                            <td>${c.NoTicket}</td>
                            <td class="text-right">${c.Monto}.00</td>
                            <td>${c.Puntos} pts</td>
                        </tr>`
            })
        )
    }

    function completeHandler() {
        //$('.spinner').hide();
    }

}

function consultarCanjes(Id, url, opcion) {
    payload = {
        id: Id,
        filtro: opcion
    }

    $.ajax({
        url: url,
        data: payload,
    })
        .done(successHandler)
        .always(completeHandler)

    function successHandler(canje) {
        if (!canje.success) {
           // console.error(canje.error)
            return
        }

        mostrarContenidoTablaCanje(canje.results)
        mostrarTotalCanje(canje.total)
    }

    function mostrarTotalCanje(Total) {

        $('#totalPuntosCanje').empty();
        $('#totalPuntosCanje').text('Total: ' + Total + ' pts ');
    }

    function mostrarContenidoTablaCanje(Puntoscanje) {

        $('#listado-puntos-canje tbody').empty();
        $('#listado-puntos-canje tbody').append(
            Puntoscanje.map(c => {
                return `<tr>
                <td>${ToFormatoEspanyol(c.fecharegistro)}
                </td> 
                <td>${c.descripcion}</td> 
                <td>${c.valorPuntos} pts </td>
                </tr>`
            })

        )

    }
    // ${new Date(c.fecharegistro).getHours()}:${new Date(c.fecharegistro).getMinutes()}
    function completeHandler() {
        //$('.spinner').hide();
    }

}

function consultarBeneficios(Id, url, opcion) {
    payload = {
        id: Id,
        asc: opcion
    }

    $.ajax({
        url: url,
        data: payload,
    })
        .done(successHandler)
        .always(completeHandler)

    function successHandler(beneficio) {
        if (!beneficio.success) {
            console.error(beneficio.error)
            return
        }

        mostrarContenidoTablaBeneficio(beneficio.results)
    }

    function mostrarContenidoTablaBeneficio(BeneficiosSolicitados) {

        $('#tabla-beneficios-solicitados tbody').empty();
        $('#tabla-beneficios-solicitados tbody').append(
            BeneficiosSolicitados.map(c => {
                return `<tr>
                <td>${ToFormatoEspanyol(c.fechaRegistro)}
                </td> 
                <td>${c.descripcion}</td> 
                </tr>`
            })

        )

    }
    // ${new Date(c.fecharegistro).getHours()}:${new Date(c.fecharegistro).getMinutes()}
    function completeHandler() {
        //$('.spinner').hide();
    }

}

function ToFormatoEspanyol(Fecha) {

    var fecha = new Date(Fecha).toLocaleDateString("es-MX", { hour: 'numeric', minute: 'numeric', hour12: true })

    return fecha
}

function InitDataTables() {

    $('#ContentTable').DataTable({
        "dom": '<lfB<t>ip>',
        "searching": false,
        "buttons": [
            {
                extend: 'excelHtml5',
                text: '<span id="realExcel" style="display:none" class="btn btn-primary btn-sm">Excel<span>',
                titleAttr: ''
            },
        ],
        "pageLength": 100,
        "language": {
            "search": "Buscar   ",
            "lengthMenu": "Elementos por página:  _MENU_",
            "info": "Mostrando _START_ - _END_ de _TOTAL_ elementos",
            "emptyTable": "No hay información",
            "paginate": {
                "first": "Primera",
                "last": "Ultima",
                "next": "Siguiente",
                "previous": "Anterior"
            },
        },
        responsive: {
            breakpoints: [
                { name: 'bigdesktop', width: Infinity },
                { name: 'meddesktop', width: 1480 },
                { name: 'smalldesktop', width: 1280 },
                { name: 'medium', width: 1188 },
                { name: 'tabletl', width: 1024 },
                { name: 'btwtabllandp', width: 848 },
                { name: 'tabletp', width: 768 },
                { name: 'mobilel', width: 480 },
                { name: 'mobilep', width: 320 }
            ]
        }
    });
}