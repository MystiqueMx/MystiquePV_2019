using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Ensaladas
{
    public class PreciosEnsaladas
    {
        public string PrecioEnsaladaPolloChica { get; set; }

        public string PrecioEnsaladaPolloMediana { get; set; }

        public string PrecioEnsaladaPolloGrande { get; set; }

        public string PrecioEnsaladaMariscosChica { get; set; }

        public string PrecioEnsaladaMariscosMediana { get; set; }

        public string PrecioEnsaladaMariscosGrande { get; set; }

        public string PrecioEnsaladaCamaronChica { get; set; }

        public string PrecioEnsaladaCamaronMediana { get; set; }

        public string PrecioEnsaladaCamaronGrande { get; set; }

        public string PrecioWrapChica { get; set; }


    }

    public class ConfiguracionEnsaladas
    {
        public PreciosEnsaladas Precios { get; set; }
        public List<IngredienteEnsalada> IngredientesProteina { get; set; }
        public List<IngredienteEnsalada> IngredientesBarraFria { get; set; }
        public List<IngredienteEnsalada> IngredientesAderezos { get; set; }
        public List<IngredienteEnsalada> IngredientesComplementos { get; set; }
        public List<IngredienteEnsalada> IngredientesExtras { get; set; }
        public List<IngredienteEnsalada> IngredientesCortesias { get; set; }
    }
}
