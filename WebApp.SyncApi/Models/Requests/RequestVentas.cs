using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.SyncApi.Models.Requests
{


    public class RequestVentas
    {
        public Venta[] venta { get; set; }
    }

    public class Venta
    {
        public int id { get; set; }
        public bool activo { get; set; }
        public DateTime fechaRegistro { get; set; }
        public DateTime fechaInicial { get; set; }
        public DateTime fechaFinal { get; set; }
        public string uuidVenta { get; set; }
        public Apertura[] Aperturas { get; set; }
    }

    public class Apertura
    {
        public int id { get; set; }
        public bool activo { get; set; }
        public DateTime fechaRegistro { get; set; }
        public DateTime fechaInicial { get; set; }
        public DateTime fechaFinal { get; set; }
        public decimal tipoCambio { get; set; }
        public decimal fondo { get; set; }
        public string uuidApertura { get; set; }
        public int ventaId { get; set; }
        public int usuarioRegistroId { get; set; }
        public int usuarioAutorizoId { get; set; }
        public Pedido[] Pedidos { get; set; }
        public Empleados empleados { get; set; }
        public Autorizo autorizo { get; set; }
        public Retiro[] Retiros { get; set; }
        public Gastos[] Gastos { get; set; }
    }

    public class Empleados
    {
        public string nombre { get; set; }
        public string apellidos { get; set; }
    }

    public class Autorizo
    {
        public string nombre { get; set; }
        public string apellidos { get; set; }
    }

    public class Pedido
    {
        public int id { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string uuidPedido { get; set; }
        public int aperturaId { get; set; }
        public int catPedidoEstatusId { get; set; }
        public int catTipoPedidoId { get; set; }
        public int usuarioRegistroId { get; set; }
        public Ticket[] Tickets { get; set; }
        public Pedidosproducto[] PedidosProductos { get; set; }
        public Pedidoestatus PedidoEstatus { get; set; }
        public Pedidotipo PedidoTipo { get; set; }
        public Usuario Usuario { get; set; }
        public SeguimientoPedidos[] SeguimientoPedidos { get; set; }
        public ConsumoEmpleados[] ConsumoEmpleados { get; set; }
        public string mesero_numero { get; set; }
        public string mesero_nombre { get; set; }
        public string mesa_nombre { get; set; }
    }

    public class Pedidoestatus
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }

    public class Pedidotipo
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }

    public class Usuario
    {
        public string nombre { get; set; }
        public string apellidos { get; set; }
    }

    public class Ticket
    {
        public int id { get; set; }
        public string folioTicket { get; set; }
        public int numeroTicket { get; set; }
        public bool facturado { get; set; }
        public decimal subTotal { get; set; }
        public decimal iva { get; set; }
        public decimal importe { get; set; }
        public DateTime fechaRegistro { get; set; }
        public DateTime? fechaCancelacion { get; set; }
        public decimal cambio { get; set; }
        public string uuidTicket { get; set; }
        public int pedidoId { get; set; }
        public int usuarioRegistroId { get; set; }
        public Ticketspago[] TicketsPagos { get; set; }
        public Usuario1 Usuario { get; set; }
        public Ticketsreimpreso[] TicketsReimpresos { get; set; }
        public TicketCancelados[] TicketCancelados { get; set; }
        public TicketDescuentos[] TicketDescuentos { get; set; }
        public bool estaCancelado { get; set; }
        public decimal tasaIva { get; set; }
    }

    public class Usuario1
    {
        public string nombre { get; set; }
        public string apellidos { get; set; }
    }

    public class Ticketspago
    {
        public int id { get; set; }
        public string confirmacion { get; set; }
        public decimal importe { get; set; }
        public int ticketId { get; set; }
        public int catTipoPagoId { get; set; }
        public int? catBancoId { get; set; }
        public Tipospago TiposPago { get; set; }
        public Banco Banco { get; set; }
        public decimal importeOriginal { get; set; }
    }

    public class Tipospago
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }

    public class TiposGasto
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }


    public class Banco
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public bool activo { get; set; }
    }

    public class Ticketsreimpreso
    {
        public int id { get; set; }
        public DateTime fechaRegistro { get; set; }
        public int ticketId { get; set; }
        public int usuarioRegistroId { get; set; }
        public int usuarioAutorizoId { get; set; }
        public Empleadosticketreimpreso empleadosTicketReimpreso { get; set; }
        public Autorizoticketreimpreso autorizoTicketReimpreso { get; set; }
    }

    public class Empleadosticketreimpreso
    {
        public string nombre { get; set; }
        public string apellidos { get; set; }
    }

    public class Autorizoticketreimpreso
    {
        public string nombre { get; set; }
        public string apellidos { get; set; }
    }

    public class Pedidosproducto
    {
        public int id { get; set; }
        public int cantidad { get; set; }
        public string notas { get; set; }
        public int productoId { get; set; }
        public int pedidoId { get; set; }
        public int ticketId { get; set; }
        public int usuarioRegistroId { get; set; }
        public Pedidosproductodetalle[] PedidosProductoDetalles { get; set; }
        public Usuario Usuario { get; set; }
        public decimal precioUnitario { get; set; }
    }

    public class Pedidosproductodetalle
    {
        public int id { get; set; }
        public string variedad { get; set; }
        public int pedidoProductoId { get; set; }
        public int insumoProductoId { get; set; }
        public InsumosProductos InsumosProducto { get; set; }
    }
    public class InsumosProductos
    {
        public int id { get; set; }

        public int agrupadorInsumoId { get; set; }

        public int productoId { get; set; }

        public int insumoId { get; set; }
    }
    public class Retiro
    {
        public int id { get; set; }
        public DateTime fechaRegistro { get; set; }
        public decimal monto { get; set; }
        public string observaciones { get; set; }     
        public Usuario Usuario { get; set; }
        public int aperturaId { get; set; }
        public int usuarioRegistroId { get; set; }
    }


    public class Gastos
    {
        public int id { get; set; }
        public DateTime fechaRegistro { get; set; }
        public decimal monto { get; set; }
        public string observaciones { get; set; }
        public Usuario Usuario { get; set; }
        public int aperturaId { get; set; }
        public int usuarioRegistroId { get; set; }
        public TiposGasto TiposGasto { get; set; }
    }


    public class SeguimientoPedidos
    {
        public int id { get; set; }
        public Clientes Clientes { get; set; }
       
    }

    public class Clientes
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string direccionEntrega { get; set; }
        public string telefono { get; set; }
        public bool otraColonia { get; set; }
        public int coloniaId { get; set; }
        public string colonia { get; set; }
    }


    public class SeguimientosPedido
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }


    public class ConsumoEmpleados
    {
        public int id { get; set; }
        public Empleados Empleado { get; set; }       
    }

    public class TicketCancelados
    {
        public int id { get; set; }
        public DateTime fechaRegistro { get; set; }
        public int ticketId { get; set; }
        public int usuarioRegistroId { get; set; }
        public string motivo { get; set; }
        public Usuario usuario { get; set; }
        public AutorizoTicketCancelado autorizoTicketCancelado { get; set; }

    }

    public class AutorizoTicketCancelado
    {
        public string nombre { get; set; }
        public string apellidos { get; set; }
    }


    public class TicketDescuentos
    {
        public int id { get; set; }
        public DateTime fechaRegistro { get; set; }
        public int ticketId { get; set; }
        public int descuentoId { get; set; }
        public decimal monto { get; set; }
        
    }
}