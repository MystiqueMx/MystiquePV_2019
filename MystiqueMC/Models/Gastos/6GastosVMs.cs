// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Gastos.GastoPvDetalleViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace MystiqueMC.Models.Gastos
{
  public class GastoPvDetalleViewModel
  {
    public int IdGastoPvDetalle { get; set; }

    public int GastoPvId { get; set; }

    public int SucursalId { get; set; }

    public int? ProveedorId { get; set; }

    public int catRubroId { get; set; }

    public int catConceptoGastoId { get; set; }

    public int? InsumoId { get; set; }

    public Decimal? Cantidad { get; set; }

    public Decimal Monto { get; set; }

    public string UsuarioRegistroId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public int VentaId { get; set; }
  }
}
