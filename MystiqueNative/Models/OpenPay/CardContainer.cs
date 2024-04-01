using MystiqueNative.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.OpenPay
{
    public class CardContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public List<Card> Respuesta { get; set; }
    }
    public class AddCardContainer : BaseContainer
    {
        [JsonProperty("respuesta")]
        public Card Respuesta { get; set; }
    }
}
