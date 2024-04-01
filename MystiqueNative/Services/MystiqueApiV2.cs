using MystiqueNative.Configuration;
using MystiqueNative.Helpers;
using MystiqueNative.Models;
using MystiqueNative.Models.Clientes;
using MystiqueNative.Models.Ensaladas;
using MystiqueNative.Models.Location;
using MystiqueNative.Models.Menu;
using MystiqueNative.Models.OpenPay;
using MystiqueNative.Models.Orden;
using MystiqueNative.Models.Platillos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.Services
{
    internal static class MystiqueApiV2
    {
        internal static class Beneficios
        {
            internal static async Task<SucursalContainer> CallObtenerSucursalesPorComercio(string idComercio)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"idComercio", idComercio},
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"clienteId", MystiqueApp.ID_CLIENTE },
                        {"membresiaId", MystiqueApp.ID_MEMBRESIA },
                    };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerSucursalesIdcomercioPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<SucursalContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerListaSucursalPorComercio | Error al contactar el servidor : " + e.Message);
#endif
                    return new SucursalContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }


            }
            internal static async Task<BeneficiosSContainer> CallObtenerBeneficiosPorSucursal(string idSucursal)
            {
                var parameters = new Dictionary<string, string>()
                {
                    {"sucursalId", idSucursal},
                    {"correoElectronico", MystiqueApp.USERNAME },
                    {"contrasenia",  MystiqueApp.PASSWORD },
                    {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                    {"clienteId", MystiqueApp.ID_CLIENTE },
                    {"membresiaId", MystiqueApp.ID_MEMBRESIA },
                };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<BeneficiosSContainer>(
                                await RestApiClient.Post(MystiqueApiV2Config.ObtenerBeneficiosIdsucursalPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerBeneficioPorIDSucursalAsync | Error al contactar el servidor : " + e.Message);
#endif
                    return new BeneficiosSContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<DetalleBeneficioContainer> CallObtenerDetalleBeneficio(string idBeneficio, string idSucursal)
            {
                var parameters = new Dictionary<string, string>()
                {
                    {"sucursalId", idSucursal },
                    {"beneficioId", idBeneficio },
                    {"clienteId", MystiqueApp.ID_CLIENTE },
                    {"correoElectronico", MystiqueApp.USERNAME },
                    {"contrasenia",  MystiqueApp.PASSWORD },
                    {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<DetalleBeneficioContainer>(
                                await RestApiClient.Post(MystiqueApiV2Config.ObtenerDetalleBeneficioPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerBeneficioPorIDSucursalAsync | Error al contactar el servidor : " + e.Message);
#endif
                    return new DetalleBeneficioContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<ComercioContainer> CallObtenerComerciosIdGiro(string idGiro)
            {
                var parameters = new Dictionary<string, string>()
                {
                    {"giroId", idGiro },
                    {"correoElectronico", MystiqueApp.USERNAME },
                    {"contrasenia",  MystiqueApp.PASSWORD },
                    {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<ComercioContainer>(
                                await RestApiClient.Post(MystiqueApiV2Config.ObtenerComerciosIdgiroPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerBeneficioPorIDSucursalAsync | Error al contactar el servidor : " + e.Message);
#endif
                    return new ComercioContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallInsertarCalificacion(string idBeneficio, string calificacion)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"beneficioId", idBeneficio },
                        {"clienteId", MystiqueApp.ID_CLIENTE  },
                        {"calificacion", calificacion },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia", MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.InsertarCalificacionPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallAgregarBeneficio | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
        }
        internal static class Account
        {
            internal static async Task<Profile> CallLogin(string email, string password)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"correoElectronico", email},
                        {"contrasenia", password },
                        {"PlayerId", MystiqueApp.PlayerId },
                        {"deviceId",MystiqueApp.DeviceId },
                        {"devicePlatform",MystiqueApp.DevicePlatform },
                        {"deviceModel",MystiqueApp.DeviceModel },
                        {"deviceVersion",MystiqueApp.DeviceVersion },
                        {"deviceConnectionType", MystiqueApp.DeviceConnectionType },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<Profile>(
                                    await RestApiClient.Post(MystiqueApiV2Config.LoginPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
                    #if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
                    #endif
                    return new Profile { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<Profile> CallFbLogin(string email, string fbid)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"correoElectronico", email},
                        {"facebookId", fbid },
                        {"PlayerId", MystiqueApp.PlayerId },
                        {"deviceId",MystiqueApp.DeviceId },
                        {"devicePlatform",MystiqueApp.DevicePlatform },
                        {"deviceModel",MystiqueApp.DeviceModel },
                        {"deviceVersion",MystiqueApp.DeviceVersion },
                        {"deviceConnectionType", MystiqueApp.DeviceConnectionType },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<Profile>(
                                    await RestApiClient.Post(MystiqueApiV2Config.FbLoginPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new Profile { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallLogout()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"playerId", MystiqueApp.PlayerId },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.LogoutPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
                    #if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallValidarNoRegistroFacebook(string email)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"correoElectronico", email },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.ValidateRecoverPasswordPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallRecuperarPassword(string email)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"Email", email },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.RecoverPasswordPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallRegistrar(string email, string password, string nombre, string paterno, string materno, string colonia, string telefono, string sexo, DateTime? fechaNacimiento = null, string facebookId = "")
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"email", email},
                        {"userName", email},
                        {"password", password },
                        {"confirmPassword", password },
                        {"facebookId", facebookId },
                        {"nombre", nombre },
                        {"paterno", paterno },
                        {"materno", materno },
                        {"telefono", telefono },
                        {"sexo", sexo },
                        {"coloniaId", string.IsNullOrEmpty(colonia)? "0" : colonia },
                        {"fechaNacimiento", fechaNacimiento.HasValue ? JsonConvert.SerializeObject(fechaNacimiento.Value).Replace("\"", "") : "" },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.RegisterPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
                    #if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<Profile> CallActualizarPerfil(string newPassword, string nombre, string paterno, string materno, string colonia, string telefono, string sexo, DateTime? fechaNacimiento = null)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"correoElectronico", MystiqueApp.USERNAME},
                        {"contrasenia", MystiqueApp.PASSWORD},
                        {"clienteId", MystiqueApp.ID_CLIENTE},
                        {"nombre", nombre},
                        {"paterno", paterno},
                        {"materno", materno},
                        {"telefono", telefono},
                        {"coloniaId", string.IsNullOrEmpty(colonia)? "0" : colonia },
                        {"catSexoId", sexo},
                        {"contraseniaNueva", string.IsNullOrEmpty(newPassword) ? MystiqueApp.PASSWORD : newPassword },
                        {"fechaNacimiento", fechaNacimiento.HasValue ? JsonConvert.SerializeObject(fechaNacimiento.Value.Date).Replace("\"","") : "" },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<Profile>(
                                    await RestApiClient.Post(MystiqueApiV2Config.UpdateProfilePath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new Profile { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }

            internal static async Task<ColoniaContainer> ConsultarColonias(string ciudad)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"ciudadId", ciudad},
                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<ColoniaContainer>(
                                        await RestApiClient.Post(MystiqueApiV2Config.ObtenerColoniasCiudadidPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>ConsultarColonias | Error al contactar el servidor : " + e.Message);
#endif
                    return new ColoniaContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }

            internal static async Task<Profile> CallLoginV2(AuthMethods method, string username, string password)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"correoElectronico", username},
                        {"contrasenia", password },
                        {"tipoAutentificacionId", ((int)method).ToString() },
                        {"PlayerId", MystiqueApp.PlayerId },
                        {"deviceId",MystiqueApp.DeviceId },
                        {"devicePlatform",MystiqueApp.DevicePlatform },
                        {"deviceModel",MystiqueApp.DeviceModel },
                        {"deviceVersion",MystiqueApp.DeviceVersion },
                        {"deviceConnectionType", MystiqueApp.DeviceConnectionType },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<Profile>(
                                    await RestApiClient.Post(MystiqueApiV2Config.IniciarSesionPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLoginV2 | Error al contactar el servidor : " + e.Message);
#endif
                    return new Profile { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }

            internal static async Task<BaseModel> CallRegistrarV2(string username, string password, string nombre, string paterno, AuthMethods method)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"email", username},
                        {"userName", username},
                        {"password", password },
                        {"confirmPassword", password },
                        {"tipoAutentificacionId", ((int)method).ToString() },
                        {"nombre", nombre },
                        {"paterno", paterno },
                        {"sexo", "0" },
                        {"coloniaId", "0" },
                        {"fechaNacimiento", "" },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.RegisterPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallRegistrarV2 | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallRegistrarV2(AuthMethods method, string email, string password, string nombre, string paterno, string materno, string colonia, string telefono, string sexo, DateTime? fechaNacimiento = null, string facebookId = "")
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"email", email},
                        {"userName", email},
                        {"password", password },
                        {"confirmPassword", password },
                        {"tipoAutentificacionId", ((int)method).ToString() },
                        {"facebookId", facebookId },
                        {"nombre", nombre },
                        {"paterno", paterno },
                        {"materno", materno },
                        {"telefono", telefono },
                        {"sexo", sexo },
                        {"coloniaId", string.IsNullOrEmpty(colonia)? "0" : colonia },
                        {"fechaNacimiento", fechaNacimiento.HasValue ? JsonConvert.SerializeObject(fechaNacimiento.Value).Replace("\"", "") : "" },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.RegisterPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
        }
        internal static class Wallet
        {
            internal static async Task<BaseModel> CallAgregarBeneficio(string idSucursal, string idBeneficio)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"sucursalId", idSucursal },
                        {"beneficioId", idBeneficio },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"clienteId", MystiqueApp.ID_CLIENTE },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                    };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.AgregarBeneficioWalletPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallAgregarBeneficio | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };

                }
            }
            internal static async Task<BaseModel> CallRemoverBeneficio(string idBeneficio)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"beneficioId", idBeneficio },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"clienteId", MystiqueApp.ID_CLIENTE },
                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.RemoverBeneficioWalletPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallAgregarBeneficio | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<WalletContainer> CallObtenerWallet()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"clienteId", MystiqueApp.ID_CLIENTE },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                    };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<WalletContainer>(
                                    await RestApiClient.Post(MystiqueApiV2Config.ObtenerWalletPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallAgregarBeneficio | Error al contactar el servidor : " + e.Message);
#endif
                    return new WalletContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            //internal static async Task<BaseModel> CallSetZonaCitySalads()
            //{
            //    throw new NotImplementedException();
            //}
            //internal static async Task<BaseModel> PingAvisoZonaCitySalads(decimal latitud, decimal longitud)
            //{
            //    throw new NotImplementedException();
            //}
        }
        internal static class Configuracion
        {
            
            internal static async Task<BaseModel> CallEnviarComentario(string mensaje, string tipoComentario)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"clienteId", MystiqueApp.ID_CLIENTE },
                        {"Mensaje", mensaje },
                        {"tipoComentarioId", tipoComentario },
                        {"fromComercio", false.ToString() },
                        {"fromCliente", true.ToString() },
                        {"FechaRegistro", JsonConvert.SerializeObject(DateTime.Now) },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    };
                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.EnviarComentarioPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallAgregarBeneficio | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<ConfiguracionContainer> CallObtenerConfiguracion()
            {
                var parameters = new Dictionary<string, string>()
                {
                    {"idEmpresa", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                };
                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerConfiguracionPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<ConfiguracionContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>ConfiguracionContainer | Error al contactar el servidor : " + e.Message);
#endif
                    return new ConfiguracionContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
        }
        internal static class CityPoints
        {
            internal static async Task<BaseModel> CallRegistarPuntos(string codigo)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"membresiaId", MystiqueApp.ID_MEMBRESIA },
                        {"codigoGenerado", codigo },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia", MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                    };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.RegistrarCitypointsPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerListaSucursalPorComercio | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<EstadoCuenta> CallObtenerPuntos()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"membresiaId", MystiqueApp.ID_MEMBRESIA },
                        {"codigoGenerado", string.Empty },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia", MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                    };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerCitypointsPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<EstadoCuenta>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerListaSucursalPorComercio | Error al contactar el servidor : " + e.Message);
#endif
                    return new EstadoCuenta { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<RecompensaCanjeada> CallCanjearPuntos(string idRecompensa)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"membresiaId", MystiqueApp.ID_MEMBRESIA },
                        {"recompensaId", idRecompensa },
                        {"playerId", MystiqueApp.PlayerId },
                        {"deviceId", MystiqueApp.DeviceId },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia", MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                    };
                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.CanjearCitypointsPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<RecompensaCanjeada>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerListaSucursalPorComercio | Error al contactar el servidor : " + e.Message);
#endif
                    return new RecompensaCanjeada { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<RecompensaContainer> CallObtenerRecompensas()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"membresiaId", MystiqueApp.ID_MEMBRESIA },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia", MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                    };
                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerRecompensasPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<RecompensaContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerListaSucursalPorComercio | Error al contactar el servidor : " + e.Message);
#endif
                    return new RecompensaContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<MovimientoCitypointsContainer> CallObtenerMovimientos()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"membresiaId", MystiqueApp.ID_MEMBRESIA },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia", MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                    };
                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerMovimientosPuntosPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<MovimientoCitypointsContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerListaSucursalPorComercio | Error al contactar el servidor : " + e.Message);
#endif
                    return new MovimientoCitypointsContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallValidarRegistroCompleto()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia", MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"comercioId", MystiqueApp.Comercios.FirstOrDefault()?.Id  },
                    };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.ValidateCapturaCitypointsPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<RecompensasCanjeadasContainer> CallObtenerRecompensasActivas()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia", MystiqueApp.PASSWORD },
                        {"membresiaId", MystiqueApp.ID_MEMBRESIA },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"comercioId", MystiqueApp.Comercios.FirstOrDefault().Id  },
                    };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<RecompensasCanjeadasContainer>(
                                    await RestApiClient.Post(MystiqueApiV2Config.ObtenerCuponesActivosPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new RecompensasCanjeadasContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallEliminarRecompensa(string canjepuntoId)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia", MystiqueApp.PASSWORD },
                        {"canjepuntoId", canjepuntoId },
                        {"membresiaId", MystiqueApp.ID_MEMBRESIA },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                        {"comercioId", MystiqueApp.Comercios.FirstOrDefault().Id  },
                    };

                try
                {
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(
                                    await RestApiClient.Post(MystiqueApiV2Config.EliminarRecompensaPath, parameters));

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLogin | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
        }
        internal static class Files
        {
            #region FILES
            public static async Task<BaseModel> CallPutProfilePicture(byte[] file, string extension)
            {
                var parameters = new Dictionary<string, string>()
                {
                    { "correoElectronico", MystiqueApp.USERNAME },
                    { "contrasenia", MystiqueApp.PASSWORD },
                    { "empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },
                    { "comercioId", MystiqueApp.Comercios.FirstOrDefault().Id  },
                };

                try
                {
                    var response = JsonConvert
                        .DeserializeObject<BaseModel>(await RestApiClient.PutFile(file, extension, MystiqueApiV2Config.UploadPicturePath, parameters));
                    if (response == null)
                        throw new ArgumentNullException("Null response");
                    return response;
                }
                catch (Exception ex)
                {
                #if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLoginAsync | Error al contactar el servidor : " + ex.Message);
                #endif
                    return new BaseModel() { Success = false, ErrorMessage = "Error al contactar al servidor" };
                }
            }
            #endregion
        }

        internal static class Notificaciones
        {
            internal static async Task<NotificacionesContainer> CallObtenerNotificacionesIdCliente()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"idCliente", MystiqueApp.ID_CLIENTE },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                    };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerNotificacionesPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<NotificacionesContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerListaSucursalPorComercio | Error al contactar el servidor : " + e.Message);
#endif
                    return new NotificacionesContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallLimpiarNotificacionesCliente()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"idCliente", MystiqueApp.ID_CLIENTE },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                    };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.LimpiarNotificacionesPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallLimpiarNotificacionesCliente | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
        }
        internal static class Facturacion
        {
            internal static async Task<FacturasContainer> CallObtenerFacturas()
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"clienteId", MystiqueApp.ID_CLIENTE },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                    };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerFacturasPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<FacturasContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallObtenerFacturas | Error al contactar el servidor : " + e.Message);
#endif
                    return new FacturasContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }

            internal static async Task<TicketContainer> CallValidarTicket(string ticket)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"codigoGenerado", ticket },

                        {"clienteId", MystiqueApp.ID_CLIENTE },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                    };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ValidarTicketPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<TicketContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallValidarTicket | Error al contactar el servidor : " + e.Message);
