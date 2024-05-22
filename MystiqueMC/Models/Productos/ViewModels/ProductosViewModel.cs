// Decompiled with JetBrains decompiler
// Type: WebApp.Web.Models.Productos.ProductosViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using MystiqueMC.Controllers;
using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebApp.Web.Models.Productos.ViewModels;


namespace WebApp.Web.Models.Productos
{
  public class ProductosViewModel
  {
    public int IdProducto { get; set; }

    public string Nombre { get; set; }

    public Decimal Precio { get; set; }

    [DisplayName("Estatus")]
    public bool Activo { get; set; }

    [DisplayName("Merma Permitida")]
    public Decimal? MermaPermitida { get; set; }

    [Required]
    [DisplayName("Tipo de producto")]
    public int TipoProducto { get; set; }

    [DisplayName("Es General")]
    public bool EsGeneral { get; set; }

    public string Clave { get; set; }

    [DisplayName("Armar Cobro Mostrador")]
    public bool ArmarCobro { get; set; }

    public bool Principal { get; set; }

    public bool TieneReceta { get; set; }

    public string Imagen { get; set; }

    public int? AreaPreparacionId { get; set; }

    public int indice { get; set; }

    public int CategoriaProductoId { get; set; }

    public int? IdReceta { get; set; }

    public int Tipo { get; set; } = 1;

    public List<DetalleRecetaProducto> DetalleRecetas { get; set; } = new List<DetalleRecetaProducto>();

    public List<VariedadesViewModel> Variedades { get; set; } = new List<VariedadesViewModel>();

    public ConfigurarProductoViewModel Configuracion { get; set; }

    public bool ForceRefresh { get; set; }
        internal List<PreciosSucursalesViewModel> PreciosSucursales { get; set; }
    }
}
