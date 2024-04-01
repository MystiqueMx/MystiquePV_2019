using MystiqueNative.Configuration;
using MystiqueNative.Models.Twitter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MystiqueNative.Helpers.Twitter
{
    public static class Authorization
    {
        public static string GetAuthenticatedHeader(Uri uri, RequestToken AccessToken, HttpMethod httpMethod = null, Dictionary<string,string> parameters = null)
        {
            if (httpMethod == null)
                httpMethod = HttpMethod.Get;

            var nonce = Guid.NewGuid().ToString().Replace("-", "");
            var timestamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture);

            var oauthParameters = new SortedDictionary<string, string>
            {
                {"oauth_consumer_key", TwitterApiConfig.Secrets.ConsumerKey},
                {"oauth_nonce", nonce},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", timestamp},
                {"oauth_token", AccessToken.OauthToken},
                {"oauth_version", "1.0"}
            };
            

            var signingParameters = new SortedDictionary<string, string>(oauthParameters);

            var parsedQuery = HttpUtility.ParseQueryString(uri.Query);
            foreach (var k in parsedQuery.AllKeys)
            {
                signingParameters.Add(k, parsedQuery[k]);
            }
            if (parameters != null)
            {
                foreach (var p in parameters.Keys)
                {
                    signingParameters.Add(p, parameters[p]);
                }
            }

            var builder = new UriBuilder(uri) { Query = "" };
            var baseUrl = builder.Uri.AbsoluteUri;

            var parameterString = string.Join("&",
                                    from k in signingParameters.Keys
                                    select Uri.EscapeDataString(k) + "=" +
                                           Uri.EscapeDataString(signingParameters[k]));

            var stringToSign = string.Join("&", new[] { httpMethod.Method.ToUpper(), baseUrl, parameterString }.Select(Uri.EscapeDataString));
            var signingKey = Uri.EscapeDataString(TwitterApiConfig.Secrets.ConsumerSecret) + "&" + Uri.EscapeDataString(AccessToken.OauthTokenSecret);
            var signature = SignWithSHA1(signingKey, stringToSign);

            oauthParameters.Add("oauth_signature", signature);

            var authHeader = string.Join(", ", from k in oauthParameters.Keys
                                               select string.Format(@"{0}=""{1}""",
                                                   Uri.EscapeDataString(k),
                                                   Uri.EscapeDataString(oauthParameters[k])));

            return "OAuth " + authHeader;
        }
        public static string GetRequestTokenHeader(Uri uri, string CallbackUrl, HttpMethod httpMethod = null)
        {
            if (httpMethod == null)
                httpMethod = HttpMethod.Get;

            var nonce = Guid.NewGuid().ToString().Replace("-", "");
            var timestamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture);

            var oauthParameters = new SortedDictionary<string, string>
            {
                {"oauth_callback", CallbackUrl},
                {"oauth_consumer_key", TwitterApiConfig.Secrets.ConsumerKey},
                {"oauth_nonce", nonce},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", timestamp},
                {"oauth_version", "1.0"}
            };

            var signingParameters = new SortedDictionary<string, string>(oauthParameters);

            var parsedQuery = HttpUtility.ParseQueryString(uri.Query);
            foreach (var k in parsedQuery.AllKeys)
            {
                signingParameters.Add(k, parsedQuery[k]);
            }

            var builder = new UriBuilder(uri) { Query = "" };
            var baseUrl = builder.Uri.AbsoluteUri;

            var parameterString = string.Join("&", signingParameters.Keys.Select(c=> Uri.EscapeDataString(c) + "=" + Uri.EscapeDataString(signingParameters[c])));

            var stringToSign = string.Join("&", new[] { httpMethod.Method.ToUpper(), baseUrl, parameterString }.Select(Uri.EscapeDataString));
            var signingKey = Uri.EscapeDataString(TwitterApiConfig.Secrets.ConsumerSecret) + "&";
            var signature = SignWithSHA1(signingKey, stringToSign);

            oauthParameters.Add("oauth_signature", signature);

            var authHeader = string.Join(", ", oauthParameters.Keys.Select(c=> string.Format(@"{0}=""{1}""", Uri.EscapeDataString(c), Uri.EscapeDataString(oauthParameters[c]))));

            return "OAuth " + authHeader;
        }
        public static string GetAccessTokenHeader(Uri uri, RequestToken AccessToken, HttpMethod httpMethod = null, Dictionary<string, string> parameters = null)
        {
            if (httpMethod == null)
                httpMethod = HttpMethod.Get;

            var nonce = Guid.NewGuid().ToString().Replace("-", "");
            var timestamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture);

            var oauthParameters = new SortedDictionary<string, string>
            {
                {"oauth_consumer_key", TwitterApiConfig.Secrets.ConsumerKey},
                {"oauth_nonce", nonce},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", timestamp},
                {"oauth_token", AccessToken.OauthToken},
                {"oauth_verifier", AccessToken.OauthTokenSecret},
                {"oauth_version", "1.0"}
            };


            var signingParameters = new SortedDictionary<string, string>(oauthParameters);

            var parsedQuery = HttpUtility.ParseQueryString(uri.Query);
            foreach (var k in parsedQuery.AllKeys)
            {
                signingParameters.Add(k, parsedQuery[k]);
            }
            if (parameters != null)
            {
                foreach (var p in parameters.Keys)
                {
                    signingParameters.Add(p, parameters[p]);
                }
            }

            var builder = new UriBuilder(uri) { Query = "" };
            var baseUrl = builder.Uri.AbsoluteUri;

            var parameterString = string.Join("&",
                                    from k in signingParameters.Keys
                                    select Uri.EscapeDataString(k) + "=" +
                                           Uri.EscapeDataString(signingParameters[k]));

            var stringToSign = string.Join("&", new[] { httpMethod.Method.ToUpper(), baseUrl, parameterString }.Select(Uri.EscapeDataString));
            var signingKey = Uri.EscapeDataString(TwitterApiConfig.Secrets.ConsumerSecret) + "&" + Uri.EscapeDataString(AccessToken.OauthTokenSecret);
            var signature = SignWithSHA1(signingKey, stringToSign);

            oauthParameters.Add("oauth_signature", signature);

            var authHeader = string.Join(", ", from k in oauthParameters.Keys
                                               select string.Format(@"{0}=""{1}""",
                                                   Uri.EscapeDataString(k),
                                                   Uri.EscapeDataString(oauthParameters[k])));

            return "OAuth " + authHeader;
        }
        #region Helpers
        public static string SignWithSHA1(string key, string content) => 
            Convert.ToBase64String( new HMACSHA1(Encoding.UTF8.GetBytes(key)).ComputeHash(Encoding.UTF8.GetBytes(content)) );
        #endregion

    }
}