#endif
                    return new TicketContainer { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallSolicitarFactura(string ticket, bool pendiente, string sucursalId, ReceptorFactura receptor)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"folioTicket", ticket },
                        {"claveUsoCFDI", receptor.UsoCFDI },
                        {"correo", receptor.Email },
                        {"rfcReceptor", receptor.Rfc},
                        {"companiaNombreLegal", receptor.RazonSocial },
                        {"codigoPostal", receptor.CodigoPostal },
                        {"direccion", receptor.Direccion },
                        {"PendienteTicket", pendiente.ToString() },
                        {"SucursalId", sucursalId },

                        {"clienteId", MystiqueApp.ID_CLIENTE },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                    };
                if (!string.IsNullOrEmpty(receptor.Id))
                {
                    parameters.Add("receptorClienteId", receptor.Id);
                }

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.SolicitarFacturaPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallSolicitarFactura | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallRemoverDatosFiscales(string receptorClienteId)
            {
                var parameters = new Dictionary<string, string>()
                    {
                        {"receptorClienteId", receptorClienteId },

                        {"clienteId", MystiqueApp.ID_CLIENTE },
                        {"correoElectronico", MystiqueApp.USERNAME },
                        {"contrasenia",  MystiqueApp.PASSWORD },
                        {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                    };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.RemoverDatosFiscalesPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallSolicitarFactura | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
            internal static async Task<BaseModel> CallReenviarFactura(string factura, string email)
            {
                var parameters = new Dictionary<string, string>()
                {
                    {"factura", factura },
                    {"email", email },

                    {"clienteId", MystiqueApp.ID_CLIENTE },
                    {"correoElectronico", MystiqueApp.USERNAME },
                    {"contrasenia",  MystiqueApp.PASSWORD },
                    {"empresaId", MystiqueApiV2Config.MystiqueAppEmpresa.ToString() },

                };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ReenviarFacturaPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<BaseModel>(resultadostring);

                    if (resultado == null)
                    {
                        throw new ArgumentNullException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ MystiqueApi>CallSolicitarFactura | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseModel { Success = false, ErrorMessage = "Ocurrio un error al contactar el servidor" };
                }
            }
        }

        internal static class Clientes
        {
            public static async Task<ClienteContainer> LlamarBuscarClienteCallCenter(string telefono)
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"telefono", telefono}
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.BuscarClienteCallCenterPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<ClienteContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");

                    }
                    return resultado;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
