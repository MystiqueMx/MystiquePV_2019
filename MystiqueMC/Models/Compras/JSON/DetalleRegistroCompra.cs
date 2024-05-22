// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Compras.JSON.DetalleRegistroCompra
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace WebApp.Web.Models.Compras.JSON
{
  public class DetalleRegistroCompra
  {
    public int idDetalleCompra { get; set; }

    public Decimal importe { get; set; }

    public int cantidad { get; set; }

    public int insumo { get; set; }

    public int? compra { get; set; }

    public string desc { get; set; }

    public string unidad { get; set; }

    public string unidadCompraId { get; set; }
  }
}
