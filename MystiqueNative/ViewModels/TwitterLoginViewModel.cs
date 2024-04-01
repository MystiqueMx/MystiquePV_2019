using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MystiqueNative.Services;
using MystiqueNative.Models;
using System.Linq;
using MystiqueNative.Models.Twitter;
using MystiqueNative.Configuration;

namespace MystiqueNative.ViewModels
{
    public class TwitterLoginViewModel : BaseViewModel
    {
        #region Singleton
        public static TwitterLoginViewModel Instance => _instance ?? (_instance = new TwitterLoginViewModel());
        private static TwitterLoginViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        #region EVENTS
        public event EventHandler<ObtenerUrlOAuthEventArgs> OnObtenerUrlOAuthFinished;
        public event EventHandler<SolicitarUsuarioTwitterEventArgs> OnSolicitarUsuarioTwitterFinished;
        #endregion
        #region FIELDS
        public RequestToken Token { get; private set; }
        #endregion
        #region API
        public async void ObtenerUrlOAuth(string callbackUrl = null)
        {
            IsBusy = true;
            var response = await TwitterApi.OAuth.SolicitarRequestToken();
            IsBusy = false;
            if (response.OauthCallbackConfirmed)
            {
                OnObtenerUrlOAuthFinished?.Invoke(this, new ObtenerUrlOAuthEventArgs
                {
                    Success = response.OauthCallbackConfirmed,
                    Token = response,
                    Url = TwitterApiConfig.AuthenticatePath + "?oauth_token=" + response.OauthToken
                });
            }
            else
            {
                OnObtenerUrlOAuthFinished?.Invoke(this, new ObtenerUrlOAuthEventArgs
                {
                    Success = response.OauthCallbackConfirmed,
                    Token = response
                });
            }
        }
        public async void SolicitarUsuarioTwitter(string oauthToken, string oauthVerifier)
        {
            IsBusy = true;
            var token = await TwitterApi.OAuth.SolicitarAccessToken(oauthToken, oauthVerifier);
            
            IsBusy = false;
            if (token.OauthCallbackConfirmed)
            {
                var user = await TwitterApi.Account.SolicitarVerificarCredenciales(token);
                if (user.Success)
                {
                    OnSolicitarUsuarioTwitterFinished?.Invoke(this, new SolicitarUsuarioTwitterEventArgs
                    {
                        Success = true,
                        Token = token,
                        User = user
                    });
                }
                else
                {
                    OnSolicitarUsuarioTwitterFinished?.Invoke(this, new SolicitarUsuarioTwitterEventArgs
                    {
                        Success = false,
                        Token = token
                    });
                }
            }
            else
            {
                OnSolicitarUsuarioTwitterFinished?.Invoke(this, new SolicitarUsuarioTwitterEventArgs
                {
                    Success = false,
                    Token = token
                });
            }
        }
        #endregion
    }
    #region EventArgs

    #endregion
}
