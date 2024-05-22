using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class CitasDoctorResponse : ResponseBase
    {
        public IEnumerable<CitaDoctor> Data { get; set; }
    }
    public class CitasResponse : ResponseBase
    {
        public IEnumerable<CitaCliente> Data { get; set; }
    }
    public class CitaCliente
    {
        public int Id { get; set; }
        public string Doctor { get; set; }
        public int Estatus { get; set; }
        public DateTime Fecha { get; set; }
        public string Direccion { get; set; }
        public string Observacion { get; set; }
        public string TelefonoContacto { get; set; }
    }
    public class CitaDoctor
    {
        public int Id { get; set; }
        public string Cliente { get; set; }
        public string FotoCliente { get; set; }
        public int Estatus { get; set; }
        public DateTime Fecha { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string RangoEdad { get; set; }
        public int? Sexo { get; set; }
    }
}