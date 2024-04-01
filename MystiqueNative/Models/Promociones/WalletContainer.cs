using System.Collections.Generic;
using MystiqueNative.Helpers;
using Newtonsoft.Json;

namespace MystiqueNative.Models
{
    public class WalletContainer : BaseModel
    {
        [JsonProperty("listaWallet")]
        public List<BeneficiosWallet> ListaWalletItems { get; set; }
    }
}