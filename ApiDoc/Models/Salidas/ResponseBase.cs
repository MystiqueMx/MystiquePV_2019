using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseBase
    {
        public ResponseBase()
        {
        }

        public ResponseBase(bool Success)
        {
            this.Success = Success;
        }
        public ResponseBase(bool Success, string ErrorMessage)
        {
            this.Success = Success;
            this.ErrorMessage = ErrorMessage;
        }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public string correoElectronico { get; set; }
        public string contrasenia { get; set; }
    }
}