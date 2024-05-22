// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Rubros.RubrosIndexViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;
using System.Collections.Generic;


namespace MystiqueMC.Models.Rubros
{
  public class RubrosIndexViewModel
  {
    public List<RubrosViewModel> RubrosVM { get; set; }

    public Decimal TotalPonderacion { get; set; }

    public string TotalPonderacionFormateado => this.TotalPonderacion.ToString("N2");
  }
}
