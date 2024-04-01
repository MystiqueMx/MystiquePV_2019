using MystiqueNative.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Clientes
{
    public class ClienteContainer : BaseContainer
    {
        [JsonProperty("ListaClientesCallCenter")]
        public List<ClienteCallCenter> listaCliente { get; set; }

        [JsonProperty("existe")]
        public bool encontrado { get; set; }
    }

    public class ClienteCallCenter
    {
        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("nombreCompleto")]
        public string nombreCompleto { get; set; }

        [JsonProperty("telefono")]
        public string telefono { get; set; }
    }
}
