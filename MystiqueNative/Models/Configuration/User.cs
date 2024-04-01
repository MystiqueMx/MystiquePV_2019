using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Configuration
{
    public class User
    {
        public string Id { get; set; }
        public string IdMembresia { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public AuthMethods AuthMethod { get; set; }
        public string Telefono { get; set; }
    }
}
