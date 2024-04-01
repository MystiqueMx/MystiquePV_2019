using MystiqueMC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MystiqueMcApi.Models.Salidas
{
    public class ResponseWallet : ResponseBase
    {
    }

    public class ResponseListaWallet : ResponseBase
    {

        public List<SP_Obtener_ListaWallet_Cliente_Result> listaWallet { get; set; }
    }
}