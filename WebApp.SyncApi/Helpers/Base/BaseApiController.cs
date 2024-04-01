using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using MystiqueMC.DAL;
using WebApp.SyncApi.Helpers;
using WebApp.SyncApi.Helpers.Base;

namespace WebApp.SyncApi.Helpers.Base
{
    public abstract class BaseApiController : ApiController
    {
        #region CONTEXT
        private MystiqueMeEntities _context;
#if DEBUG
        protected MystiqueMeEntities Contexto
        {
            get
            {
                if (_context != null) return _context;
                _context = new MystiqueMeEntities();
                _context.Configuration.LazyLoadingEnabled = false;
                _context.Configuration.ProxyCreationEnabled = false;
                _context.Database.Log = sql => Trace.WriteLine(sql);
                return _context;
            }
        }
#else
        protected MystiqueMeEntities Contexto
        {
            get
            {
                if (_context != null) return _context;
                _context = new MystiqueMeEntities();
                _context.Configuration.LazyLoadingEnabled = false;
                _context.Configuration.ProxyCreationEnabled = false;
                return _context;
            }
        }
#endif
        #endregion

        #region Validaciones
        private readonly string _appSecret = ConfigurationManager.AppSettings.Get("HEADER_APP_SECRET");

        /// <summary>
        /// Valida que el header de la peticion coincida con el token definido en webconfig
        /// </summary>
        protected bool IsAppSecretValid
        {
            get
            {
                var secret = HttpContext.Current.Request.Headers["X-Api-Secret"];
                return !string.IsNullOrEmpty(secret)
                       && secret.Equals(_appSecret);
            }
        }

        protected bool IsAuthenticated => RequestContext.Principal != null;

        #endregion

        #region Responses
        private readonly string _mensajeNoPermisos = ConfigurationManager.AppSettings.Get("MENSAJE_NO_PERMISOS");
        private readonly string _mensajeNoAutentificado = ConfigurationManager.AppSettings.Get("MENSAJE_NO_AUTENTIFICADO");
        private readonly string _mensajeErrorServidor = ConfigurationManager.AppSettings.Get("MENSAJE_ERROR_SERVIDOR");
        protected ErrorCodeResponseBase RespuestaNoPermisos => new ErrorCodeResponseBase
        {
            Message = _mensajeNoPermisos,
            ResponseCode = (int)ResponseTypes.CodigoPermisos,
        };
        protected ErrorCodeResponseBase RespuestaNoAutentificado => new ErrorCodeResponseBase
        {
            Message = _mensajeNoAutentificado,
            ResponseCode = (int)ResponseTypes.CodigoAutentificacion,
        };
        protected ErrorCodeResponseBase RespuestaOkMensaje(string mensaje) => new ErrorCodeResponseBase
        {
            ResponseCode = (int)ResponseTypes.CodigoOk,
            Message = mensaje
        };
        protected ErrorCodeResponseBase RespuestaOk => new ErrorCodeResponseBase
        {
            ResponseCode = (int)ResponseTypes.CodigoOk,
        };
        protected ErrorCodeResponseBase RespuestaErrorInterno => new ErrorCodeResponseBase
        {
            Message = _mensajeErrorServidor,
            ResponseCode = (int)ResponseTypes.CodigoExcepcion,
        };
        protected static ErrorCodeResponseBase RespuestaErrorValidacion(ModelStateDictionary modelState) => new ErrorCodeResponseBase
        {
            Message = string.Join(", ", modelState.Select(f => $"({f.Key} : { string.Join(", ", f.Value.Errors.Select(c => $" {c.ErrorMessage} ").ToArray())})").ToArray()),
            ResponseCode = (int)ResponseTypes.CodigoValidacion,
        };
        protected static ErrorCodeResponseBase RespuestaErrorValidacion(string errors) => new ErrorCodeResponseBase
        {
            Message = errors,
            ResponseCode = (int)ResponseTypes.CodigoValidacion,
        };
        #endregion

        #region Logger

        protected static readonly log4net.ILog Logger
            = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region USER
        public int CurrentUserId
        {
            get
            {
                var usrClaims = (ClaimsIdentity)RequestContext.Principal.Identity;
                var user = usrClaims.FindFirst(c => c.Type == "user");
                if (user != null && int.TryParse(user.Value, out var userId))
                {
                    return userId;
                }
                return 0;

            }
        }
        public string CurrentUserRole
        {
            get
            {
                var usrClaims = (ClaimsIdentity)RequestContext.Principal.Identity;
                var claim = usrClaims.FindFirst(c => c.Type == "role");
                return claim.Value;

            }
        }
        public string CurrentUserName
        {
            get
            {
                var usrClaims = (ClaimsIdentity)RequestContext.Principal.Identity;
                var claim = usrClaims.FindFirst(c => c.Type == "sub");
                return claim.Value;

            }
        }
        public usuarios CurrentUser
        {
            get
            {
                if (CurrentUserId > 0 && Contexto.usuarios.Any(c => c.idUsuario == CurrentUserId))
                {
                    return Contexto.usuarios.First(c => c.idUsuario == CurrentUserId);
                }
                return null;

            }
        }
        #endregion
    }

}
