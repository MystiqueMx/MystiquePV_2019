using System;

namespace ApiDoc.Models.Entradas
{
    public class RequestMisCitas : RequestBase
    {
        public int Cliente { get; set; }
    }
    public class RequestMisCitasDoctor : RequestBase
    {
        public int Doctor { get; set; }
        public DateTime? fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public EstatusCitas estatus { get; set; }
    }
    public class RequestCita : RequestBase
    {
        public int Cliente { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime Fecha { get; set; }
        public int Doctor { get; set; }
        public string PlayerId { get; set; }
    }
    public class RequestDoctorCita : RequestBase
    {
        public int Cita { get; set; }
        public EstatusCitas estatus { get; set; }
        public DateTime Fecha { get; set; }
        public string Observacion { get; set; }
        public bool? IsBeneficioEscaneado { get; set; }
        public string CadenaCodigo { get; set; }
    }
}