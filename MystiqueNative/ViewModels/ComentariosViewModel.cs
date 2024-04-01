using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MystiqueNative.Services;
using MystiqueNative.Models;

namespace MystiqueNative.ViewModels
{
    public class ComentariosViewModel : BaseViewModel
    {
        #region Singleton
        public static ComentariosViewModel Instance => _instance ?? (_instance = new ComentariosViewModel());
        private static ComentariosViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        public event EventHandler<EnviarComentarioArgs> OnEnviarComentarioFinished;
        public async void EnviarComentario(string tipoComentario, string mensaje)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Configuracion.CallEnviarComentario(mensaje,TipoComentariosHelper.DescripcionToId[tipoComentario].ToString());
            OnEnviarComentarioFinished?.Invoke(this, new EnviarComentarioArgs { Success = response.Success, Message = response.ErrorMessage });
            IsBusy = false;
            ErrorMessage = response.ErrorMessage;
            ErrorStatus = !response.Success;
        }
    }
    #region EventArgs

    #endregion
}


