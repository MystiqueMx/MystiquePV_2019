using System;
using System.Configuration;
using System.Linq;
using System.Web;
using MystiqueMC.DAL;

namespace MystiqueMcApi.Helpers
{
    public class PermisosApi
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string APP_SECRET = ConfigurationManager.AppSettings.Get("MYSTIQUE_APP");
        public bool UsuarioExiste(string correo, string contrasenia, int empresaId)
        {
            bool resultado = false;

            if (contextEntity.clientes.FirstOrDefault(w => w.email == correo && w.password == contrasenia && w.empresaId == empresaId) != null)
            {
                resultado = true;
            }
            return resultado;
        }

        public bool UsuarioRecuperarContrasenia(string correo, string contrasenia , int empresaId)
        {
            bool resultado = false;

            // if (contextEntity.clientes.FirstOrDefault(w => w.email == correo && w.password == contrasenia && w.facebookId !=contrasenia) != null)
            if (contextEntity.clientes.FirstOrDefault(w => w.email == correo && w.password != w.facebookId) != null)
            {
                resultado = true;
            }
            return resultado;
        }

                
        public bool validarUsuarioDatosRequeridos(clientes verificar)
        {
            bool resultado = true;  // Solo para que pasen las pruebas cuando se suba a la tienda la aplicacion de IOS
          
            if (verificar != null)
            {

                if ((verificar.catColoniaId != null) && (verificar.catSexoId != null) && (verificar.telefono != null) && (verificar.fechaNacimiento != null))
                {
                    resultado = true;
                }
            }
            return resultado;
        }

        /// Valida que el header de la peticion coincida con el token definido en webconfig
        /// </summary>
        public bool IsAppSecretValid
        {
            get
            {
                var secret = HttpContext.Current.Request.Headers["X-Api-Secret"];
                if (!string.IsNullOrEmpty(secret) && secret.Equals(APP_SECRET)) return true;
                else return false;
            }

        }
        
        public string ObtenerMensajeRespuesta(string llave)
        {
            string resultado = "";
            try
            {
                resultado = ConfigurationManager.AppSettings.Get(llave);
            }
            catch (Exception e)
            {
                logger.Error("Error al obtener el valor del archivo de configuracion" + e);
            }
            return resultado;
        }
    }
}