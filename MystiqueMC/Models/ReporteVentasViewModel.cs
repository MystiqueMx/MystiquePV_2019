// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.ReporteVentasViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models
{
  public class ReporteVentasViewModel
  {
    public int cveVenta { get; set; }

    public int? semana { get; set; }

    public int? numero { get; set; }

    public string dia { get; set; }

    public int? comensales { get; set; }

    public Decimal? ventasTotales { get; set; }

    public DateTime fechaInicial { get; set; }

    public DateTime? fechaFinal { get; set; }
  }
}
