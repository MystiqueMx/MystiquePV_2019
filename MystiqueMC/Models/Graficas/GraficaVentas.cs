// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Graficas.GraficaVentas
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;

namespace MystiqueMC.Models.Graficas
{
  public class GraficaVentas
  {
    public int IdSucursal { get; set; }

    public string Nombre { get; set; }

    public Decimal Valor { get; set; }

    public string Etiqueta { get; set; }

    public string Color { get; set; }
  }
}
