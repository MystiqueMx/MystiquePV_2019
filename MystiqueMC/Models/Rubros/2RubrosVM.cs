// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Rubros.ConceptosGastosViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models.Rubros
{
  public class ConceptosGastosViewModel
  {
    public int IdCatConceptoGasto { get; set; }

    public int ComercioId { get; set; }

    public int CatRubroId { get; set; }

    public string Descripcion { get; set; }

    public bool Activo { get; set; }

    public Decimal Ponderacion { get; set; }
  }
}
