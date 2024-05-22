using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDoc.Models.Entradas
{
    public class RequestWallet : RequestBase
    {
        public int beneficioId { get; set; }
        public int clienteId { get; set; }

        public int empresaId { get; set; }
    }


    public class RequestWalletColeccion : RequestBase
    {
        public List<int> ListWalletId { get; set; }
    }

    public class RequestWalletObtener : RequestBase
    {
        public int clienteId { get; set; }
    }
}