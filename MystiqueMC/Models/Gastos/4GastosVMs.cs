// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Gastos.GastosPvViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models.Gastos
{
  public class GastosPvViewModel
  {
    public int IdGastoPv { get; set; }

    public int VentaId { get; set; }

    public int SucursalId { get; set; }

    public int GastoIdSucursal { get; set; }

    public int AperturaId { get; set; }

    public string UsuarioRegistro { get; set; }

    public Decimal Monto { get; set; }

    public string MontoFormateado => this.Monto.ToString("N2");

    public DateTime FechaRegistro { get; set; }

    public string Observaciones { get; set; }

    public string TipoGasto { get; set; }

    public string ObservacionesConcat
    {
      get => string.Format("{0} {1}", (object) this.Observaciones, (object) this.TipoGasto);
    }

    public bool Aplicado { get; set; }
  }
}
