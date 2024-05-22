// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Gastos.GastoPvValidarViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models.Gastos
{
  public class GastoPvValidarViewModel
  {
    public int IdGastoPv { get; set; }

    public int VentaId { get; set; }

    public DateTime FechaVenta { get; set; }

    public string FechaVentaFormateada => this.FechaVenta.ToString("dd/MM/yyyy");

    public int? FolioVenta { get; set; }

    public int SucursalId { get; set; }

    public string Sucursal { get; set; }

    public string UsuarioRegistro { get; set; }

    public Decimal Monto { get; set; }

    public Decimal MontoValidado { get; set; }

    public string MontoFormateado => this.Monto.ToString("N2");

    public DateTime FechaRegistro { get; set; }

    public DateTime FechaGasto { get; set; }

    public string Observaciones { get; set; }

    public string TipoGasto { get; set; }

    public string ObservacionesConcat
    {
      get => string.Format("{0}  -  {1}", (object) this.Observaciones, (object) this.TipoGasto);
    }

    public bool Aplicado { get; set; }

    public string ObservacionesValidar { get; set; }

    public int catRubroId { get; set; }

    public int catConceptoGastoId { get; set; }

    public int? ProveedorId { get; set; }

    public int InsumoId { get; set; }

    public Decimal? Cantidad { get; set; }

    public string NoRemision { get; set; }

    public string NoFactura { get; set; }

    public Decimal? Descuento { get; set; }

    public Decimal? IVA { get; set; }
  }
}
