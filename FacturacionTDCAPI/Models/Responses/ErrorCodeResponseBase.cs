using Newtonsoft.Json;

namespace FacturacionTDCAPI.Models.Responses
{
    public class ErrorCodeResponseBase
    {
        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}