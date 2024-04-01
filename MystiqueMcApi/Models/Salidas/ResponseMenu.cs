using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponseMenu : ErrorObjCodeResponseBase
    {
        public List<ResponseMenuListado> respuesta { get; set; }
    }

    public class ResponseMenuListado
    {
        public int menuId { get; set; }
        public string descripcion { get; set; }
        public string imagen { get; set; }
        public int? orden { get; set; }
        public bool esEnsalada { get; set; }
    }

    public class ResponseListasMenuEnsalada : ErrorObjCodeResponseBase
    {
        public ResponseMenuEnsalada respuesta { get; set; }
    }


    public class ResponseMenuEnsalada
    {
        public List<EnsaladaPlatillo> configuracionEnsalada { get; set; }
        public List<CatEnsaladaPlatilloPrecios> ingredientesEnsalada { get; set; }
    }


    public class EnsaladaPlatillo
    {
        public int platilloId { get; set; }
        public string tamanio { get; set; }
        public decimal precioBase { get; set; }
        public int proteina { get; set; }
        public int barraFria { get; set; }
        public int aderezos { get; set; }
        public int complementos { get; set; }
        public int? cortesia { get; set; }
        public int idPresentacion { get; set; }

    }


    public class CatEnsaladaPlatilloPrecios
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public decimal? precio { get; set; }
        public int? catTipoIngredienteEnsalada { get; set; }

    }

    public class ResponseListasMenu : ErrorObjCodeResponseBase
    {
        public listaResponseMenuPlatillo respuesta { get; set; }
    }

    public class listaResponseMenuPlatillo
    {
        public List<ResponseMenuPlatillo> listaPlatillos { get; set; }
    }

    public class ResponseMenuPlatillo
    {
        public int idPlatillo { get; set; }
        public string nombre { get; set; }
        public string desPlatillo { get; set; }
        public bool isCombo { get; set; }
        public int cantidad { get; set; }
        public string urlImagen { get; set; }
        public decimal precio { get; set; }

        public int? orden { get; set; }

        public List<PlatilloNivelUno> configuracionUno { get; set; }
    }

    public class PlatilloNivelUno
    {
        public int? id { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public int? cantidad { get; set; }
        public List<PlatilloNivelDos> configuracionDos { get; set; }
    }

    public class PlatilloNivelDos
    {
        public int? id { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public int? cantidad { get; set; }
        public List<PlatilloNivelTres> configuracionTres { get; set; }
    }

    public class PlatilloNivelTres
    {
        public int? id { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public int? cantidad { get; set; }
    }
}