using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponseSucursal
    {
        public int SucursalId { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string DireccionColonia { get; set; }
        public string DireccionCalle { get; set; }
    }

    public class ResponseListaSucursal : ResponseBase
    {

        public List<SP_Obtener_Sucursal_Comercio_Result> ListSucursalPorComercio { get; set; }
    }

    #region HAZ TU PEDIDO
    public class ResponseHazPedidoSucursal : ErrorObjCodeResponseBase
    {
        public ResponseHazPedidoDatosSucursal respuesta { get; set; }
    }

    public class ResponseHazPedidoDatosSucursal
    {
        public List<SPObtenerSucursalesPoligonoResult> listadoSucursales { get; set; }
    }

    public class SPObtenerSucursalesPoligonoResult
    {
        public int idSucursal { get; set; }
        public string nombre { get; set; }
        public string logoUrl { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.TimeSpan> horaInicio { get; set; }
        public Nullable<System.TimeSpan> horaFin { get; set; }
        public decimal montoMinimo { get; set; }
        public decimal costoEnvio { get; set; }
        public int abierto { get; set; }
        public string direccion { get; set; }
        public bool activoPlataforma { get; set; }
        public bool tieneRepartoDomicilio { get; set; }
        public bool tieneDriveThru { get; set; }
    }

    public class ResponseHazPedidoSucursalActivas : ErrorObjCodeResponseBase
    {
        public ResponseHazPedidoDatosSucursalActiva respuesta { get; set; }
    }

    public class ResponseHazPedidoDatosSucursalActiva
    {
        public List<SPObtenerSucursalesActivasResult> listadoSucursalesActivas { get; set; }
    }

    public class SPObtenerSucursalesActivasResult
    {
        public int idSucursal { get; set; }
        public string nombre { get; set; }
        public string logoUrl { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.TimeSpan> horaInicio { get; set; }
        public Nullable<System.TimeSpan> horaFin { get; set; }
        public decimal montoMinimo { get; set; }
        public decimal costoEnvio { get; set; }
        public int abierto { get; set; }
        public string direccion { get; set; }
        public bool tieneRepartoDomicilio { get; set; }
    }
    #endregion
}