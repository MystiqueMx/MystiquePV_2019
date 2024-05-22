// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Compras.ComprasIndexViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace WebApp.Web.Models.Compras
{
  public class ComprasIndexViewModel
  {
    public DateTime FechaCaptura { get; set; }

    public string Proveedor { get; set; }

    public string Sucursal { get; set; }

    public string Observaciones { get; set; }

    public Decimal Total { get; set; }

    public string Estatus { get; set; }

    public int Id { get; set; }

    public bool Editable { get; set; }

    public string FechaCapturaFormateada => this.FechaCaptura.ToString("dd/MM/yyyy hh:mm tt");

    public string TotalFormateado => this.Total.ToString("N2");
  }
}
