// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Productos.ViewModels.ConfigurarProductoViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll


namespace WebApp.Web.Models.Productos.ViewModels
{
  public class ConfigurarProductoViewModel
  {
    public int IdProducto { get; set; }

    public int Tipo { get; set; }

    public string Nombre { get; set; }

    public Agrupador[] Agrupadores { get; set; } = new Agrupador[0];
  }
}
