using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponseBeneficio : ResponseBase
    {

    }

    public class ResponseListaBeneficio : ResponseBase
    {
        List<beneficios> listaBeneficios { get; set; }
    }



    public class ListaBeneficioSucursal : ResponseBase
    {
        public int ComercioId { get; set; }
        public string Descripcion { get; set; }
        public int BeneficioId { get; set; }
        public bool HorarioActivo { get; set; }
        public string HorarioInicio { get; set; }
        public string HorarioFin { get; set; }
        public string Dias { get; set; }
        public string DiasSucursal { get; set; }
        public string TerminosYCondiciones { get; set; }
        public string tipoCodigo { get; set; }
        public string CadenaCodigo { get; set; }
        public string urlImgBeneficio { get; set; }
        public bool isDiaValido { get; set; }
    }


    public class ResponseBeneficioSucursal : ResponseBase
    {
        public List<SP_ObtenerBeneficiosSucursal_Result> listaBeneficiosSucursal { get; set; }
    }


    public class ListaBeneficioDetalle : ResponseBase
    {
        public string BeneficioDescripcion { get; set; }
        public string UrlImagenCodigo { get; set; }
        public string SucursalNombre { get; set; }
        public Nullable<bool> hasCode { get; set; }
        public string contentBarQRCode { get; set; }
        public string typeCode { get; set; }
        public Nullable<int> cantidadCalificados { get; set; }
        public Nullable<decimal> calificacionPromedio { get; set; }
        public string urlImgBeneficio { get; set; }
        public Nullable<bool> calificadoPorCliente { get; set; }
        public string Dias { get; set; }
        public Nullable<bool> savedInWallet { get; set; }
    }


    public class ResponseBeneficioDetalle : ResponseBase
    {
        public SP_Obtener_DetalleBeneficio_Result beneficioDetalle { get; set; }
    }

}