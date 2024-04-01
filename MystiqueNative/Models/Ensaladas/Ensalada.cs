using MystiqueNative.Models.HazTuWrap;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Ensaladas
{
    public class Ensalada
    {
        public int IdEnsalada { get; set; }
        public PresentacionEnsalada Presentacion { get; set; }
        public Dictionary<int, int> CantidadIngredientesBarra { get; set; }
        public Dictionary<int, int> CantidadIngredientesProteina { get; set; }
        public Dictionary<int, int> CantidadIngredientesExtra { get; set; }
        public Dictionary<int, int> CantidadIngredientesAderezos { get; set; }
        public Dictionary<int, int> CantidadIngredientesComplementos { get; set; }
        public Dictionary<int, int> CantidadIngredientesCortesias { get; set; }

        public AderezoEnsalada Aderezo { get; set; }
        public ComplementosEnsalada Complemento { get; set; }
        
        /* exclusivo de Hacer Wrap */
        public TortillaWrap Tortilla { get; set; }

        public decimal Precio { get; set; }

        public string PrecioConFormatoMoneda => Precio.ToString("C");

    }
}
