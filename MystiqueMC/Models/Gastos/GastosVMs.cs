// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Gastos.GastosIndexViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models.Gastos
{
  public class GastosIndexViewModel
  {
    public int IdGasto { get; set; }

    public int SucursalId { get; set; }

    public string Sucursal { get; set; }

    public int catConceptoGastoId { get; set; }

    public string catConceptoGasto { get; set; }

    public int? ProveedorId { get; set; }

    public string Proveedor { get; set; }

    public Decimal Monto { get; set; }

    public string Observacion { get; set; }

    public DateTime FechaGasto { get; set; }

    public string FechaGastoFormateada => this.FechaGasto.ToString("dd/MM/yyyy");

    public string MontoFormateado => this.Monto.ToString("N2");
  }
}
