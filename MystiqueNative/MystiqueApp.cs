using MystiqueNative.Models;
using MystiqueNative.Models.Configuration;
using MystiqueNative.Models.Facebook;
using MystiqueNative.Models.Location;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MystiqueNative
{
    public static class MystiqueApp
    {
        public static event PropertyChangedEventHandler PropertyChanged;
        static void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke("STATIC PROPERTY", new PropertyChangedEventArgs(name));
        }
        public static string PlayerId { get => playerId; set { playerId = value; OnPropertyChanged(nameof(PlayerId)); } }

        public static int PedidosActivos { get; set; }

        public static string FacebookId { get; set; }
        public static FacebookProfile FbProfile { get; set; }
        public static string FbProfilePicture { get; set; }
        //TODO Move strings to this Models
        public static User Usuario { get; set; }
        internal static DeviceInfo Device { get; set; }
        public static Configuracion Config { get; set; }


        internal static string ID_CLIENTE { get; set; }
        internal static string ID_MEMBRESIA { get; set; }
        internal static string USERNAME { get; set; }
        internal static string PASSWORD { get; set; }

        public static string DeviceId { get; set; }
        public static string DeviceModel { get; set; }
        public static string DevicePlatform { get; set; }
        public static string DeviceVersion { get; set; }
        public static string DeviceConnectionType { get; set; }

        public static List<Comercio> Comercios { get; internal set; }
        public static string Comercio{
            get
            {
                return Comercios.FirstOrDefault()?.Id;
            }
        }
        public static string VersionAndroid { get; internal set; }
        public static string VersionAndroidPruebas { get; internal set; }
        public static string VersionIOS { get; internal set; }
        public static string VersionIOSPruebas { get; internal set; }
        public static string DescripcionSoporte { get; internal set; }
        public static string TerminosSoporte { get; internal set; }
        public static string EmailSoporte { get; internal set; }
        public static string TelefonoSoporte { get; internal set; }
        public static string IdQDC { get ; internal set;}

        private static string playerId;
        
        public static Point UltimaUbicacionConocida { get; set; }
        public static Direction UltimaDireccionConocida { get; set; }

        public static async Task Obtenerconfiguracion()
        {
            var config = await Services.MystiqueApiV2.Configuracion.CallObtenerConfiguracion();
            if (config.Success)
            {
                Config = config.Configuracion;
                Comercios = config.Comercios;
                VersionAndroid = config.Configuracion.VersionAndroid;
                VersionIOS = config.Configuracion.VersionIOS;
                TerminosSoporte = config.Configuracion.TerminosSoporte;
                TelefonoSoporte = config.Configuracion.TelefonoSoporte;
                EmailSoporte = config.Configuracion.EmailSoporte;
                DescripcionSoporte = config.Configuracion.DescripcionSoporte;
                IdQDC = config.Configuracion.IdQDC;
                VersionIOSPruebas = config.Configuracion.VersionIOSPruebas;
                VersionAndroidPruebas = config.Configuracion.VersionAndroidPruebas;
            }
        }
    }
}
