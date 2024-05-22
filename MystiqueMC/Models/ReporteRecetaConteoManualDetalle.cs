// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.ReporteRecetaConteoManualDetalle
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models
{
  public class ReporteRecetaConteoManualDetalle
  {
    public string Anio { get; set; }

    public int Semana { get; set; }

    public string DiaSemana { get; set; }

    public DateTime? Fecha { get; set; }

    public string UnidadMedida { get; set; }

    public int IdInsumo { get; set; }

    public string Nombre { get; set; }

    public Decimal TotalProductosVenta { get; set; }

    public Decimal TotalInventarioManual { get; set; }

    public Decimal Diferencia { get; set; }
  }
}
