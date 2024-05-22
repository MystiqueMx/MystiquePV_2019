using System;
using System.Collections.Generic;

namespace ApiDoc.Models.Salidas
{
    public class DetalleSucursalResponse : ResponseBase
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string Imagen { get; set; }
        public IEnumerable<HorarioSucursal> HorarioSemanal { get; set; }
        public HorarioSucursal HorarioHoy { get; set; }
        public string EmailContacto { get; set; }
        public string Telefono { get; set; }
        public TipoSucursales Tipo { get; set; }
        public IEnumerable<BeneficioSucursal> Beneficios { get; set; }
        public Anexo Doctor { get; set; }
    }

    public class HorarioSucursal
    {
        public System.DayOfWeek Dia { get; set; }
        public string Horario { get; set; }
    }
    public class BeneficioSucursal
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string Codigo { get; set; }
        public string Vigencia { get; set; }
    }
    public class Anexo
    {
        public string Descripcion { get; set; }
        public string Especialidad { get; set; }
        public decimal? PrecioConsulta { get; set; }
        public string Cedula { get; set; }
        public IEnumerable<string> Imagenes { get; set; }
        public IEnumerable<string> ImagenesAseguranzas { get; set; }
    }
}