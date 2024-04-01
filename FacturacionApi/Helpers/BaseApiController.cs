using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FacturacionApi.DAL;
using FacturacionApi.PaxFacturacion;

public abstract class BaseApiController : ApiController
    {
        #region Version Facturacion
        protected const string VersionFacturacion = "3.3";
        #endregion 
        #region Contexto
        private FacturacionContext _context;
        #if DEBUG
        protected FacturacionContext Contexto
        {
            get
            {
                if (_context != null) return _context;
                _context = new FacturacionContext();
                _context.Database.Log = sql => Logger.Debug(sql);
                return _context;
            }
        }
        #else
        public FacturacionContext Contexto => _context ?? (_context = new FacturacionContext());
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
        #endregion

        #region Responses
        private readonly string _mensajeNoPermisos = ConfigurationManager.AppSettings.Get("MENSAJE_NO_PERMISOS");
        private readonly string _mensajeNoAutentificado = ConfigurationManager.AppSettings.Get("MENSAJE_NO_AUTENTIFICADO");
        private readonly string _mensajeErrorServidor = ConfigurationManager.AppSettings.Get("MENSAJE_ERROR_SERVIDOR");
        protected ErrorCodeResponseBase RespuestaNoPermisos => new ErrorCodeResponseBase
        {
            Message = _mensajeNoPermisos,
            ResponseCode = (int) ResponseTypes.CodigoPermisos,
        };
        protected ErrorCodeResponseBase RespuestaNoAutentificado => new ErrorCodeResponseBase
        {
            Message = _mensajeNoAutentificado,
            ResponseCode = (int) ResponseTypes.CodigoAutentificacion,
        };
        protected ErrorCodeResponseBase RespuestaOkMensaje(string mensaje) => new ErrorCodeResponseBase
        {
            ResponseCode = (int) ResponseTypes.CodigoOk,
            Message = mensaje
        };
        protected ErrorCodeResponseBase RespuestaOk => new ErrorCodeResponseBase
        {
            ResponseCode = (int) ResponseTypes.CodigoOk,
        };
        protected ErrorCodeResponseBase RespuestaErrorInterno => new ErrorCodeResponseBase
        {
            Message = _mensajeErrorServidor,
            ResponseCode = (int) ResponseTypes.CodigoExcepcion,
        };
        protected static ErrorCodeResponseBase RespuestaErrorValidacion(ModelStateDictionary modelState) => new ErrorCodeResponseBase
        {
            Message = string.Join(", ", modelState.Select( f=> $"({f.Key} : { string.Join(", ",f.Value.Errors.Select(c=> $" {c.ErrorMessage} ").ToArray())})" ).ToArray()),
            ResponseCode = (int) ResponseTypes.CodigoValidacion,
        };
        protected static ErrorCodeResponseBase RespuestaErrorValidacion(string errors) => new ErrorCodeResponseBase
        {
            Message = errors,
            ResponseCode = (int) ResponseTypes.CodigoValidacion,
        };
        #endregion

        #region Logger

        protected static readonly log4net.ILog Logger 
            = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region PaxFacturacion
        private readonly string _servicioPaxFacturacion = ConfigurationManager.AppSettings.Get("URL_PAX_FACTURACION");
        public wcfRecepcionASMXSoapClient NuevoClientePaxFacturacion =>
            new wcfRecepcionASMXSoapClient(HttpBindingPaxFacturacion, new EndpointAddress(_servicioPaxFacturacion));

        private static BasicHttpBinding HttpBindingPaxFacturacion
        {
            get
            {
                var httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                httpBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                httpBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
                httpBinding.Security.Mode = BasicHttpSecurityMode.Transport;
                return httpBinding;
            }
        }
        #endregion
    }
