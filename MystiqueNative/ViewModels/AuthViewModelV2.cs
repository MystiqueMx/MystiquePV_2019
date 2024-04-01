using MystiqueNative.Configuration;
using MystiqueNative.Helpers;
using MystiqueNative.Models;
using MystiqueNative.Models.Configuration;
using MystiqueNative.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class AuthViewModelV2 : BaseViewModel
    {
        #region SINGLETON
        public static AuthViewModelV2 Instance => _instance ?? (_instance = new AuthViewModelV2());
        private static AuthViewModelV2 _instance;
        public static void Destroy() => _instance = null;
        #endregion
        #region EVENTS
        public event EventHandler<LoginEventArgs> OnIniciarSesionFinished;
        public event EventHandler<ConsultarColoniasEventArgs> OnConsultarColoniasFinished;
        public event EventHandler<RecuperarPasswordEventArgs> OnRecuperarContrasenaFinished;
        public event EventHandler<RegistrarEventArgs> OnRegistrarFailed;
        public event EventHandler<PictureUploadEventArgs> OnPictureUploadFailed;
        public event EventHandler<ActualizarPerfilEventArgs> OnActualizarPerfilFinished;
        #endregion
        #region FIELDS  
        public Profile Usuario { get => _usuario; private set { _usuario = value; OnPropertyChanged("Usuario"); } }
        public List<string> Colonias { get; } = new List<string>();
        private Profile _usuario;
        private readonly List<string> _coloniasId = new List<string>();
        
        public string ProfilePictureUrl => Usuario == null || string.IsNullOrEmpty(Usuario.Foto)
            ? string.Empty 
            : $"{MystiqueApiV2Config.DownloadPicturePath}?p={Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Usuario.Foto))}";

        #endregion
        #region API
        public async Task<bool> IniciarSesion(AuthMethods method, string username, string password, string nombre = null, string paterno = null)
        {
            IsBusy = true;
            var config = await MystiqueApiV2.Configuracion.CallObtenerConfiguracion();

            if (config.Success) // Obtener Configuracion del aplicativo
            {
                SetConfiguracion(config);
                
                var response = await MystiqueApiV2.Account.CallLoginV2(method, username, password);
                if (response.Success) // Iniciar Sesion
                {
                    SetUser(username, password, response, method);
                    
                    OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                    {
                        Success = true, 
                        Usuario = response,
                        Method = method,
                        Username = username,
                        Password = password
                    });
                    return true;
                }
                else
                {
                    if(method == AuthMethods.Email)
                    {
                        OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                        {
                            Success = false,
                            Message = response.ErrorMessage
                        });
                    }
                    else
                    {
                        if(string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(paterno))
                        {
                            OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                            {
                                Success = false,
                                Message = response.ErrorMessage
                            });
                        }
                        else  // FB/TWTTR/GOOGLE cuando envia el nombre / apellido, registra y reintenta el login
                        {
                            var registro = await MystiqueApiV2.Account.CallRegistrarV2(username, password, nombre, paterno, method);
                            if (registro.Success)
                            {
                                var retryLogin = await MystiqueApiV2.Account.CallLoginV2(method, username, password);
                                if (retryLogin.Success)
                                {
                                    SetUser(username, password, retryLogin, method);
                                    OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                                    {
                                        Success = true,
                                        Usuario = response,
                                        Method = method,
                                        Username = username,
                                        Password = password
                                    });
                                    return true;
                                }
                                else
                                {
                                    OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                                    {
                                        Success = false,
                                        Message = retryLogin.ErrorMessage
                                    });
                                }
                            }
                            else
                            {
                                OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                                {
                                    Success = false,
                                    Message = registro.ErrorMessage
                                });
                            }
                        }
                        
                        
                    }
                    
                }
            }
            else
            {
                ErrorMessage = config.ErrorMessage;
                OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                {
                    Success = false,
                    Message = config.ErrorMessage
                });
            }
            IsBusy = false;
            return false;
        }
        public async void Registrar(RegisterUser user)
        {
            const AuthMethods method = AuthMethods.Email;
            IsBusy = true;
            if (user.Metodo == null) throw new ArgumentNullException(nameof(user.Metodo));
            BaseModel response;
            try
            {
                
                if (!string.IsNullOrEmpty(user.Sexo))
                {
                    user.Sexo = SexosHelper.GenderToInt[user.Sexo].ToString();
                }
                else
                {
                    user.Sexo = "0";
                }

                if (!string.IsNullOrEmpty(user.Colonia) && Colonias.Contains(user.Colonia))
                    user.Colonia = _coloniasId[Colonias.IndexOf(user.Colonia)];
                else
                    user.Colonia = string.Empty;

            }
            catch
            {
                user.Colonia = string.Empty;
            }

            if (DateTime.TryParseExact(user.FechaNacimiento,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal,
                           result: out DateTime bd))
            {
                response = await MystiqueApiV2.Account.CallRegistrarV2(method, user.Email, user.Password, user.Nombre, user.Paterno, user.Materno, user.Colonia, user.Telefono, user.Sexo, fechaNacimiento: bd);
            }
            else
            {
                response = await MystiqueApiV2.Account.CallRegistrarV2(method, user.Email, user.Password, user.Nombre, user.Paterno, user.Materno, user.Colonia, user.Telefono, user.Sexo);

            }

            if (response.Success)
            {
                var login = await MystiqueApiV2.Account.CallLoginV2(method, user.Email, user.Password);
                if (login.Success)
                {
                    SetUser(user.Email, user.Password, login, method);
                    OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                    {
                        Success = true,
                        Method = AuthMethods.Email,
                        Username = user.Email,
                        Password = user.Password,
                        Usuario = login
                    });
                }
                else
                {
                    OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                    {
                        Success = false,
                        Message = login.ErrorMessage
                    });
                }
            }
            else
            {
                OnIniciarSesionFinished?.Invoke(this, new LoginEventArgs
                {
                    Success = false,
                    Message = response.ErrorMessage
                });
            }
            IsBusy = false;
        }
        public async void ActualizarPerfil(string nombre, string paterno, string materno, string fechaNacimiento, string sexo, string telefono, string colonia, string password)
        {
            if (IsBusy) return;
            IsBusy = true;

            Profile response;

            try
            {
                colonia = SetColoniaId(colonia);
                if (!string.IsNullOrEmpty(sexo))
                {
                    sexo = SexosHelper.GenderToInt[sexo].ToString();
                }
                else
                {
                    sexo = "0";
                }
            }
            catch
            {
                colonia = string.Empty;
            }

            if (DateTime.TryParseExact(fechaNacimiento,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal,
                           result: out DateTime bd))
            {
                response = await MystiqueApiV2.Account.CallActualizarPerfil(password, nombre, paterno, materno, colonia, telefono, sexo, bd);
            }
            else
            {
                response = await MystiqueApiV2.Account.CallActualizarPerfil(password, nombre, paterno, materno, colonia, telefono, sexo);
            }

            IsBusy = false;
            if (response.Success)
            {
                Usuario = response;
                if (!string.IsNullOrEmpty(password))
                    MystiqueApp.PASSWORD = password;

                OnActualizarPerfilFinished?.Invoke(this, new ActualizarPerfilEventArgs { Success = true, Usuario = Usuario });
            }
            else
            {
                OnActualizarPerfilFinished?.Invoke(this, new ActualizarPerfilEventArgs { Success = false, Message = response.ErrorMessage });
            }
        }
        public async void ActualizarPerfil(string nombre, string paterno, string materno, string fechaNacimiento, string sexo, string telefono, string colonia, string password, byte[] foto, string extensionFoto)
        {
            if (IsBusy) return;
            IsBusy = true;

            Profile response;

            try
            {
                colonia = SetColoniaId(colonia);
                if (!string.IsNullOrEmpty(sexo))
                {
                    sexo = SexosHelper.GenderToInt[sexo].ToString();
                }
                else
                {
                    sexo = "0";
                }
            }
            catch
            {
                colonia = string.Empty;
            }

            if (!string.IsNullOrEmpty(extensionFoto))
            {
                var profileResponse = await MystiqueApiV2.Files.CallPutProfilePicture(foto, extensionFoto);
                if (!profileResponse.Success)
                {
                    OnPictureUploadFailed?.Invoke(this, new PictureUploadEventArgs { Success = false, Message = profileResponse.ErrorMessage });
                }
            }

            if (DateTime.TryParseExact(fechaNacimiento,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.None,
                           result: out DateTime bd))
            {
                response = await MystiqueApiV2.Account.CallActualizarPerfil(password, nombre, paterno, materno, colonia, telefono, sexo, bd);
            }
            else
            {
                response = await MystiqueApiV2.Account.CallActualizarPerfil(password, nombre, paterno, materno, colonia, telefono, sexo);
            }

            
            if (response.Success)
            {
                Usuario = response;
                if (!string.IsNullOrEmpty(password))
                    MystiqueApp.PASSWORD = password;

                OnActualizarPerfilFinished?.Invoke(this, new ActualizarPerfilEventArgs { Success = true, Usuario = Usuario });
            }
            else
            {
                OnActualizarPerfilFinished?.Invoke(this, new ActualizarPerfilEventArgs { Success = false, Message = response.ErrorMessage});
            }
            IsBusy = false;
        }
        private string SetColoniaId(string colonia)
        {
            if (!string.IsNullOrEmpty(colonia))
            {
                if (string.IsNullOrEmpty(Usuario.Colonia))
                {
                    return Colonias.Contains(colonia) ? _coloniasId[Colonias.IndexOf(colonia)] : string.Empty;
                }
                else
                {
                    return colonia != Usuario.Colonia ? _coloniasId[Colonias.IndexOf(colonia)] : Usuario.ColoniaId;
                }
            }
            else
            {
                return colonia;
            }
        }
        public async void CerrarSesion()
        {

            IsBusy = true;
            var response = await MystiqueApiV2.Account.CallLogout();
            IsBusy = false;
            ErrorMessage = string.Empty;
            MystiqueApp.FbProfile = null;
            MystiqueApp.FbProfilePicture = string.Empty;
            MystiqueApp.FacebookId = string.Empty;
            MystiqueApp.USERNAME = string.Empty;
            MystiqueApp.PASSWORD = string.Empty;
            MystiqueApp.ID_CLIENTE = string.Empty;
            MystiqueApp.ID_MEMBRESIA = string.Empty;
        }
        public async void RecuperarContrasena(string username)
        {
            if (IsBusy) return;
            IsBusy = true;
            var config = await MystiqueApiV2.Configuracion.CallObtenerConfiguracion();

            if (config.Success)
            {
                MystiqueApp.Config = config.Configuracion;
                MystiqueApp.Comercios = config.Comercios;

                MystiqueApp.VersionAndroid = config.Configuracion.VersionAndroid;
                MystiqueApp.VersionIOS = config.Configuracion.VersionIOS;
                MystiqueApp.TerminosSoporte = config.Configuracion.TerminosSoporte;
                MystiqueApp.TelefonoSoporte = config.Configuracion.TelefonoSoporte;
                MystiqueApp.EmailSoporte = config.Configuracion.EmailSoporte;
                MystiqueApp.DescripcionSoporte = config.Configuracion.DescripcionSoporte;
                MystiqueApp.IdQDC = config.Configuracion.IdQDC;
                MystiqueApp.VersionIOSPruebas = config.Configuracion.VersionIOSPruebas;
                MystiqueApp.VersionAndroidPruebas = config.Configuracion.VersionAndroidPruebas;

                var isUserValid = await MystiqueApiV2.Account.CallValidarNoRegistroFacebook(username);
                if (isUserValid.Success)
                {
                    var response = await MystiqueApiV2.Account.CallRecuperarPassword(username);
                    if (response.Success)
                    {
                        OnRecuperarContrasenaFinished?.Invoke(this, new RecuperarPasswordEventArgs
                        {
                            Success = true,
                            Message = response.ErrorMessage
                        });
                    }
                    else
                    {
                        OnRecuperarContrasenaFinished?.Invoke(this, new RecuperarPasswordEventArgs
                        {
                            Success = false,
                            Message = response.ErrorMessage
                        });
                    }
                }
                else
                {
                    OnRecuperarContrasenaFinished?.Invoke(this, new RecuperarPasswordEventArgs
                    {
                        Success = false,
                        Message = isUserValid.ErrorMessage
                    });
                }
            }
            else
            {
                OnRecuperarContrasenaFinished?.Invoke(this, new RecuperarPasswordEventArgs
                {
                    Success = false,
                    Message = config.ErrorMessage
                });
            }
            IsBusy = false;
        }
        public async Task ConsultarColonias(string ciudad)
        {
            ciudad = !string.IsNullOrEmpty(ciudad) ? CiudadesHelper.CiudadesToInt[ciudad].ToString() : "0";
            IsBusy = true;
            if(ciudad != "0") {
                Colonias.Clear();
                _coloniasId.Clear();

                var response = await MystiqueApiV2.Account.ConsultarColonias(ciudad);
                if (response.Success)
                {
                    Colonias.AddRange(response.colonias.Select(c => c.Descripcion));
                    _coloniasId.AddRange(response.colonias.Select(c => c.Id));
                    OnConsultarColoniasFinished?.Invoke(this, new ConsultarColoniasEventArgs { Success = true, Colonias = Colonias });
                }
                else
                {
                    OnConsultarColoniasFinished?.Invoke(this, new ConsultarColoniasEventArgs { Success = false });
                }
            }
            else
            {
                Colonias.Clear();
                _coloniasId.Clear();
            }
            IsBusy = false;
            
        }
        #endregion
        #region Helpers
        private static void SetConfiguracion(ConfiguracionContainer config)
        {
            MystiqueApp.Config = config.Configuracion;
            MystiqueApp.Comercios = config.Comercios;
            MystiqueApp.VersionAndroid = config.Configuracion.VersionAndroid;
            MystiqueApp.VersionIOS = config.Configuracion.VersionIOS;
            MystiqueApp.TerminosSoporte = config.Configuracion.TerminosSoporte;
            MystiqueApp.TelefonoSoporte = config.Configuracion.TelefonoSoporte;
            MystiqueApp.EmailSoporte = config.Configuracion.EmailSoporte;
            MystiqueApp.DescripcionSoporte = config.Configuracion.DescripcionSoporte;
            MystiqueApp.IdQDC = config.Configuracion.IdQDC;
            MystiqueApp.VersionIOSPruebas = config.Configuracion.VersionIOSPruebas;
            MystiqueApp.VersionAndroidPruebas = config.Configuracion.VersionAndroidPruebas;

        }
        private void SetUser(string username, string password, Profile response, AuthMethods method)
        {
            Usuario = response;
            MystiqueApp.ID_CLIENTE = response.Id;
            MystiqueApp.ID_MEMBRESIA = response.IdMembresia;
            MystiqueApp.USERNAME = username;
            MystiqueApp.PASSWORD = password;
            MystiqueApp.Usuario = new User
            {
                AuthMethod = method,
                Id = response.Id,
                IdMembresia = response.IdMembresia,
                Password = response.Password,
                Username = response.Email
            };
            Usuario.Password = string.Empty;
        }
        #endregion
    }
    #region EventArgs

    #endregion
}
