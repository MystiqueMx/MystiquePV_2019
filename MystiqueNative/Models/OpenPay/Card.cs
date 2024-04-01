using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.OpenPay
{
    public class Card
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("creation_date")]
        public string CreatedAt { get; set; }

        [JsonProperty("holder_name")]
        public string HolderName { get; set; }

        [JsonProperty("card_number")]
        public string CardNumber { get; set; }

        [JsonProperty("cvv2")]
        public string Cvv { get; set; }

        [JsonProperty("expiration_month")]
        public string ExpirationMonth { get; set; }

        [JsonProperty("expiration_year")]
        public string ExpirationYear { get; set; }

        [JsonProperty("address")]
        public Address AddressObj { get; set; }

        [JsonProperty("allows_charges")]
        public bool AllowsCharges { get; set; }

        [JsonProperty("allows_payouts")]
        public string AllowsPayouts { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("bank_name")]
        public string BankName { get; set; }

        [JsonProperty("bank_code")]
        public string BankCode { get; set; }

        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        [JsonProperty("points_card")]
        public bool PointsCard { get; set; }

        [JsonIgnore]
        public string MaskedCardNumber => CardNumber.Substring(CardNumber.Length - 4, 4).PadLeft(16, '*');
    }
}
