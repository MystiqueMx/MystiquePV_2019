using MystiqueMC.DAL;
using ApiDoc.Helpers;
using ApiDoc.Models.Entradas;
using ApiDoc.Models.Salidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Http;

namespace ApiDoc.Controllers
{
    public class LoginDoctorController : ApiController
    {
        private MystiqueMeEntities contextEntity = new MystiqueMeEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PermisosApi validar = new PermisosApi();
        readonly string MENSAJE_NO_PERMISOS = "MYSTIQUE_MENSAJE_NO_PERMISOS";
        readonly string MENSAJE_ERROR_SERVIDOR = "MYSTIQUE_MENSAJE_ERROR_SERVIDOR";
        readonly string MENSAJE_USUARIO_NO_REGISTRADO = "MYSTIQUE_MENSAJE_USUARIO_NO_REGISTRADO";
        readonly string MENSAJE_USUARIO_BLOQUEADO = "MYSTIQUE_MENSAJE_USUARIO_BLOQUEADO";
        readonly string MENSAJE_OTRO_METODO_AUTENTIFICACION = "MYSTIQUE_MENSAJE_OTRO_METODO_AUTENTIFICACION";



    }
}