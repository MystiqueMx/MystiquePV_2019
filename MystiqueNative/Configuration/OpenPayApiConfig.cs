using System;
using System.Collections.Generic;
using System.Text;

namespace MystiqueNative.Configuration
{
    public class OpenPayApiConfig
    {
        #region CONFIG
#if DEBUG
        //LOCAL
        internal const string UrlApi = "https://sandbox-api.openpay.mx/v1/";
        internal const string PublicKey = @"pk_0b5b5bc175ef4786840e44f14b05ecd5";
        internal const string MerchantId = @"mxblow1y4fbclv9687rv";
#else
        //EXTERNA
         internal const string UrlApi = "https://sandbox-api.openpay.mx/v1/";
        internal const string PublicKey = @"pk_0b5b5bc175ef4786840e44f14b05ecd5";
        internal const string MerchantId = @"mxblow1y4fbclv9687rv";
        //private const string UrlApi = "https://api.openpay.mx/v1/";
        //internal const string PublicKey = @"pk_0b5b5bc175ef4786840e44f14b05ecd5";
        //internal const string MerchantId = @"mxblow1y4fbclv9687rv";
#endif
        #endregion

        #region ENDPOINTS
        private const string ObtenerTokenRoute = @"/tokens";
        #endregion
        #region API
        internal static string ObtenerTokenPath => $"{UrlApi}{MerchantId}{ObtenerTokenRoute}";
        #endregion
    }
}
