// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Compras.JSON.RegistroCompraViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace WebApp.Web.Models.Compras.JSON
{
  public class RegistroCompraViewModel
  {
    public int? id { get; set; }

    public Decimal descuentos { get; set; }

    public Decimal iva { get; set; }

    public Decimal total { get; set; }

    public int sucursal { get; set; }

    public int proveedor { get; set; }

    public string remision { get; set; }

    public string factura { get; set; }

    public string observacion { get; set; }

    public DateTime? fechaCompra { get; set; }

    public int estatusCompra { get; set; }

    public DetalleRegistroCompra[] detalle { get; set; } = new DetalleRegistroCompra[0];
  }
}