#if DEBUG
                    Console.WriteLine("~ MystiqueApiV2>LlamarBuscarClienteCallCenter | Error al contactar el servidor : " + e.Message);
#endif
                    return new ClienteContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Ocurrio un error al contactar el servidor." } };
                }
            }

            public static async Task<ClienteContainer> LlamarBuscarListaClienteCallCenter(string telefono, string nombre)
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"telefono", telefono},
                        {"nombre", nombre}
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.BuscarListaClienteCallCenterPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<ClienteContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");

                    }
                    return resultado;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
#if DEBUG
                    Console.WriteLine("~ MystiqueApiV2>LlamarBuscarListaClienteCallCenter | Error al contactar el servidor : " + e.Message);
#endif
                    return new ClienteContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Ocurrio un error al contactar el servidor." } };
                }
            }

            public static async Task<ClienteContainer> LlamarObtenerClienteCallCenter()
            {
                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerClienteCallCenterPath);
                    var resultado = JsonConvert.DeserializeObject<ClienteContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");

                    }
                    return resultado;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
#if DEBUG
                    Console.WriteLine("~ MystiqueApiV2>LlamarBuscarClienteCallCenter | Error al contactar el servidor : " + e.Message);
#endif
                    return new ClienteContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Ocurrio un error al contactar el servidor." } };
                }
            }

            public static async Task<ClienteContainer> LlamarGuardarClienteCallCenter(string nombre, string apPaterno, string apMaterno, string telefono)
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"nombre", nombre},
                        {"apPaterno", apPaterno},
                        {"apMaterno", apMaterno},
                        {"telefono", telefono},
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.GuardarClienteCallCenterPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<ClienteContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");

                    }
                    return resultado;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
