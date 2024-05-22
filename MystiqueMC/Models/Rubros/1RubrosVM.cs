// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Rubros.RubrosViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models.Rubros
{
  public class RubrosViewModel
  {
    public int IdCatRubro { get; set; }

    public int ComercioId { get; set; }

    public string Descripcion { get; set; }

    public Decimal Ponderacion { get; set; }

    public bool EsCosto { get; set; }

    public bool Activo { get; set; }
  }
}
