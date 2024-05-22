// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.HomeViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using MystiqueMC.DAL;
using MystiqueMC.Models.Graficas;
using System.Collections.Generic;


namespace MystiqueMC.Models
{
  public class HomeViewModel
  {
    public List<comercios> Comercios { get; set; }

    public List<sucursales> Sucursales { get; set; }

    public Grafica GraficaUno { get; set; }

    public Grafica GraficaDos { get; set; }

    public Grafica GraficaTres { get; set; }

    public Resumen ResumenCompras { get; set; }

    public empresas Empresa { get; set; }
  }
}
