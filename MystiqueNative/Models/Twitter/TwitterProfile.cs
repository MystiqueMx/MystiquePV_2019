using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Twitter
{
    /// <summary>
    /// <see cref="https://developer.twitter.com/en/docs/accounts-and-users/manage-account-settings/api-reference/get-account-verify_credentials"/>
    /// </summary>
    public class TwitterProfile
    {
        [JsonProperty("created_at")]
        public string FechaRegistro { get; set; }
        [JsonProperty("id_str")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Nombre { get; set; }
        [JsonProperty("profile_image_url_https")]
        public string UrlImagenPerfil { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        public bool Success { get; internal set; }
    }
}
