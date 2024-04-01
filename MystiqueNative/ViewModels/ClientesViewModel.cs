using MystiqueNative.Helpers;
using MystiqueNative.Models.Clientes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class ClientesViewModel : BaseViewModel
    {
        #region Singleton
        public static ClientesViewModel Instance => _instance ?? (_instance = new ClientesViewModel());
        private static ClientesViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion

        #region Events
        public event EventHandler<BaseEventArgs> OnFinishRegisterCliente;
        public event EventHandler<BaseEventArgs> OnFinishBuscarCliente;
        public event EventHandler<BaseListEventArgs> OnFinishObtenerClientes;
        #endregion

        #region Declaration
        public ObservableCollection<ClienteCallCenter> ClienteCallCenter { get; set; }

        public ClienteCallCenter ClienteSelected { get; set; }

        public bool clienteEncontrado { get; set; }
        #endregion

        #region Constructor
        public ClientesViewModel()
        {
            ClienteCallCenter = new ObservableCollection<ClienteCallCenter>();
        }
        #endregion

        #region Methods
        public async Task BuscarClienteCallCenter(string telefono)
        {
            #region BuscarClienteCallCenter
            var response = await Services.MystiqueApiV2.Clientes.LlamarBuscarClienteCallCenter(telefono);
            if (response.Estatus.IsSuccessful)
            {
                clienteEncontrado = response.encontrado;
                OnFinishBuscarCliente?.Invoke(this, new BaseEventArgs()
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }
            else
            {
                OnFinishBuscarCliente?.Invoke(this, new BaseEventArgs()
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }
            #endregion
        }

        public async Task BuscarListaClienteCallCenter(string telefono, string nombre)
        {
            #region BuscarClienteCallCenter
            var response = await Services.MystiqueApiV2.Clientes.LlamarBuscarListaClienteCallCenter(telefono, nombre);
            if (response.Estatus.IsSuccessful)
            {
                ClienteCallCenter.Clear();
                response.listaCliente.ForEach(c => ClienteCallCenter.Add(c));
                OnFinishObtenerClientes?.Invoke(this, new BaseListEventArgs()
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }
            else
            {
                OnFinishObtenerClientes?.Invoke(this, new BaseListEventArgs()
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }
            #endregion
        }

        public async Task ObtenerClientesCallCenter()
        {
            #region ObtenerClientes
            var response = await Services.MystiqueApiV2.Clientes.LlamarObtenerClienteCallCenter();
            if (response.Estatus.IsSuccessful)
            {
                ClienteCallCenter.Clear();
                response.listaCliente.ForEach(c => ClienteCallCenter.Add(c));
                OnFinishObtenerClientes?.Invoke(this, new BaseListEventArgs()
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }
            else
            {
                OnFinishObtenerClientes?.Invoke(this, new BaseListEventArgs()
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }
            #endregion
        }

        public async Task RegistrarClienteCallCenter(string nombre, string apPaterno, string apMaterno, string telefono)
        {
            #region GuardarPublicacionFav
            var response = await Services.MystiqueApiV2.Clientes.LlamarGuardarClienteCallCenter(nombre, apPaterno, apMaterno, telefono);
            if (response.Estatus.IsSuccessful)
            {
                OnFinishRegisterCliente?.Invoke(this, new BaseEventArgs()
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }
            else
            {
                OnFinishRegisterCliente?.Invoke(this, new BaseEventArgs()
                {
                    Success = response.Estatus.IsSuccessful,
                    Message = response.Estatus.Message
                });
            }
            #endregion
        }
        #endregion
    }
}
