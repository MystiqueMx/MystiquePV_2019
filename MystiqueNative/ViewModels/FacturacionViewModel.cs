using MystiqueNative.Helpers;
using MystiqueNative.Models;
using MystiqueNative.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MystiqueNative.ViewModels
{
    public class FacturacionViewModel : BaseViewModel
    {
        public FacturacionViewModel()
        {
            Facturas = new ObservableCollection<Factura>();
            ReceptoresGuardados = new ObservableCollection<ReceptorFactura>();
            UsosCfdi = new ObservableCollection<UsoCfdi>();
        }
        #region Singleton
        public static FacturacionViewModel Instance => _instance ?? (_instance = new FacturacionViewModel());
        private static FacturacionViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        #region Events
        public event EventHandler<ValidarTicketEventArgs> OnValidarTicketFinished;
        public event EventHandler<BaseEventArgs> OnObtenerFacturasFinished;
        public event EventHandler<BaseEventArgs> OnSolicitarFacturaFinished;
        public event EventHandler<BaseEventArgs> OnRemoverDatosFiscalesFinished;
        public event EventHandler<BaseEventArgs> OnReenviarFacturaFinished;
        #endregion
        #region Fields
        public Ticket TicketEscaneado { get; private set; }
        public ObservableCollection<Factura> Facturas { get; private set; }
        public ObservableCollection<ReceptorFactura> ReceptoresGuardados { get; private set; }
        public ObservableCollection<UsoCfdi> UsosCfdi { get; private set; }
        public string[] UsosCfdiAsStringArray()
        {
            return UsosCfdi
                    .Select(c => c.Descripcion)
                    .ToArray();
        }
        public List<string> UsosCfdiAsStringList()
        {
            return UsosCfdi
                .Select(c => c.Descripcion)
                .ToList();
        }
        #endregion
        public async void ObtenerFacturas()
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Facturacion.CallObtenerFacturas();
            if (!response.Success)
            {
                OnObtenerFacturasFinished?.Invoke(this, new BaseEventArgs { Success = false, Message = response.ErrorMessage });
            }
            else
            {
                Facturas.Clear();
                response.Facturas
                    .ForEach(c => Facturas.Add(c));

                OnObtenerFacturasFinished?.Invoke(this, new BaseEventArgs { Success = true });
            }
            IsBusy = false;
        }
        public async void ValidarTicket(string codigoQr)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Facturacion.CallValidarTicket(codigoQr);
            if (response.Success)
            {
                ReceptoresGuardados.Clear();
                UsosCfdi.Clear();
                response.ReceptoresGuardados.ForEach(c => ReceptoresGuardados.Add(c));
                response.CatalogoUsos.ForEach(c => UsosCfdi.Add(c));

                TicketEscaneado = new Ticket
                {
                    FechaCompra = response.FechaCompra,
                    MontoCompra = response.MontoCompra,
                    Id = response.Id,
                    Sucursal = response.Sucursal,
                    SucursalId = response.SucursalId,
                    PendienteTicket = response.PendienteTicket
                };

                OnValidarTicketFinished?.Invoke(this, new ValidarTicketEventArgs
                {
                    Success = true,
                    UsosCfdi = response.CatalogoUsos,
                    ReceptoresGuardados = response.ReceptoresGuardados,
                    TicketEscaneado = TicketEscaneado
                });
            }
            else
            {
                OnValidarTicketFinished?.Invoke(this, new ValidarTicketEventArgs { Success = true, Message = response.ErrorMessage });
            }
            
            IsBusy = false;
            ErrorMessage = response.ErrorMessage;
            ErrorStatus = !response.Success;
        }
        public async void SolicitarFactura(ReceptorFactura receptorFactura)
        {
            IsBusy = true;
            var response = await MystiqueApiV2.Facturacion.CallSolicitarFactura(TicketEscaneado.Id, TicketEscaneado.PendienteTicket, TicketEscaneado.SucursalId, receptorFactura);
            if (response.Success)
            {
                var facturasContainer = await MystiqueApiV2.Facturacion.CallObtenerFacturas();
                if (facturasContainer.Success)
                {
                    Facturas.Clear();
                    facturasContainer.Facturas
                        .ForEach(c => Facturas.Add(c));
                }
                TicketEscaneado = null;
            }
            OnSolicitarFacturaFinished?.Invoke(this, new BaseEventArgs { Success = response.Success, Message = response.ErrorMessage });
            IsBusy = false;
        }
        public void AgregarCfdiVacio()
        {
            ReceptoresGuardados.Add(new ReceptorFactura{ Id = "0" });
        }

        public async void RemoverReceptor(int index)
        {
            IsBusy = true;
            var receptor = ReceptoresGuardados[index];
            var response = await MystiqueApiV2.Facturacion.CallRemoverDatosFiscales(receptor.Id);
            if (response.Success)
            {
                ReceptoresGuardados.RemoveAt(index);
            }
            else
            {
                OnRemoverDatosFiscalesFinished?.Invoke(this, new BaseEventArgs { Success = response.Success, Message = response.ErrorMessage });
            }
            IsBusy = false;
        }
        public async void ReenviarFactura(int index, string email)
        {
            IsBusy = true;
            var factura = Facturas[index];
            var response = await MystiqueApiV2.Facturacion.CallReenviarFactura(factura.Id, email);
            OnReenviarFacturaFinished?.Invoke(this, new BaseEventArgs { Success = response.Success, Message = response.ErrorMessage });
            IsBusy = false;
        }
    }
}
