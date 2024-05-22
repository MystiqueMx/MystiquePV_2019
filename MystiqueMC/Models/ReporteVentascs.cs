// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.ReporteVentascs
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;

namespace MystiqueMC.Models
{
  public class ReporteVentascs
  {
    public string cveVenta { get; set; }

    public string semana { get; set; }

    public string numero { get; set; }

    public string dia { get; set; }

    public DateTime fechaInicio { get; set; }

    public DateTime fechaFin { get; set; }

    public int ventasTotales { get; set; }

    public string comensales { get; set; }
  }
}
