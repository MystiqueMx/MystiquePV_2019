using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class Configuracion
    {
        #region Soporte
        [JsonProperty("telefonoContacto")]
        public string TelefonoSoporte { get; set; }
        [JsonProperty("correoContacto")]
        public string EmailSoporte { get; set; }
        [JsonProperty("txtTerminosCondiciones")]
        public string TerminosSoporte { get; set; }
        [JsonProperty("txtSoporte")]
        public string DescripcionSoporte { get; set; }
        #endregion
        #region Versiones
        [JsonProperty("versionAndroid")]
        public string VersionAndroid { get; set; }
        [JsonProperty("versionAndroidTest")]
        public string VersionAndroidPruebas { get; set; }
        [JsonProperty("versioniOS")]
        public string VersionIOS { get; set; }
        [JsonProperty("versioniOSTest")]
        public string VersionIOSPruebas { get; set; }
        #endregion
        #region QDC
        [JsonProperty("idQDC")]
        public string IdQDC { get; set; }
        #endregion
        [JsonProperty("mostrarComercios")]
        public bool MostrarComercios { get; set; }
        [JsonProperty("mostrarSucursales")]
        public bool MostrarSucursales { get; set; }
    }
}