#if DEBUG
                    Console.WriteLine("~ MystiqueApiV2>LlamarGuardarClienteCallCenter | Error al contactar el servidor : " + e.Message);
#endif
                    return new ClienteContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Ocurrio un error al contactar el servidor." } };
                }
            }
        }
    }

    internal static class QdcApi
    {
        internal static class Restaurantes
        {
            public static async Task<RestaurantesContainer> LlamarObtenerRestaurantes(FiltroRestaurante filtro)
            {
                try
                {
                    if (MystiqueApp.UltimaUbicacionConocida == null)
                    {
#if DEBUG
                        Console.WriteLine("~ QdcApi>LlamarObtenerRestaurantes | Error al obtener la ultima ubicación : QdcApplication.Instance.UltimaUbicacionConocida == null");
#endif
                        return new RestaurantesContainer
                        {
                            Resultados = new ResultadoRestaurantes
                            {
                                Restaurantes = new List<Restaurante>()
                            },
                            Estatus = new BaseModel
                            {
                                ResponseCode = ResponseTypes.CodigoInterno,
                                Message = "No se pudo obtener la ubicación actual, intenta activar la ubicación o selecciona una en el mapa"
                            }
                        };
                    }
                    var parameters = new Dictionary<string, string>()
                    {
                        {"Latitud", MystiqueApp.UltimaUbicacionConocida.Latitude.ToString("N10") },
                        {"Longitud", MystiqueApp.UltimaUbicacionConocida.Longitude.ToString("N10") },
                        {"RestauranteTiposReparto", $"{(int)filtro}" },
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerRestaurantesPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<RestaurantesContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerRestaurantes | Error al contactar el servidor : " + e.Message);
#endif
                    return new RestaurantesContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }

            public static async Task<MenuRestauranteContainer> LlamarObtenerMenuRestaurante(string idRestaurante)
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"sucursalId", idRestaurante},
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerMenuRestaurantePath, parameters);
                    var resultado = JsonConvert.DeserializeObject<MenuRestauranteContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");

                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerMenuRestaurante | Error al contactar el servidor : " + e.Message);
#endif
                    return new MenuRestauranteContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }

            public static async Task<EnsaladasContainer> LlamarObtenerConfiguracionEnsaladas()
            {
                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerConfiguracionEnsaladasPath);
                    var resultado = JsonConvert.DeserializeObject<EnsaladasContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");

                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerConfiguracionEnsaladas | Error al contactar el servidor : " + e.Message);
#endif
                    return new EnsaladasContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }

            public static async Task<PlatilloContainer> LlamarObtenerPlatillosMenu(string idMenu, string idRestaurante)
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"menuId", idMenu},
                        {"sucursalId", idRestaurante}
                    };

                    //var client = new RestApiClient(QdcApiConfig.QdcAppSecret);
                    //var resultadostring = await client.Post(QdcApiConfig.ObtenerPlatillosMenuPath, parameters);
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerPlatillosMenuPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<PlatilloContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");

                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerPlatillosMenu | Error al contactar el servidor : " + e.Message);
