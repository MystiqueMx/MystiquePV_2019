// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Productos.Requests.AgrupadorNuevo
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;


namespace WebApp.Web.Models.Productos.Requests
{
  public class AgrupadorNuevo
  {
    public int? Id { get; set; }

    public int IdProducto { get; set; }

    public int Tipo { get; set; }

    public string Descripcion { get; set; }

    public int Cantidad { get; set; }

    public int Indice { get; set; }

    public int PuedeAgregarExtra { get; set; }

    public Decimal? CostoExtra { get; set; }

    public int DebeConfirmarPorSeparado { get; set; }

    public int[] Opciones { get; set; } = new int[0];

    public int familia { get; set; }
  }
}
