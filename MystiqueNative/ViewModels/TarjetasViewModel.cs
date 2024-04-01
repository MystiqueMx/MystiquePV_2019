using MystiqueNative.Helpers;
using MystiqueNative.Models.OpenPay;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class TarjetasViewModel : BaseViewModel
    {
        #region SINGLETON
        public static TarjetasViewModel Instance => _instance ?? (_instance = new TarjetasViewModel());
        private static TarjetasViewModel _instance;

        public static void Destroy() => _instance = null;
        #endregion
        #region CTOR

        public TarjetasViewModel()
        {
            Tarjetas = new ObservableCollection<Card>();

        }

        #endregion

        #region EVENTS

        public event EventHandler<BaseEventArgs> OnRemoverTarjetaFinished;
        public event EventHandler<BaseEventArgs> OnObtenerTarjetasFinished;
        public event EventHandler<AgregarTarjetaEventArgs> OnAgregarTarjetaFinished;

        #endregion

        #region FIELDS

        public ObservableCollection<Card> Tarjetas;

        #endregion

        #region API

        public async Task AgregarTarjeta(Card tarjeta)
        {
            IsBusy = true;
            //TODO REMOVE CONEKTA
            //HERE
            var conektaCard = new Models.Conekta.Card(tarjeta.HolderName, tarjeta.CardNumber, tarjeta.Cvv,
                tarjeta.ExpirationMonth, tarjeta.ExpirationYear);
            var token = await Services.ConektaApi.Tokens.Create(conektaCard);
            if (token != null && token.ContainsKey("id"))
            //TOHERE
            //TODO UNCOMMENT 
            //var token = Services.OpenPayApi.Tokens.ObtenerToken(tarjeta);
            //if (token.IsSuccessful)
            {
                //TODO REMOVE CONEKTA
                //HERE
                var brand = "Otro";
                if (ValidatorHelper.IsAmexCreditCard(tarjeta.CardNumber))
                {
                    brand = "American Express";
                }
                if (ValidatorHelper.IsMastercardCreditCard(tarjeta.CardNumber))
                {
                    brand = "MasterCard";
                }
                if (ValidatorHelper.IsVisaCreditCard(tarjeta.CardNumber))
                {
                    brand = "Visa";
                }

                //#if DEBUG
                //var response = await Services.QdcApi.OpenPay.RegistrarTarjetaNueva("tok_test_visa_1881","tok_test_visa_1881",tarjeta.HolderName, tarjeta.MaskedCardNumber, brand);
                //#else
                var id = token["id"].ToString();
                if (tarjeta.CardNumber == "4242424242424242")
                {
                    id = "tok_test_visa_4242";
                }
                else if (tarjeta.CardNumber == "4012888888881881")
                {
                    id = "tok_test_visa_1881";
                }
                var response = await Services.QdcApi.OpenPay.RegistrarTarjetaNueva(id, id, tarjeta.HolderName, tarjeta.MaskedCardNumber, brand);
                //#endif
                //TOHERE

                //TODO UNCOMMENT 
                //var sessionId = await ServiceLocator.Instance.Get<IOpenPaySessionId>().CreateDeviceSessionIdInternal(
                //    OpenPayApiConfig.MerchantId, OpenPayApiConfig.PublicKey, OpenPayApiConfig.UrlApi);
                //var response = await Services.QdcApi.OpenPay.RegistrarTarjetaNueva(token.Id, sessionId);

                if (response.Estatus.IsSuccessful)
                {
                    Tarjetas.Add(response.Respuesta);
                    OnAgregarTarjetaFinished?.Invoke(this, new AgregarTarjetaEventArgs
                    {
                        Success = response.Estatus.IsSuccessful,
                        Message = response.Estatus.Message,
                        EditedId = response.Respuesta.Id
                    });
                }
                else
                {
                    OnAgregarTarjetaFinished?.Invoke(this, new AgregarTarjetaEventArgs
                    {
                        Success = response.Estatus.IsSuccessful,
                        Message = response.Estatus.Message,
                    });
                }

            }
            else
            {
                OnAgregarTarjetaFinished?.Invoke(this, new AgregarTarjetaEventArgs
                {
                    Success = false,
                    Message = "Tarjeta declinada"
                });
            }
            IsBusy = false;
        }

        public async Task ObtenerTarjetas()
        {
            IsBusy = true;

            var response = await Services.QdcApi.OpenPay.ObtenerTarjetas();

            if (response.Estatus.IsSuccessful)
            {
                Tarjetas.Clear();
                if (response.Respuesta != null)
                {
                    foreach (var card in response.Respuesta)
                    {
                        Tarjetas.Add(card);
                    }
                }
            }
            OnObtenerTarjetasFinished?.Invoke(this, new BaseEventArgs { Success = response.Estatus.IsSuccessful, Message = response.Estatus.Message });

            IsBusy = false;
        }

        public async Task RemoverTarjeta(string id)
        {
            IsBusy = true;

            var response = await Services.QdcApi.OpenPay.RemoverTarjeta(id);
            if (response.Estatus.IsSuccessful)
            {
                Tarjetas.Remove(Tarjetas.First(c => c.Id == id));
            }
            OnObtenerTarjetasFinished?.Invoke(this, new BaseEventArgs { Success = response.Estatus.IsSuccessful, Message = response.Estatus.Message });

            IsBusy = false;
        }
        /// <summary>
        ///  See https://openpay.mx/docs/api/#c-digos-de-error
        /// 3001	402 Payment Required	La tarjeta fue declinada.
        /// 3002	402 Payment Required	La tarjeta ha expirado.
        /// 3003	402 Payment Required	La tarjeta no tiene fondos suficientes.
        /// 3004	402 Payment Required	La tarjeta ha sido identificada como una tarjeta robada.
        /// 3005	402 Payment Required	La tarjeta ha sido identificada como una tarjeta fraudulenta.
        /// 3006	412 Precondition Failed	La operación no esta permitida para este cliente o esta transacción.
        /// 3008	412 Precondition Failed	La tarjeta no es soportada en transacciones en linea.
        /// 3009	402 Payment Required	La tarjeta fue reportada como perdida.
        /// 3010	402 Payment Required	El banco ha restringido la tarjeta.
        /// 3011	402 Payment Required	El banco ha solicitado que la tarjeta sea retenida. Contacte al banco.
        /// 3012	412 Precondition Failed	Se requiere solicitar al banco autorización para realizar este pago.
        /// </summary>
        /// <param name="tokenErrorCode"></param>
        /// <returns></returns>
        private static string ObtenerMensajeErrorOpenPay(Error tokenErrorCode)
        {
            return $"Tarjeta declinada";
        }
        #endregion
    }
    public class AgregarTarjetaEventArgs : BaseEventArgs
    {
        public string EditedId { get; set; }
    }
}
