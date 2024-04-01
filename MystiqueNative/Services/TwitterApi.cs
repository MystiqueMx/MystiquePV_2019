using MystiqueNative.Configuration;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Twitter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.Services
{
    public static class TwitterApi
    {
        public static class OAuth
        {
            public static async Task<RequestToken> SolicitarRequestToken(string callbackUrl = null)
            {
                try
                {
                    if (string.IsNullOrEmpty(callbackUrl)) callbackUrl = TwitterApiConfig.CallbackUrl;
                    var requestToken = await TwitterApiClient.RequestToken(callbackUrl);
                    if (!string.IsNullOrEmpty(requestToken))
                    {
                        var parsedRequestToken = System.Web.HttpUtility.ParseQueryString(requestToken);
                        if (bool.TryParse(parsedRequestToken["oauth_callback_confirmed"], out var callbackConfirmed))
                        {
                            return new RequestToken
                            {
                                OauthCallbackConfirmed = callbackConfirmed,
                                OauthToken = parsedRequestToken["oauth_token"],
                                OauthTokenSecret = parsedRequestToken["oauth_token_secret"]
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex);
#endif
                }

                return new RequestToken { OauthCallbackConfirmed = false };
            }
            public static async Task<RequestToken> SolicitarAccessToken(string oauthToken, string oauthVerifier)
            {
                try
                {
                    var token = new RequestToken
                    {
                        OauthToken = oauthToken,
                        OauthTokenSecret = oauthVerifier,
                    };
                    var requestToken = await TwitterApiClient.RequestAccessToken(token);
                    if (!string.IsNullOrEmpty(requestToken))
                    {
                        var parsedRequestToken = System.Web.HttpUtility.ParseQueryString(requestToken);
                        if (!string.IsNullOrEmpty(parsedRequestToken["oauth_token"]))
                        {
                            return new RequestToken
                            {
                                OauthCallbackConfirmed = true,
                                OauthToken = parsedRequestToken["oauth_token"],
                                OauthTokenSecret = parsedRequestToken["oauth_token_secret"],
                                ScreenName = parsedRequestToken["screen_name"],
                                UserId = parsedRequestToken["user_id"],
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex);
#endif
                }

                return new RequestToken { OauthCallbackConfirmed = false };
            }
        }
        public static class Account
        {
            public static async Task<TwitterProfile> SolicitarVerificarCredenciales(RequestToken token)
            {
                try
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>()
                    {
                        {"include_entities", "false"},
                        {"skip_status", "true" },
                        {"include_email", "true" },
                    };

                    var response = await TwitterApiClient.Get(TwitterApiConfig.VerifyCredentialsPath, token, parameters);
                    if (!string.IsNullOrEmpty(response))
                    {
                        var profile = JsonConvert.DeserializeObject<TwitterProfile>(response);
                        if(profile != null)
                        {
                            profile.Success = true;
                            return profile;
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex);
#endif
                }

                return new TwitterProfile { Success = false };
            }
        }
    }
}
