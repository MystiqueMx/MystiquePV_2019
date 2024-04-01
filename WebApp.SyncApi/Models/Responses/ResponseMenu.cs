using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApp.SyncApi.Helpers.Base;

namespace WebApp.SyncApi.Models.Responses
{
    public class ResponseMenu : ResponseBase
    {
        public ResponseEntidadesMenu respuesta { get; set; }

    }

    public class ResponseEntidadesMenu
    {
        public List<EntidadCategoriaInsumo> ListadoCategoriaInsumo { get; set; } = new List<EntidadCategoriaInsumo>();
        public List<EntidadCategoriaProductos> ListadoCategoriaProductos { get; set; } = new List<EntidadCategoriaProductos>();
        public List<EntidadAreaPreparacion> ListadoAreaPreparacion { get; set; } = new List<EntidadAreaPreparacion>();
        public List<EntidadProductos> ListadoProductos { get; set; } = new List<EntidadProductos>();
        public List<EntidadVariedadProductos> ListadoVariedadProductos { get; set; } = new List<EntidadVariedadProductos>();
        public List<EntidadAgrupadorInsumos> ListadoAgrupadorInsumos { get; set; } = new List<EntidadAgrupadorInsumos>();
        public List<EntidadInsumoProductos> ListadoInsumoProductos { get; set; } = new List<EntidadInsumoProductos>();
        public List<EntidadConfiguracionArmadoProducto> ListadoConfiguracionArmadoProducto { get; set; } = new List<EntidadConfiguracionArmadoProducto>();
        public List<EntidadColonias> ListadoColonias { get; set; } = new List<EntidadColonias>();
        public EntidadConfiguracionSucursal ListadoConfiguracionSucursal { get; set; } = new EntidadConfiguracionSucursal();
        public List<EntidadInsumos> ListadoInsumos { get; set; } = new List<EntidadInsumos>();
        public List<EntidadDescuentos> ListadoDescuentos { get; set; } = new List<EntidadDescuentos>();
        public List<EntidadTipoPagos> ListadoTiposPagos { get; set; } = new List<EntidadTipoPagos>();
    }


    public class EntidadCategoriaInsumo
    {      
        public int id { get; set; }      
        public string nombre { get; set; }
    }


    public class EntidadCategoriaProductos
    {        
        public int id { get; set; }        
        public string nombre { get; set; }     
        public string codigo { get; set; }    
        public int indice { get; set; }
    }

    public class EntidadAreaPreparacion
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }

    public class EntidadProductos
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int? categoriaProductoId { get; set; }
        public decimal precio { get; set; }
        public string imagen { get; set; }  
        public int? indice { get; set; }
        public int? areaPreparacionId { get; set; }      
        public bool esCombo { get; set; }
        public bool tieneVariedad { get; set; }
        public bool esEnsalada { get; set; }
        public string clave { get; set; }
        public bool principal { get; set; }

    }


    public class EntidadVariedadProductos
    {
        public int id { get; set; }
        public int productoId { get; set; }
        public string nombre { get; set; }
        public string imagen { get; set; }
    }

    public class EntidadAgrupadorInsumos
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public bool confirmarPorSeparado { get; set; }
        public int indice { get; set; }
        public bool agregarExtra { get; set; }
        public int? productoId { get; set; }

    }

    public class EntidadInsumoProductos
    {
        public int id { get; set; }
        public int? productoId { get; set; }
        public int agrupadorInsumoId { get; set; }        
        public int? insumoId { get; set; }
    }

    public class EntidadConfiguracionArmadoProducto
    {
        public int id { get; set; }
        public int productoId { get; set; }
        public int agrupadorInsumoId { get; set; }
        public int cantidad { get; set; }
    }


    public class EntidadColonias
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }

    public class EntidadConfiguracionSucursal
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string razonSocial { get; set; }
        public string rfc { get; set; }        
        public string direccion { get; set; }
        public decimal? maxEfectivo { get; set; } 
        public decimal? costoEnvio { get; set; }
        public string faceBook { get; set; } 
        public string sitioWeb { get; set; } 
        public decimal? iva { get; set; } 
        public string mensajeTicket { get; set; }
        public string direccionFiscal { get; set; }
        public string regimenFiscal { get; set; }
        public string logo { get; set; }
    }

    public class EntidadInsumos
    {
        public int id { get; set; }
        public string nombre { get; set; }       
        public int? categoriaInsumoId { get; set; }
        public string imagen { get; set; }        

    }


    public class EntidadDescuentos
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public DateTimeOffset fechaInicio { get; set; }
        public DateTimeOffset fechaFin { get; set; }
        public TimeSpan horaInicio { get; set; }
        public TimeSpan horaFin { get; set; }
        public decimal montoPorcentaje { get; set; }
        public bool lunes { get; set; }
        public bool martes { get; set; }
        public bool miercoles { get; set; }
        public bool jueves { get; set; }
        public bool viernes { get; set; }
        public bool sabado { get; set; }
        public bool domingo { get; set; }
        public int? productoId { get; set; }
    }


    public class EntidadTipoPagos
    {
        public int id { get; set; }
        public string nombre { get; set; }  
        public bool activo { get; set; }
    }

}