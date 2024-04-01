using System;
using System.Text;

namespace MystiqueNative.Models
{
    public class Ticket
    {
        public string Id { get; set; }
        public string Sucursal { get; set; }
        public string FechaCompra { get; set; }
        public string MontoCompra { get; set; }
        public string SucursalId { get; set; }
        public bool PendienteTicket { get; set; }
        /// <summary>
        /// //////////////////////////////////////
        /// </summary>
        public string FechaCompraConFormatoEspanyol
        {
            get
            {
                if (string.IsNullOrEmpty(FechaCompra) )
                    return FechaCompra;
                else
                {
                    if (1900 > FechaCompraAsDateTime.Value.Year)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return DateTime.Parse(FechaCompra).ToString("dd/MM/yyyy");
                    }
                }
            }
        }
        public DateTime? FechaCompraAsDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(FechaCompra))
                    return null;
                else
                    return DateTime.Parse(FechaCompra);
            }
        }
    }
}
