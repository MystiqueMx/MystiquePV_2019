using Newtonsoft.Json;
using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MystiqueNative.Models.Ensaladas
{
    public class EnsaladasContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public ResultadoEnsaladas Resultados { get; set; }
    }
    public class ResultadoEnsaladas
    {
        [JsonProperty("ingredientesEnsalada")]
        public List<IngredienteEnsalada> ListaIngredientesEnsaladas { get; set; }

        [JsonProperty("configuracionEnsalada")]
        public List<ConfiguracionEnsalada> ListaConfiguracionesEnsaladas { get; set; }
    }
    public class IngredienteEnsalada
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("precio")]
        public decimal Precio { get; set; }

        [JsonProperty("catTipoIngredienteEnsalada")]
        public int TipoIngrediente { get; set; }

        public CategoriaIngredienteEnsalada Categoria => (CategoriaIngredienteEnsalada)TipoIngrediente;
    }
    public class ConfiguracionEnsalada
    {
        [JsonProperty("idPresentacion")]
        public int Id { get; set; }

        [JsonProperty("platilloId")]
        public int IdPlatillo { get; set; }

        [JsonProperty("tamanio")]
        public string DescripcionTamano { get; set; }

        [JsonProperty("precioBase")]
        public decimal Precio { get; set; }

        [JsonProperty("proteina")]
        public int CantidadProteina { get; set; }

        [JsonProperty("barraFria")]
        public int CantidadBarraFria { get; set; }

        [JsonProperty("aderezos")]
        public int CantidadAderezos { get; set; }

        [JsonProperty("complementos")]
        public int CantidadComplementos { get; set; }

        [JsonProperty("cortesia")]
        public int CantidadCortesias { get; set; }

        public int CantidadExtras => 0;

        public PresentacionEnsalada Presentacion => (PresentacionEnsalada)Id;

        public string PrecioConFormatoMoneda => Precio.ToString("C");
    }

    public enum PresentacionEnsalada
    {
        NoDefinida = 0,

        [Description("Ensalada de pollo chica")]
        PolloChica = 1,

        [Description("Ensalada de pollo mediana")]
        PolloMediana = 2,

        [Description("Ensalada de pollo grande")]
        PolloGrande = 3,

        [Description("Ensalada de mariscos chica")]
        MariscosChica = 4,

        [Description("Ensalada de mariscos mediana")]
        MariscosMediana = 5,

        [Description("Ensalada de mariscos grande")]
        MariscosGrande = 6,

        [Description("Ensalada de camaron chica")]
        CamaronChica = 7,

        [Description("Ensalada de camaron mediana")]
        CamaronMediana = 8,

        [Description("Ensalada de camaron grande")]
        CamaronGrande = 9,

        [Description("Wrap")]
        WrapChica = 10
    }

    public enum CategoriaIngredienteEnsalada
    {
        NoDefinida = 0,
        Proteina = 1,
        BarraFria = 2,
        Aderezos = 3,
        Complementos = 4,
        Extras = 5,
        Cortesias = 6,
    }

    public enum AderezoEnsalada
    {
        NoDefinida = 0,

        [Description("Aderezo dentro")]
        EnEnsalada = 1,

        [Description("Aderezo por separado")]
        PorSeparado = 2,
    }

    public enum ComplementosEnsalada
    {
        NoDefinida = 0,

        [Description("Complementos sobre la ensalada")]
        EnEnsalada = 1,

        [Description("Complementos por separado")]
        PorSeparado = 2,
    }
}
