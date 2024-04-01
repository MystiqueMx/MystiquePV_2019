using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Models.Facebook
{
    public class FacebookProfile
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("age_range")]
        public string AgeRange { get; set; }
        [JsonProperty("permissions")]
        public string Permissions { get; set; }
    }
}
