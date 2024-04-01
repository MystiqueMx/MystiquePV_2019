using Newtonsoft.Json;
using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class MovimientoCitypoints
    {

        [JsonProperty("idMovimientoPuntos")]
        public string Id { get; set; }
        [JsonProperty("membresiaId")]
        public string IdMembresia { get; set; }
        [JsonProperty("fechaRegistro")]
        public string FechaRegistroAsString { get; set; }
        [JsonProperty("fechaCompraTicket")]
        public string FechaCompraAsString { get; set; }
        [JsonProperty("puntos")]
        public string Puntos { get; set; }
        [JsonProperty("catEstatusAplicaPuntoId")]
        public string Estatus { get; set; }
        [JsonProperty("montoCompra")]
        public string Monto { get; set; }
        [JsonProperty("producto")]
        public string Producto { get; set; }
        [JsonProperty("folioCompra")]
        public string Folio { get; set; }

        public bool IsUp
        {
            get
            {
                if (int.TryParse(Estatus, out int p))
                    return p == 1;
                else
                    return false;
            }
        }
        public int PuntosAsInt
        {
            get
            {
                if (float.TryParse(Puntos, out float p))
                    return (int)p;
                else
                    throw new ArgumentException();
            }
        }
        public int MontoAsInt
        {
            get
            {
                if (Monto == null) return 0;
                if (float.TryParse(Monto, out float p))
                    return (int)p;
                else
                    throw new ArgumentException();
            }
        }
        public string FechaRegistroConFormatoEspanyol
        {
            get
            {
                if (string.IsNullOrEmpty(FechaRegistroAsString))
                    return FechaRegistroAsString;
                else
                {
                    if (1900 > FechaRegistroAsDateTime.Value.Year)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return DateTime.Parse(FechaRegistroAsString).ToString("dd/MM/yyyy");
                    }
                }
            }
        }
        public DateTime? FechaRegistroAsDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(FechaRegistroAsString))
                    return null;
                else
                    return DateTime.Parse(FechaRegistroAsString);
            }
        }
        public string FechaCompraConFormatoEspanyol
        {
            get
            {
                if (string.IsNullOrEmpty(FechaCompraAsString))
                    return FechaCompraAsString;
                else
                {
                    if (1900 > FechaCompraAsDateTime.Value.Year)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return DateTime.Parse(FechaCompraAsString).ToString("dd/MM/yyyy");
                    }
                }
            }
        }
        public DateTime? FechaCompraAsDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(FechaCompraAsString))
                    return null;
                else
                    return DateTime.Parse(FechaCompraAsString);
            }
        }

    }
}
