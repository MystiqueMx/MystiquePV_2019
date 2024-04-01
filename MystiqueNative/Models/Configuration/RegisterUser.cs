using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Configuration
{
    public class RegisterUser
    {
        public AuthMethods Metodo { get; set; }
        public string FbId { get; set; }
        public string Nombre { get; set; }
        public string Materno { get; set; }
        public string Paterno { get; set; }
        public string FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string Telefono { get; set; }
        public string Colonia { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
