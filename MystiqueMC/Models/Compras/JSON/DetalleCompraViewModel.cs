// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Compras.JSON.DetalleCompraViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll


namespace WebApp.Web.Models.Compras.JSON
{
  public class DetalleCompraViewModel
  {
    public string descuentos { get; set; }

    public string iva { get; set; }

    public string total { get; set; }

    public string sucursal { get; set; }

    public string proveedor { get; set; }

    public string remision { get; set; }

    public string factura { get; set; }

    public string observacion { get; set; }

    public DetalleCompraJson[] detalle { get; set; }

    public string fechaCompra { get; set; }
  }
}
