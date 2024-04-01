using System;
using System.Collections.Generic;
using System.Text;
using MystiqueNative.Models.Twitter;

namespace MystiqueNative.Configuration
{
    public static class TwitterApiConfig
    {
        #region OAUTH
        public const string CallbackUrl = "mystique://";
        public static SecretModel Secrets => new SecretModel
        {
            ConsumerKey = "chD9l4nZddfnaRf0h5on3NN3q",
            ConsumerSecret = "doddD9HBEbQfzU92Oh2RklYqSZmd44RIzawX0G9AlNb9sItvzd",
            AccessToken = "1014197337428840448-CS5w3Qnzq3yz1ejC9dsizx2KhYpWKf",
            AccessTokenSecret = "1014197337428840448-CS5w3Qnzq3yz1ejC9dsizx2KhYpWKf" };
        #endregion
        #region URL
        private const string UrlApi = "https://api.twitter.com";
        internal static string RequestTokenPath => $"{UrlApi}/oauth/request_token";
        internal static string AuthenticatePath => $"{UrlApi}/oauth/authenticate";
        internal static string AccessTokenPath => $"{UrlApi}/oauth/access_token";
        internal static string VerifyCredentialsPath => $"{UrlApi}/1.1/account/verify_credentials.json";

        #endregion
    }

}
