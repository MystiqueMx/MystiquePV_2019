using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Entradas
{
    public class RequestLogin : RequestBase
    {
        public string deviceId { get; set; }
        public string deviceModel { get; set; }
        public string devicePlatform { get; set; }
        public string deviceVersion { get; set; }
        public string deviceConnectionType { get; set; }
        public string playerId { get; set; }
        public int empresaId { get; set; }
        public int tipoAutentificacionId { get; set; }

    }

    public class RequestLogout : RequestBase
    {
        public string playerId { get; set; }
        public int empresaId { get; set; }
    }


    public class RequestLoginActivoId : RequestBase
    {
        public int usuarioId { get; set; }

    }

    public class RequestLoginPorId : RequestBase
    {
        public int usuarioId { get; set; }
        public int loginId { get; set; }

    }

    public class RequestLoginSesionUsuarioDispositivo : RequestBase
    {
        public int usuarioId { get; set; }
        public string deviceId { get; set; }

    }

    public class RequestLoginRevisarNombreDisponible : RequestBase
    {
        public string userName { get; set; }
    }


    public class RequestLoginFB : RequestBase
    {
        public string deviceId { get; set; }
        public string deviceModel { get; set; }
        public string devicePlatform { get; set; }
        public string deviceVersion { get; set; }
        public string deviceConnectionType { get; set; }
        public string playerId { get; set; }
        public string facebookId { get; set; }
        public int empresaId { get; set; }

    }

}