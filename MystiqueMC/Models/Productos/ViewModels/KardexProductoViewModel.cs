// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Productos.ViewModels.KardexProductoViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WebApp.Web.Models.Productos.ViewModels
{
  public class KardexProductoViewModel
  {
    public int IdVariedadesProducto { get; set; }

    public int ProductoId { get; set; }

    [Required]
    public string Descripción { get; set; }

    [Required]
    public string Imagen { get; set; }

    public bool TieneReceta { get; set; }

    public List<MystiqueMC.DAL.VariedadProductos> VariedadProductos { get; set; }
  }
}
