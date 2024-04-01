using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Entradas
{
    public class RequestFactura : RequestBase
    {
        public int clienteId { get; set; }
    }


    public class RequestValidarTicket : RequestBase
    {
        public int clienteId { get; set; }
        public string codigoGenerado { get; set; }
    }


    public class RequestsolicitarFactura : RequestBase
    {
        public int clienteId { get; set; }
        public int receptorClienteId { get; set; }
        public string claveUsoCFDI { get; set; }
        public string correo { get; set; }
        public string rfcReceptor { get; set; }
        public string companiaNombreLegal { get; set; }
        public string codigoPostal { get; set; }
        public string folioTicket { get; set; }
        public string direccion { get; set; }
        public bool PendienteTicket { get; set; }
        public int SucursalId { get; set; }
    }
    public class RequestEliminarDatosFiscales : RequestBase
    {
        public int clienteId { get; set; }
        public int receptorClienteId { get; set; }
    }
    public class RequestReenvioFactura : RequestBase
    {
        public int clienteId { get; set; }
        public Guid factura { get; set; }
        public int email { get; set; }
    }
}