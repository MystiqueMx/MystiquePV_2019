// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Productos.ViewModels.Agrupador
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using System;


namespace WebApp.Web.Models.Productos.ViewModels
{
  public class Agrupador
  {
    public int Id { get; set; }

    public int Cantidad { get; set; }

    public string Descripcion { get; set; }

    public int Indice { get; set; }

    public bool PuedeAgregarExtra { get; set; }

    public bool DebeConfirmarPorSeparado { get; set; }

    public Decimal? CostoExtra { get; set; }

    public OpcionAgrupador[] Opciones { get; set; } = new OpcionAgrupador[0];
  }
}
