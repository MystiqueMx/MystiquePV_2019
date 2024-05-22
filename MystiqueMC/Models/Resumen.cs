// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Resumen
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models
{
  public class Resumen
  {
    public Decimal TotalCompras { get; set; }

    public Decimal TotalComprasEnPuntos { get; set; }

    public Decimal TotalCanjes { get; set; }

    public Decimal TotalCanjesEnPuntos { get; set; }

    public Decimal NumeroBeneficiosUsados { get; set; }

    public Decimal TotalBeneficios { get; internal set; }
  }
}
