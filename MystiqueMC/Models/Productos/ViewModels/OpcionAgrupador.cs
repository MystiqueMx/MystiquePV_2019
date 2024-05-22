// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Productos.ViewModels.OpcionAgrupador
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using MystiqueMC.DAL;

namespace WebApp.Web.Models.Productos.ViewModels
{
  public class OpcionAgrupador
  {
    public int Id { get; set; }

    public MystiqueMC.DAL.Productos Producto { get; set; }

    public Insumos Insumo { get; set; }
  }
}
