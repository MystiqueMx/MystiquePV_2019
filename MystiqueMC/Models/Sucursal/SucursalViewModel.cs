// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Sucursal.SucursalViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using MystiqueMC.DAL;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace MystiqueMC.Models.Sucursal
{
  public class SucursalViewModel
  {
    [Required]
    public int idSucursal { get; set; }

    [Required]
    public int comercioId { get; set; }

    [Required]
    public string nombre { get; set; }

    [Required]
    public string telefono { get; set; }

    [Required]
    public int direccionSucursal { get; set; }

    public string latitud { get; set; }

    public string longitud { get; set; }

    public string placeId { get; set; }

    public int radioZonaMystique { get; set; }

    public int sucursalPuntoVenta { get; set; }

    [Required]
    public int idDatoFiscal { get; set; }

    public string razonFiscal { get; set; }

    public string rfc { get; set; }

    public int direccionFiscal { get; set; }

    public string codigoP { get; set; }

    public int catRegimenFiscalId { get; set; }

    [Required]
    public int idDireccion { get; set; }

    [Required]
    public string calle { get; set; }

    [Required]
    public string colonia { get; set; }

    public string NoExterior { get; set; }

    public string NoInterior { get; set; }

    public string codigoPostal { get; set; }

    public int ciudadId { get; set; }

    public int estadoId { get; set; }

    public string entreCalles { get; set; }

    [Required]
    public int idSucursalHorarioLunes { get; set; }

    public string diaLunes { get; set; }

    public string HorarioInicioLunes { get; set; }

    public string HorarioFinLunes { get; set; }

    public string descripcionLunes { get; set; }

    [Required]
    public int idSucursalHorarioMartes { get; set; }

    public string diaMartes { get; set; }

    public string HorarioInicioMartes { get; set; }

    public string HorarioFinMartes { get; set; }

    public string descripcionMartes { get; set; }

    [Required]
    public int idSucursalHorarioMiercoles { get; set; }

    public string diaMiercoles { get; set; }

    public string HorarioInicioMiercoles { get; set; }

    public string HorarioFinMiercoles { get; set; }

    public string descripcionMiercoles { get; set; }

    [Required]
    public int idSucursalHorarioJueves { get; set; }

    public string diaJueves { get; set; }

    public string HorarioInicioJueves { get; set; }

    public string HorarioFinJueves { get; set; }

    public string descripcionJueves { get; set; }

    [Required]
    public int idSucursalHorarioViernes { get; set; }

    public string diaViernes { get; set; }

    public string HorarioInicioViernes { get; set; }

    public string HorarioFinViernes { get; set; }

    public string descripcionViernes { get; set; }

    [Required]
    public int idSucursalHorarioSabado { get; set; }

    public string diaSabado { get; set; }

    public string HorarioInicioSabado { get; set; }

    public string HorarioFinSabado { get; set; }

    public string descripcionSabado { get; set; }

    [Required]
    public int idSucursalHorarioDomingo { get; set; }

    public string diaDomingo { get; set; }

    public string HorarioInicioDomingo { get; set; }

    public string HorarioFinDomingo { get; set; }

    public string descripcionDomingo { get; set; }

    public datosFiscales idDatoFiscalNavigation { get; set; }

    public sucursales idSucursalNavigation { get; set; }

    [Required]
    public int? catZonaId { get; set; }

    public string ColorIndicador { get; set; }

    public int activo { get; set; }

    public IEnumerable<SelectListItem> Statuses { get; set; }
  }
}
