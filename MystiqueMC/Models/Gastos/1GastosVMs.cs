// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Gastos.GastosIndexValidarViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models.Gastos
{
  public class GastosIndexValidarViewModel
  {
    public int IdVenta { get; set; }

    public DateTime FechaRegistroVenta { get; set; }

    public string FechaRegistroVentaFormateada => this.FechaRegistroVenta.ToString("dd/MM/yyyy");

    public int SucursalId { get; set; }

    public string Sucursal { get; set; }

    public int? Folio { get; set; }

    public Decimal? Monto { get; set; }

    public string MontoFormateado => this.Monto.Value.ToString("N2");
  }
}
