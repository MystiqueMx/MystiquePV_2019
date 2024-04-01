using MystiqueNative.Configuration;
using MystiqueNative.Helpers;
using MystiqueNative.Models;
using MystiqueNative.Models.Configuration;
using MystiqueNative.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        #region FIELDS  
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public Profile Usuario { get => usuario; private set { usuario = value; OnPropertyChanged("Usuario"); } }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public List<string> Colonias { get => colonias; private set { colonias = value; OnPropertyChanged("Colonias"); } }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public bool ConfigStatus { get => configStatus; private set { configStatus = value; OnPropertyChanged("ConfigStatus"); } }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public bool LoggedStatus { get => loggedStatus; private set { loggedStatus = value; OnPropertyChanged("LoggedStatus"); } }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public bool ColoniasStatus { get => coloniasStatus; private set { coloniasStatus = value; OnPropertyChanged("ColoniasStatus"); } }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public bool ProfileEditStatus { get => profileEditStatus; private set { profileEditStatus = value; OnPropertyChanged("ProfileEditStatus"); } }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public bool RecoverPasswordStatus { get => recoverPasswordStatus; private set { recoverPasswordStatus = value; OnPropertyChanged("RecoverPasswordStatus"); } }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public bool RegisterStatus { get => profilePictureUploadStatus; private set { profilePictureUploadStatus = value; OnPropertyChanged("RegisterStatus"); } }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public string ProfilePictureUrl
        {
            get => Usuario == null || string.IsNullOrEmpty(Usuario.Foto)
                ? string.Empty 
                : string.Format("{0}?p={1}", MystiqueApiV2Config.DownloadPicturePath, Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Usuario.Foto))); 
        }
        #endregion
        #region API
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async void ConfigurarAplicacion()
        {
            if (IsBusy) return;
            IsBusy = true;
            var response = await MystiqueApiV2.Configuracion.CallObtenerConfiguracion();
            IsBusy = false;
            if (response != null && response.Success)
            {
                MystiqueApp.Config = response.Configuracion;
                MystiqueApp.Comercios = response.Comercios;

                MystiqueApp.VersionAndroid = response.Configuracion.VersionAndroid;
                MystiqueApp.VersionIOS = response.Configuracion.VersionIOS;
                MystiqueApp.TerminosSoporte = response.Configuracion.TerminosSoporte;
                MystiqueApp.TelefonoSoporte = response.Configuracion.TelefonoSoporte;
                MystiqueApp.EmailSoporte = response.Configuracion.EmailSoporte;
                MystiqueApp.DescripcionSoporte = response.Configuracion.DescripcionSoporte;
                MystiqueApp.IdQDC = response.Configuracion.IdQDC;
                MystiqueApp.VersionIOSPruebas = response.Configuracion.VersionIOSPruebas;
                MystiqueApp.VersionAndroidPruebas = response.Configuracion.VersionAndroidPruebas;
            }
            else
            {
                if(response != null)
                    ErrorMessage = response.ErrorMessage;
            }
            if (response != null)
                ConfigStatus = response.Success;
            else
                ConfigStatus = false;
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async void IniciarSesion(string Username, string Password)
        {
            if (IsBusy) return;
            IsBusy = true;

            while(string.IsNullOrEmpty(MystiqueApp.PlayerId))
                await Task.Delay(1000);

            var response = await MystiqueApiV2.Account.CallLogin(Username, Password);
            if (!ConfigStatus && !IsBusy)
            {
                var config = await MystiqueApiV2.Configuracion.CallObtenerConfiguracion();
                if (config != null && config.Success)
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
                else
                {
                    if (config != null)
                        ErrorMessage = config.ErrorMessage;
                }
                if (config != null)
                    ConfigStatus = config.Success;
                else
                    ConfigStatus = false;


            }
            
            if (response != null && response.Success)
            {
                Usuario = response;
                MystiqueApp.ID_CLIENTE = response.Id;
                MystiqueApp.ID_MEMBRESIA = response.IdMembresia;
                MystiqueApp.USERNAME = Username;
                MystiqueApp.PASSWORD = Password;
                Usuario.Password = string.Empty;
            }
            else
            {
                ErrorStatus = true;
                ErrorMessage = response.ErrorMessage;
            }
            LoggedStatus = response.Success;
            IsBusy = false;
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async Task<bool> IniciarSesionAsBool(string Username, string Password)
        {
            IsBusy = true;
            var config = await MystiqueApiV2.Configuracion.CallObtenerConfiguracion();
            var response = await MystiqueApiV2.Account.CallLogin(Username, Password);

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
            }
            else
            {
                    ErrorMessage = config.ErrorMessage;
            }
             
            if (response.Success)
            {
                Usuario = response;
                MystiqueApp.ID_CLIENTE = response.Id;
                MystiqueApp.ID_MEMBRESIA = response.IdMembresia;
                MystiqueApp.USERNAME = Username;
                MystiqueApp.PASSWORD = Password;
                Usuario.Password = string.Empty;
            }
            else
            {
                ErrorStatus = true;
                ErrorMessage = response.ErrorMessage;
            }
            ConfigStatus = config.Success;
            LoggedStatus = response.Success;
            IsBusy = false;
            return response.Success;
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async Task<bool> IniciarSesionConFacebook(string Username, string Fbid)
        {
            IsBusy = true;
               
            var config = await MystiqueApiV2.Configuracion.CallObtenerConfiguracion();
            var response = await MystiqueApiV2.Account.CallFbLogin(Username, Fbid);

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
            }
            else
            {
                ErrorMessage = config.ErrorMessage;
            }

            if (response.Success)
            {
                Usuario = response;
                MystiqueApp.ID_CLIENTE = response.Id;
                MystiqueApp.ID_MEMBRESIA = response.IdMembresia;
                MystiqueApp.USERNAME = Username;
                MystiqueApp.PASSWORD = response.Password;
                Usuario.Password = string.Empty;
            }
            else
            {
                ErrorStatus = true;
                ErrorMessage = response.ErrorMessage;
            }
            ConfigStatus = config.Success;
            LoggedStatus = response.Success;
            IsBusy = false;
            return response.Success;
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async void Registrar(string Nombre, string Paterno, string Materno, string FechaNacimiento, string Sexo, string Telefono, string Colonia, string Email, string Password)
        {
            if (IsBusy) return;
            IsBusy = true;

            BaseModel response;
            if (!string.IsNullOrEmpty(Sexo))
            {
                Sexo = SexosHelper.GenderToInt[Sexo].ToString();
            }
            else
            {
                Sexo = "0";
            }

            if (DateTime.TryParseExact(FechaNacimiento,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal,
                           result: out DateTime bd))
            {
                try
                {
                    if (!string.IsNullOrEmpty(Colonia) && Colonias.Contains(Colonia))
                        Colonia = coloniasId[colonias.IndexOf(Colonia)];
                    else
                        Colonia = string.Empty;
                }
                catch
                {
                    Colonia = string.Empty;
                }
                response = await MystiqueApiV2.Account.CallRegistrar(Email, Password, Nombre, Paterno, Materno, Colonia, Telefono, Sexo , bd);
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(Colonia) && Colonias.Contains(Colonia))
                        Colonia = coloniasId[colonias.IndexOf(Colonia)];
                    else
                        Colonia = string.Empty;
                }
                catch
                {
                    Colonia = string.Empty;
                }
                response = await MystiqueApiV2.Account.CallRegistrar(Email, Password, Nombre, Paterno, Materno, Colonia, Telefono, Sexo);
            }
  
            IsBusy = false;
            if (response.Success)
            {
                var login = await MystiqueApiV2.Account.CallLogin(Email, Password);
                if (login.Success)
                {
                    Usuario = login;
                    MystiqueApp.ID_CLIENTE = login.Id;
                    MystiqueApp.ID_MEMBRESIA = login.IdMembresia;
                    MystiqueApp.USERNAME = Email;
                    MystiqueApp.PASSWORD = Password;
                    Usuario.Password = string.Empty;
                }
                else
                {
                    ErrorStatus = true;
                    ErrorMessage = response.ErrorMessage;
                }
                LoggedStatus = login.Success;
            }
            else
            {
                ErrorStatus = true;
                ErrorMessage = response.ErrorMessage;
                LoggedStatus = false;
            }
            
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async void Registrar(RegisterUser user)
        {
            IsBusy = true;

            BaseModel response;
            if (!string.IsNullOrEmpty(user.Sexo))
            {
                user.Sexo = SexosHelper.GenderToInt[user.Sexo].ToString();
            }
            else
            {
                user.Sexo = "0";
            }

            if (DateTime.TryParseExact(user.FechaNacimiento,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal,
                           result: out DateTime bd))
            {
                try
                {
                    if (!string.IsNullOrEmpty(user.Colonia) && Colonias.Contains(user.Colonia))
                        user.Colonia = coloniasId[colonias.IndexOf(user.Colonia)];
                    else
                        user.Colonia = string.Empty;
                }
                catch
                {
                    user.Colonia = string.Empty;
                }
                
                response = await MystiqueApiV2.Account.CallRegistrar(user.Email, user.Password, user.Nombre, user.Paterno, user.Materno, user.Colonia, user.Telefono, user.Sexo, fechaNacimiento: bd, facebookId: user.FbId);
            }
            else
            {

                try
                {
                    if (!string.IsNullOrEmpty(user.Colonia) && Colonias.Contains(user.Colonia))
                        user.Colonia = coloniasId[colonias.IndexOf(user.Colonia)];
                    else
                        user.Colonia = string.Empty;
                }
                catch
                {
                    user.Colonia = string.Empty;
                }
                response = await MystiqueApiV2.Account.CallRegistrar(user.Email, user.Password, user.Nombre, user.Paterno, user.Materno, user.Colonia, user.Telefono, user.Sexo, facebookId: user.FbId);

            }

            RegisterStatus = response.Success;
            IsBusy = false;
            if (response.Success)
            {
                var login = await MystiqueApiV2.Account.CallLogin(user.Email, user.Password);
                if (login.Success)
                {
                    Usuario = login;
                    MystiqueApp.ID_CLIENTE = login.Id;
                    MystiqueApp.ID_MEMBRESIA = login.IdMembresia;
                    MystiqueApp.USERNAME = user.Email;
                    MystiqueApp.PASSWORD = user.Password;
                    Usuario.Password = string.Empty;
                }
                else
                {
                    ErrorStatus = true;
                    ErrorMessage = response.ErrorMessage;
                }
                LoggedStatus = login.Success;
            }
            else
            {
                ErrorStatus = true;
                ErrorMessage = response.ErrorMessage;
                LoggedStatus = false;
            }
            
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async void ActualizarPerfil(string Nombre, string Paterno, string Materno, string FechaNacimiento, string Sexo, string Telefono, string Colonia, string Password)
        {
            if (IsBusy) return;
            IsBusy = true;

            Profile response;

            if (!string.IsNullOrEmpty(Sexo))
            {
                Sexo = SexosHelper.GenderToInt[Sexo].ToString();
            }
            else
            {
                Sexo = "0";
            }

            if (DateTime.TryParseExact(FechaNacimiento,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.AssumeLocal,
                           result: out DateTime bd))
            {
                
                try
                {
                    Colonia = SetColoniaId(Colonia);
                    
                }
                catch
                {
                    Colonia = string.Empty;
                }
                response = await MystiqueApiV2.Account.CallActualizarPerfil(Password, Nombre, Paterno, Materno, Colonia, Telefono, Sexo, bd);
            }
            else
            {

                try
                {
                    Colonia = SetColoniaId(Colonia);
                }
                catch
                {
                    Colonia = string.Empty;
                }
                response = await MystiqueApiV2.Account.CallActualizarPerfil(Password, Nombre, Paterno, Materno, Colonia, Telefono, Sexo);

            }

            IsBusy = false;
            if (response.Success)
            {
                Usuario = response;
                if(!string.IsNullOrEmpty(Password))
                    MystiqueApp.PASSWORD = Password;
            }
            else
            {
                ErrorStatus = true;
                ErrorMessage = response.ErrorMessage;
            }
            ProfileEditStatus = response.Success;
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        private string SetColoniaId(string colonia)
        {
            if (!string.IsNullOrEmpty(colonia))
            {
                if (string.IsNullOrEmpty(Usuario.Colonia))
                {
                    if (Colonias.Contains(colonia))
                        return coloniasId[colonias.IndexOf(colonia)];
                    else
                        return string.Empty;
                }
                else
                {
                    if (colonia != Usuario.Colonia)
                        return coloniasId[colonias.IndexOf(colonia)];
                    else
                        return Usuario.ColoniaId;
                }
            }
            else
            {
                return colonia;
            }
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async void ActualizarPerfil(string Nombre, string Paterno, string Materno, string FechaNacimiento, string Sexo, string Telefono, string Colonia, string Password, byte[] Foto, string ExtensionFoto)
        {
            if (IsBusy) return;
            IsBusy = true;

            Profile response;

            if (!string.IsNullOrEmpty(Sexo))
            {
                Sexo = SexosHelper.GenderToInt[Sexo].ToString();
            }

            if (!string.IsNullOrEmpty(ExtensionFoto))
            {
                var profileResponse = await MystiqueApiV2.Files.CallPutProfilePicture(Foto, ExtensionFoto);
                if (!profileResponse.Success)
                {
                    ErrorStatus = true;
                    ErrorMessage = profileResponse.ErrorMessage;
                }
                profilePictureUploadStatus = profileResponse.Success;
            }

            if (DateTime.TryParseExact(FechaNacimiento,
                           format: "dd/MM/yyyy",
                           provider: CultureInfo.InvariantCulture,
                           style: DateTimeStyles.None,
                           result: out DateTime bd))
            {

                try
                {
                    Colonia = SetColoniaId(Colonia);
                }
                catch
                {
                    Colonia = string.Empty;
                }
                response = await MystiqueApiV2.Account.CallActualizarPerfil(Password, Nombre, Paterno, Materno, Colonia, Telefono, Sexo, bd);
            }
            else
            {

                try
                {
                    Colonia = SetColoniaId(Colonia);
                }
                catch
                {
                    Colonia = string.Empty;
                }
                response = await MystiqueApiV2.Account.CallActualizarPerfil(Password, Nombre, Paterno, Materno, Colonia, Telefono, Sexo);
            }

            IsBusy = false;
            if (response.Success)
            {
                Usuario = response;
                if (!string.IsNullOrEmpty(Password))
                    MystiqueApp.PASSWORD = Password;
            }
            else
            {
                ErrorStatus = true;
                ErrorMessage = response.ErrorMessage;
            }
            ProfileEditStatus = response.Success;
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async void CerrarSesion()
        {

            IsBusy = true;
            var response = await MystiqueApiV2.Account.CallLogout();
            IsBusy = false;
            LoggedStatus = false;
            ErrorMessage = string.Empty;
            MystiqueApp.FbProfile = null;
            MystiqueApp.FbProfilePicture = string.Empty;
            MystiqueApp.FacebookId = string.Empty;
            MystiqueApp.USERNAME = string.Empty;
            MystiqueApp.PASSWORD = string.Empty;
            MystiqueApp.ID_CLIENTE = string.Empty;
            MystiqueApp.ID_MEMBRESIA = string.Empty;
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async void RecuperarContrasena(string Username)
        {
            if (IsBusy) return;
            IsBusy = true;
            var IsUserValid = await MystiqueApiV2.Account.CallValidarNoRegistroFacebook(Username);
            if (IsUserValid.Success)
            {
                var response = await MystiqueApiV2.Account.CallRecuperarPassword(Username);
               
                if (response != null && response.Success)
                {
                    ErrorMessage = response.ErrorMessage;
                }
                else
                {
                    ErrorStatus = true;
                    ErrorMessage = response.ErrorMessage;
                }
                RecoverPasswordStatus = response.Success;
            }
            else
            {
                ErrorStatus = true;
                ErrorMessage = IsUserValid.ErrorMessage;
                RecoverPasswordStatus = IsUserValid.Success;
            }
            IsBusy = false;
        }
        [Obsolete("Auth V1 depreciado, usar Auth V2", true)]
        public async Task ConsultarColonias(string Ciudad)
        {
            if (IsBusy) await Task.Delay(500);
            if (!string.IsNullOrEmpty(Ciudad))
            {
                Ciudad = CiudadesHelper.CiudadesToInt[Ciudad].ToString();
            }
            else
            {
                Ciudad = "0";
            }
            IsBusy = true;
            if(Ciudad != "0") {
                Colonias.Clear();
                coloniasId.Clear();

                var response = await MystiqueApiV2.Account.ConsultarColonias(Ciudad);
                if (response.Success)
                {
                    foreach(var c in response.colonias)
                    {
                        Colonias.Add(c.Descripcion);
                        coloniasId.Add(c.Id);
                    }
                }
                ColoniasStatus = response.Success;
            }
            else
            {
                Colonias.Clear();
                coloniasId.Clear();
                ColoniasStatus = true;
            }
           // await Task.Delay(8000);
            IsBusy = false;
            
        }
        #endregion
        private Profile usuario;
        private List<string> colonias = new List<string>();
        private List<string> coloniasId = new List<string>();
        private bool configStatus;
        private bool loggedStatus;
        private bool coloniasStatus;
        private bool profileEditStatus;
        private bool recoverPasswordStatus;
        private bool profilePictureUploadStatus;
    }
}
