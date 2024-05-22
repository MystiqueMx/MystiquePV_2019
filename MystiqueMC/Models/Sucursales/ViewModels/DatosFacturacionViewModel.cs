using MystiqueMC.Helpers.DataAnnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MystiqueMC.Models.Sucursales.ViewModels
{
    public class DatosFacturacionViewModel
    {
        public int EmpresaId { get; set; }
        public string LabelEmpresa { get; set; }
        public string LabelComercio { get; set; }
        public string Nombre { get; set; }

        [Required]
        public int ComercioId { get; set; }

        [Required]
        public int IdSucursal { get; set; }

        [Required]
        [DisplayName("Asignar datos fiscales")]
        public int EstatusDatosFiscales { get; set; }

        [RequiredIfIntEquals((int)Views.EstatusDatosFiscales.Catalogo, "EstatusDatosFiscales")]
        [DisplayName("Datos fiscales de la sucursal")]
        public int? IdDatosFiscales { get; set; }

        [RequiredIfIntEquals((int)Views.EstatusDatosFiscales.Nuevos, "EstatusDatosFiscales")]
        [DisplayName("Razón social")]
        public string FacturacionNombre { get; set; }

        [RequiredIfIntEquals((int)Views.EstatusDatosFiscales.Nuevos, "EstatusDatosFiscales")]
        [DisplayName("R.F.C.")]
        public string FacturacionRfc { get; set; }

        [RequiredIfIntEquals((int)Views.EstatusDatosFiscales.Nuevos, "EstatusDatosFiscales")]
        [DisplayName("Código postal")]
        public string FacturacionCodigoPostal { get; set; }

        [RequiredIfIntEquals((int)Views.EstatusDatosFiscales.Nuevos, "EstatusDatosFiscales")]
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