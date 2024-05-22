// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Sucursal.DatosFacturacionViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using MystiqueMC.Helpers.DataAnnotation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace MystiqueMC.Models.Sucursal
{
  public class DatosFacturacionViewModel
  {
    public int EmpresaId { get; set; }

    public string LabelEmpresa { get; set; }

    public string LabelComercio { get; set; }

    public string Nombre { get; set; }

    public int? direccionId { get; set; }

    public int? datosFiscalesId { get; set; }

    [Required]
    public int ComercioId { get; set; }

    [Required]
    public int IdSucursal { get; set; }

    [Required]
    [DisplayName("Asignar datos fiscales")]
    public int EstatusDatosFiscalesId { get; set; }

    [DisplayName("Datos fiscales de la sucursal")]
    public int? IdDatosFiscales { get; set; }

    [RequiredIfIntEquals(2, "EstatusDatosFiscales")]
    [DisplayName("Razón social")]
    public string FacturacionNombre { get; set; }

    [RequiredIfIntEquals(2, "EstatusDatosFiscales")]
    [DisplayName("R.F.C.")]
    public string FacturacionRfc { get; set; }

    [RequiredIfIntEquals(2, "EstatusDatosFiscales")]
    [DisplayName("Código postal")]
    public string FacturacionCodigoPostal { get; set; }

    [RequiredIfIntEquals(2, "EstatusDatosFiscales")]
    [DisplayName("Régimen fiscal")]
    public int? FacturacionRegimenFiscal { get; set; }

    [DisplayName("Calle")]
    public string FacturacionCalle { get; set; }

    [DisplayName("Colonia")]
    public string FacturacionColonia { get; set; }

    [DisplayName("Número")]
    public string FacturacionNumero { get; set; }

    [DisplayName("Ciudad")]
    public int? FacturacionIdCiudad { get; set; }

    [DisplayName("Estado")]
    public int? FacturacionIdEstado { get; set; }
  }
}
