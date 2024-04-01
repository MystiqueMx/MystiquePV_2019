using System.Collections.Generic;
using MystiqueNative.Helpers;
using MystiqueNative.Models;

namespace MystiqueNative.ViewModels
{
    public class ValidarTicketEventArgs : BaseEventArgs
    {
        public Ticket TicketEscaneado { get; set; }
        public List<UsoCfdi> UsosCfdi { get; set; }
        public List<ReceptorFactura> ReceptoresGuardados { get; set; }
    }
}