#endif
                    return new PlatilloContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }

            public static async Task<DirectorioContainer> LlamarObtenerDirectorio()
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerDirectorioPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<DirectorioContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerDirectorio | Error al contactar el servidor : " + e.Message);
#endif
                    return new DirectorioContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
        }

        internal static class OpenPay
        {
            public static async Task<AddCardContainer> RegistrarTarjetaNueva(string token, string sessionId, string holderName, string maskedNumber, string brand)
            {

                var parameters = new Dictionary<string, string>()
                {
                    {"tokenId", token },
                    {"deviceSesionId", sessionId },
                    {"consumidorId", MystiqueApp.Usuario.Id },
                    {"holderName", holderName },
                    {"cardNumber", maskedNumber },
                    {"brand", brand },
                };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.RegistarTarjetaPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<AddCardContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>RegistrarTarjetaNueva | Error al contactar el servidor : " + e.Message);
#endif
                    return new AddCardContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }

            public static async Task<CardContainer> ObtenerTarjetas()
            {

                var parameters = new Dictionary<string, string>()
                {
                    {"consumidorId", MystiqueApp.Usuario.Id },
                };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerTarjetasPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<CardContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>ObtenerTarjetas | Error al contactar el servidor : " + e.Message);
#endif
                    return new CardContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }

            public static async Task<BaseContainer> RemoverTarjeta(string token)
            {

                var parameters = new Dictionary<string, string>()
                {
                    {"tokenId", token },
                    {"consumidorId", MystiqueApp.Usuario.Id },
                };

                try
                {
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.RemoverTarjetasPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<BaseContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>RemoverTarjeta | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
        }

        internal static class Direccion
        {
            public static async Task<DirectionsContainer> LlamarAgregarDireccion(Direction direccion)
            {
                try
                {

                    var parameters = new Dictionary<string, string>()
                    {
                        {"alias", direccion.Nombre },
                        {"latitud", direccion.Latitud.ToString("F10") },
                        {"longitud", direccion.Longitud.ToString("F10") },
                        {"calle", direccion.Thoroughfare },
                        {"numeroExt", direccion.SubThoroughfare },
                        {"nombreColonia", direccion.SubLocality },
                        {"referencias", direccion.OtherAddressLines  },
                        {"activo", $"{true}" },
                        {"consumidorId", MystiqueApp.Usuario.Id},
                    };
                    if (direccion.IdColonia != 0)
                    {
                        parameters.Add("coloniaId", $"{direccion.IdColonia}");
                    }
                    if (!string.IsNullOrEmpty(direccion.PostalCode))
                    {
                        parameters.Add("codigoPostal", $"{direccion.PostalCode}");
                    }

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.AgregarDireccionPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<DirectionsContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarAgregarDireccion | Error al contactar el servidor : " + e.Message);
#endif
                    return new DirectionsContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }

            public static async Task<DirectionsContainer> LlamarEditarDireccion(Direction direccion, bool active)
            {
                try
                {

                    var parameters = new Dictionary<string, string>()
                    {
                        {"direccionId", direccion.Id.ToString() },
                        {"alias", direccion.Nombre },
                        {"latitud", direccion.Latitud.ToString("F10") },
                        {"longitud", direccion.Longitud.ToString("F10") },
                        {"calle", direccion.Thoroughfare },
                        {"numeroExt", direccion.SubThoroughfare },
                        {"nombreColonia", direccion.SubLocality },
                        {"referencias", direccion.OtherAddressLines  },
                        {"codigoPostal", direccion.PostalCode },
                        {"activo", $"{active}"},
                        {"consumidorId", MystiqueApp.Usuario.Id },
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.EditarDireccionPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<DirectionsContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarEditarDireccion | Error al contactar el servidor : " + e.Message);
#endif
                    return new DirectionsContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }

            public static async Task<DirectionsContainer> LlamarObtenerDirecciones(int idRestaurante)
            {
                try
                {

                    var parameters = new Dictionary<string, string>()
                    {
                        {"consumidorId", MystiqueApp.Usuario.Id },
                    };
                    if (idRestaurante > 0)
                    {
                        parameters.Add("sucursalId", $"{idRestaurante}");
                    }

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerDireccionesPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<DirectionsContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerDirecciones | Error al contactar el servidor : " + e.Message);
#endif
                    return new DirectionsContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }

            public static async Task<ColoniasContainer> LlamarObtenerColonias(Point ubicacion, string codigoPostal, int idRestaurante)
            {
                try
                {

                    var parameters = new Dictionary<string, string>()
                    {
                        {"consumidorId", MystiqueApp.Usuario.Id },
                        {"latitud", $"{ubicacion.Latitude:F10}"},
                        {"longitud", $"{ubicacion.Longitude:F10}"},
                        {"codigoPostal", codigoPostal },
                    };

                    if (idRestaurante > 0)
                    {
                        parameters.Add("sucursalId", $"{idRestaurante}");
                    }

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerColoniasPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<ColoniasContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerColonias | Error al contactar el servidor : " + e.Message);
#endif
                    return new ColoniasContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
        }

        internal static class Orden
        {
            public static async Task<BaseContainer> LlamarAgregarPedido(NuevaOrden nuevaOrden)
            {
                try
                {
                    nuevaOrden.IdConsumidor = int.Parse(MystiqueApp.Usuario.Id);
                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.RegistrarPedidoPath, nuevaOrden);
                    var resultado = JsonConvert.DeserializeObject<BaseContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarAgregarPedido | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
            public static async Task<OrdenesContainer> LlamarObtenerPedidosActivos()
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"consumidorId", MystiqueApp.Usuario.Id },
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerPedidosActivosPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<OrdenesContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerPedidosActivos | Error al contactar el servidor : " + e.Message);
#endif
                    return new OrdenesContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
            public static async Task<BaseContainer> LlamarAgregarSeguimiento(int idPedido, string mensaje)
            {
                try
                {

                    var parameters = new Dictionary<string, string>()
                    {
                        {"pedidoId", $"{idPedido}"},
                        {"mensaje", mensaje },
                        {"consumidorId", MystiqueApp.Usuario.Id },
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.RegistrarMensajePedidoPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<BaseContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarAgregarSeguimiento | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
            public static async Task<DetalleOrdenContainer> LlamarObtenerDetallePedido(int idPedido)
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"pedidoId", $"{idPedido}"},
                        {"consumidorId", MystiqueApp.Usuario.Id },
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerDetallePedidoPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<DetalleOrdenContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerDetallePedido | Error al contactar el servidor : " + e.Message);
#endif
                    return new DetalleOrdenContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
            public static async Task<HistorialContainer> LlamarObtenerHistorialPedidos()
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"consumidorId", MystiqueApp.Usuario.Id },
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerHistorialPedidosPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<HistorialContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerHistorialPedidos | Error al contactar el servidor : " + e.Message);
#endif
                    return new HistorialContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
            public static async Task<BaseContainer> LlamarCalificarPedido(int idPedido, int calificacionProducto, int calificacionReparto, int calificacionMovil)
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"pedidoId", $"{idPedido}"},
                        {"calProducto", $"{calificacionProducto}"},
                        {"calReparticion", $"{calificacionReparto}"},
                        {"calMovil", $"{calificacionMovil}"},
                        {"consumidorId", MystiqueApp.Usuario.Id },
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.CalificarPedidoPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<BaseContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarCalificarPedido | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
        }

        internal static class Notificaciones
        {
            public static async Task<NotificacionesContainer_HP> LlamarObtenerNotificaciones()
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"consumidorId", MystiqueApp.Usuario.Id },
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ObtenerNotificacionesHPPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<NotificacionesContainer_HP>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarObtenerNotificaciones | Error al contactar el servidor : " + e.Message);
#endif
                    return new NotificacionesContainer_HP { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
            public static async Task<BaseContainer> LlamarActualizarNotificaciones()
            {
                try
                {
                    var parameters = new Dictionary<string, string>()
                    {
                        {"consumidorId", MystiqueApp.Usuario.Id },
                    };

                    var resultadostring = await RestApiClient.Post(MystiqueApiV2Config.ActualizarNotificacionesHPPath, parameters);
                    var resultado = JsonConvert.DeserializeObject<BaseContainer>(resultadostring);

                    if (resultado == null)
                    {
                        throw new WebException("Null response");
                    }
                    return resultado;
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine("~ QdcApi>LlamarActualizarNotificaciones | Error al contactar el servidor : " + e.Message);
#endif
                    return new BaseContainer { Estatus = new BaseModel { ResponseCode = ResponseTypes.CodigoNoConexion, Message = "Hubo un problema al conectarse con la red Quiero de Comer" } };
                }
            }
        }
    }
}
