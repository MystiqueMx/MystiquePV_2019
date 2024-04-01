using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.OpenPay
{
    public class Error
    {
        [JsonProperty("request_id")]
        public string Id { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("http_code")]
        public int HttpCode { get; set; }

        /// <summary>
        /// See https://www.openpay.mx/en/docs/api/#error-codes
        /// </summary>
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("fraud_rules")]
        public string[] FraudRules { get; set; }
    }
}
