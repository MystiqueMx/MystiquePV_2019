using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MystiqueNative.Services;
using MystiqueNative.Models;

namespace MystiqueNative.ViewModels
{
    public class WalletViewModel : BaseViewModel
    {
        public ObservableCollection<BeneficiosWallet> BeneficiosWallet { get; set; }
        public bool AgregarStatus { get => _agregarStatus; private set { _agregarStatus = value; OnPropertyChanged("AgregarStatus"); } }
        public WalletViewModel() => BeneficiosWallet = new ObservableCollection<BeneficiosWallet>();
        private bool _agregarStatus;
        public async void ObtenerBeneficiosWallet()
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Wallet.CallObtenerWallet();
            IsBusy = false;
            if (!response.Success)
            {
                ErrorMessage = response.ErrorMessage;
            }
            else
            {
                BeneficiosWallet.Clear();  
                foreach (var t in response.ListaWalletItems)
                    BeneficiosWallet.Add(t);
            }
            AgregarStatus = !response.Success;
        }
        public async void AgregarBeneficioWallet(string idSucursal, string idBeneficio)
        {
            IsBusy = true;
            var validacion = await MystiqueApiV2.Beneficios.CallObtenerDetalleBeneficio(idBeneficio, idSucursal);
            IsBusy = false;
            if (validacion.Success)
            {
                if (!validacion.Detalle.EstaEnWallet)
                {
                    var response = await MystiqueApiV2.Wallet.CallAgregarBeneficio(idSucursal, idBeneficio);
                    if (!response.Success)
                    {
                        ErrorMessage = response.ErrorMessage;
                    }
                    AgregarStatus = response.Success;
                }
                else
                {
                    AgregarStatus = validacion.Success;
                }
            }
            else
            {
                AgregarStatus = validacion.Success;
            }
            
            IsBusy = false;
            
        }
        public async void EliminarBeneficioWallet(string idBeneficio)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Wallet.CallRemoverBeneficio(idBeneficio);
            IsBusy = false;
            if (!response.Success)
            {
                ErrorMessage = response.ErrorMessage;
            }
            ErrorStatus = !response.Success;
        }
        
    }
}
