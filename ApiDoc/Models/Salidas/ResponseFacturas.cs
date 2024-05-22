using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseFacturas : ResponseBase
    {
        public int clienteId { get; set; }
    }
   
    public class ResponseListaCatUsoCFDI
    {
        public List<ResponseCatUsoCFDI> catUsoCFDI { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ResponseCatUsoCFDI
    {
        public string idCatUsoCFDI { get; set; }
        public string descripcion { get; set; }
    }




    public class ResponseListaDatosFiscalesReceptor
    {
        public List<ResponseDatosFiscalesReceptor> listaDatosFiscalesReceptor { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ResponseDatosFiscalesReceptor
    {
       public int receptorClienteId  { get; set; }   
       public bool prederteminada { get; set; }
       public string email { get; set; }
       public string  claveUsoCFDI { get; set; }
       public string companiaNombreLegal { get; set; }
       public string rfc { get; set; }
       public string codigoPostal { get; set; }
       public string direccion { get; set; }
    }



    public class ResponseValidarTicket
    {
        public List<ResponseDatosFiscalesReceptor> listaDatosFiscalesReceptor { get; set; }
        public List<ResponseCatUsoCFDI> catUsoCFDI { get; set; }
        public string ticket { get; set; }
        public string sucursal { get; set; }
        public DateTime fechaCompra { get; set; }
        public decimal montoCompra { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public int sucursalId { get; set; }
        public bool PendienteTicket { get; set; }
    }



    public class ResponseListadoMisFacturas
    {
        public List<ResponseMisFacturas> listado { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class ResponseMisFacturas
    {
        public Guid facturaClienteId { get; set; }
        public string numeroFactura { get; set; }
        public DateTime fecha { get; set; }
        public DateTime fechaCompra { get; set; }
        public decimal montoCompra { get; set; }
        public string sucursal { get; set; }
        public string estatus { get; set; }
        public bool PuedeReenviar { get; set; }
        public string rfc { get; set; }
        public string razonSocial { get; set; }
        public string email { get; set; }
    }

    public class ResponseregistrarFacturas
    {       
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

    }
}