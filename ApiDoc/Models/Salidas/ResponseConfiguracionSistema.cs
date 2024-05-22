using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Salidas
{
    public class ResponseConfiguracionSistema : ResponseBase
    {
        public ResponseConfiSistema configuraciones { get; set; }
        public List<ResponseConfiSistemaComercios> listaComercios { get; set; }
}

    public class ResponseConfiSistema
    {
        public Nullable<int> empresaId { get; set; }
        public string telefonoContacto { get; set; }
        public string correoContacto { get; set; }
        public string txtTerminosCondiciones { get; set; }
        public string txtSoporte { get; set; }
        public string versionAndroid { get; set; }
        public string versionAndroidTest { get; set; }
        public string versioniOS { get; set; }
        public string versioniOSTest { get; set; }
        public Nullable<int> idQDC { get; set; }
        public bool? mostrarComercios { get; set; }
        public bool? mostrarSucursales { get; set; }

        public string soporteLinea1Ingles { get; set; }
        public string soporteLinea2Ingles { get; set; }
    }



    public class ResponseConfiSistemaComercios
    {
        public int empresaId { get; set; }
        public int idComercio { get; set; }
        public string nombreComercial { get; set; }
        public string logoUrl { get; set; }        
    }

}