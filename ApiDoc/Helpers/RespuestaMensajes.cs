using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ApiDoc.Helpers
{
    public class RespuestaMensajes
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string ObtenerMensajeRespuesta(string llave)
        {
            string resultado = "";
            try
            { 
                 resultado = ConfigurationManager.AppSettings.Get(llave);
            }
            catch(Exception e)
            {
                logger.Error("Error al obtener el valor del archivo de configuracion"+e);
            }
            return resultado;
        }
    }
}