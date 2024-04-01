using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Configuration
{
    public class ConektaApiConfig
    {
        #region CONFIG
#if DEBUG

        internal const string UrlApi = "https://api.conekta.io";
        public const string UriConektaJs = "https://conektaapi.s3.amazonaws.com/v0.3.2/js/conekta.js";
        internal const string ApiVersion = @"2.0.0";

        //Key para Fresco
        //DEV
        public const string PublicKey = @"key_PSNAEaduhyvt6yAzRLHsGiQ";
#else
        internal const string UrlApi = "https://api.conekta.io";
        public const string UriConektaJs = "https://conektaapi.s3.amazonaws.com/v0.3.2/js/conekta.js";
        internal const string ApiVersion = @"2.0.0";

        //Key para Fresco
        public const string PublicKey = @"key_ZHrQymzePausMZanZ64XJ4w";
        
#endif
        #endregion
    }
}
