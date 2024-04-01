namespace MystiqueNative.Configuration
{
    public static class MystiqueApiV2Config
    {
        #region CONFIG
#pragma warning disable S125 // Sections of code should not be "commented out"
#if DEBUG

        //LOCAL
        //private const string UrlApi = "http://localhost/MystFrescoApi/";
        //PRODUCTIVO:
        //private const string UrlApi = "http://k8.grupored.mx/ServiciosMystiqueFidelizacion";
        //QA
        private const string UrlApi = "http://k9.grupored.mx/Mystique_MovilServicios_QA";

#else
        //private const string UrlApi = "http://k7.grupored.mx/ServiciosMystiqueMcQas";
        //private const string UrlApi = "http://k7.grupored.mx/ServiciosMystiqueMcPRD";
        //private const string UrlApi = "http://k7.grupored.mx/ServiciosMystiquePvPRD";
        //private const string UrlApi = "http://k7.grupored.mx/ServiciosFidelizacionApi";
        //private const string UrlApi = "http://k7.grupored.mx/MystiquePv_PrePrd_Movil";

        //PRODUCTIVO:
        //private const string UrlApi = "http://k8.grupored.mx/ServiciosMystiqueFidelizacion";
        private const string UrlApi = "http://k9.grupored.mx/Mystique_MovilServicios_QA";
#endif
        // Header: 
        internal const string MystiqueAppSecret = @"yfHa2cpplZZy105Yd41FYcjgb27j7uvMo069IiL1yw|cgr@ySl";

        public const int MystiqueAppEmpresa = 1;
#pragma warning restore S125 // Sections of code should not be "commented out"
        #endregion
        #region ENDPOINTS
        // Flujo Principal
        private const string ObtenerSucursalesIdcomercioRoute = @"api/obtenerListaSucursalesPorComercio";
        private const string ObtenerBeneficiosIdsucursalRoute = @"api/obtenerBeneficiosPorSucursal";
        private const string ObtenerDetalleBeneficioRoute = @"api/obtenerBeneficioDetalle";
        private const string ObtenerComerciosIdgiroRoute = @"api/obtenerListadoComerciosPorGiro";
        private const string InsertarCalificacionRoute = @"api/insertarCalificacionPorBeneficio";

        // Account
        private const string IniciarSesionRoute = @"api/iniciarSesion";
        private const string LoginRoute = @"api/autentificarse";
        private const string FbLoginRoute = @"api/autentificarseFB";
        private const string LogoutRoute = @"api/logout";
        private const string RecoverPasswordRoute = @"api/recuperarPassword";
        private const string ValidateRecoverPasswordRoute = @"api/validarRecuperarContrasenia";
        private const string RegisterRoute = @"api/registrarUsuario";
        private const string UpdateProfileRoute = @"api/actualizarUsuario";

        // Wallet
        private const string AgregarBeneficioWalletRoute = @"api/insertarWallet";
        private const string RemoverBeneficioWalletRoute = @"api/eliminarWallet";
        private const string ObtenerWalletRoute = @"api/obtenerWalletPorCliente";
        private const string SetZonacitysaladRoute = @"api/avisoZonaCitySalads";

        // FlujoConfiguracion

        private const string EnviarComentarioRoute = @"api/insertarComentario";
        private const string ObtenerConfiguracionRoute = @"api/obtenerConfiguracion";

        // Citypoints
        private const string RegistrarCitypointsRoute = @"api/registrarCompraParaPuntos";
        private const string CanjearCitypointsRoute = @"api/registrarCanjePuntos";
        private const string ObtenerCitypointsRoute = @"api/obtenerEstadoCuentaPuntos";
        private const string ObtenerRecompensasRoute = @"api/obtenerListadoRecompensas";
        private const string ObtenerMovimientosPuntosRoute = @"api/obtenerListadoMovimientoPuntosV2";
        private const string ValidateCapturaCitypointsRoute = @"api/validarRegistroUsuarioCanjePuntos";
        private const string ObtenerCuponesActivosRoute = @"api/obtenerListaDeCuponesActivos";
        private const string EliminarRecompensaRoute = @"api/eliminarRecompensa";

        // Imagen perfil
        private const string DownloadPictureRoute = @"Files/ProfilePictures";
        private const string UploadPictureRoute = @"Files/UploadProfilePicture";

        // Colonias
        private const string ObtenerColoniasCiudadidRoute = @"api/obtenerColonias";

        // Notificaciones
        private const string ObtenerNotificacionesRoute = @"api/obtenerNotificacionesCliente";
        private const string LimpiarNotificacionesRoute = @"api/actualizarNotificacionesCliente";

        //Facturacion 
        private const string ObtenerFacturasRoute = @"api/obtenerMisFacturas";
        private const string ValidarTicketRoute = @"api/validarTicket";
        private const string SolicitarFacturaRoute = @"api/solicitarFactura";
        private const string RemoverDatosFiscales = @"api/Factura/removerDatosFiscales";
        private const string ReenviarFactura = @"api/Factura/solicitarReenvioFactura";

        //Clientes Call Center
        private const string BuscarClienteCallCenterRoute = @"api/hazPedido/BuscarClienteCallCenter";
        private const string BuscarListaClienteCallCenterRoute = @"api/hazPedido/BuscarListaClienteCallCenter";
        private const string ObtenerClienteCallCenterRoute = @"api/hazPedido/ObtenerClientesCallCenter";        
        private const string GuardarClienteCallCenterRoute = @"api/hazPedido/GuardarClienteCallCenter";

        /** HAZ TU PEDIDO / QDC **/

        // MenuRestaurantes
        private const string ObtenerRestaurantesRoute = @"api/hazPedido/ObtenerSucursalesPoligono";
        private const string ObtenerMenuRestauranteRoute = @"api/hazPedido/ObtenerMenuSucursal";
        private const string ObtenerConfiguracionEnsaladasRoute = @"api/hazPedido/ObtenerPlatillosEnsaladas";
        private const string ObtenerPlatillosMenuRoute = @"api/hazPedido/ObtenerPlatillosMenu";

        //Directorio
        private const string ObtenerDirectorioRoute = @"api/hazPedido/ObtenerSucursalesActivas";

        //Tarjetas Coneckta
        private const string RegistarTarjetaRoute = @"api/hazPedido/RegistrarTarjetaCK";
        private const string ObtenerTarjetasRoute = @"api/hazPedido/ListadoTarjetasCK";
        private const string RemoverTarjetasRoute = @"api/hazPedido/EliminarTarjetaCK";

        //Direcciones 
        private const string ObtenerColoniasRoute = @"api/hazPedido/ObtenerColonias";
        private const string ObtenerDireccionesRoute = @"api/hazPedido/ObtenerDirecciones";
        private const string AgregarDireccionRoute = @"api/hazPedido/AgregarDireccion";
        private const string EditarDireccionRoute = @"api/hazPedido/EditarDireccion";

        //Pedidos 
        private const string RegistrarPedidoRoute = @"api/hazPedido/RegistrarPedido";
        private const string ObtenerPedidosActivosRoute = @"api/hazPedido/ObtenerPedidosActivos";
        private const string RegistrarMensajePedidoRoute = @"api/hazPedido/RegistrarMensajePedido";
        private const string ObtenerDetallePedidoRoute = @"api/hazPedido/ObtenerInformacionPedido";
        private const string ObtenerHistorialPedidosRoute = @"api/hazPedido/ObtenerHistorialPedidos";
        private const string CalificarPedidoRoute = @"api/hazPedido/CalificarPedido";

        //Notificaciones 
        private const string ObtenerNotificacionesHPRoute = @"api/hazPedido/ObtenerNotificacionesConsumidor";
        private const string ActualizarNotificacionesHPRoute = @"api/hazPedido/ActualizarNotificacionesConsumidor";
        /** FIN HAZ TU PEDIDO / QDC **/

        #endregion
        #region API
        internal static string FbLoginPath => $"{UrlApi}/{FbLoginRoute}";
        internal static string LoginPath => $"{UrlApi}/{LoginRoute}";
        internal static string IniciarSesionPath => $"{UrlApi}/{IniciarSesionRoute}";
        internal static string LogoutPath => $"{UrlApi}/{LogoutRoute}";
        internal static string RecoverPasswordPath => $"{UrlApi}/{RecoverPasswordRoute}";
        internal static string ValidateRecoverPasswordPath => $"{UrlApi}/{ValidateRecoverPasswordRoute}";
        internal static string RegisterPath => $"{UrlApi}/{RegisterRoute}";
        internal static string UpdateProfilePath => $"{UrlApi}/{UpdateProfileRoute}";

        internal static string InsertarCalificacionPath => $"{UrlApi}/{InsertarCalificacionRoute}";

        internal static string ObtenerSucursalesIdcomercioPath => $"{UrlApi}/{ObtenerSucursalesIdcomercioRoute}";

        internal static string ObtenerBeneficiosIdsucursalPath => $"{UrlApi}/{ObtenerBeneficiosIdsucursalRoute}";
        internal static string ObtenerDetalleBeneficioPath => $"{UrlApi}/{ObtenerDetalleBeneficioRoute}";
        internal static string ObtenerComerciosIdgiroPath => $"{UrlApi}/{ObtenerComerciosIdgiroRoute}";

        internal static string AgregarBeneficioWalletPath => $"{UrlApi}/{AgregarBeneficioWalletRoute}";
        internal static string RemoverBeneficioWalletPath => $"{UrlApi}/{RemoverBeneficioWalletRoute}";
        internal static string ObtenerWalletPath => $"{UrlApi}/{ObtenerWalletRoute}";
        internal static string SetZonacitysaladsPath => $"{UrlApi}/{SetZonacitysaladRoute}";

        internal static string ObtenerNotificacionesPath => $"{UrlApi}/{ObtenerNotificacionesRoute}";
        internal static string EnviarComentarioPath => $"{UrlApi}/{EnviarComentarioRoute}";
        internal static string ObtenerConfiguracionPath => $"{UrlApi}/{ObtenerConfiguracionRoute}";

        internal static string RegistrarCitypointsPath => $"{UrlApi}/{RegistrarCitypointsRoute}";
        internal static string CanjearCitypointsPath => $"{UrlApi}/{CanjearCitypointsRoute}";
        internal static string ObtenerCitypointsPath => $"{UrlApi}/{ObtenerCitypointsRoute}";
        internal static string ObtenerRecompensasPath => $"{UrlApi}/{ObtenerRecompensasRoute}";
        internal static string ObtenerMovimientosPuntosPath => $"{UrlApi}/{ObtenerMovimientosPuntosRoute}";
        internal static string UploadPicturePath => $"{UrlApi}/{UploadPictureRoute}";

        internal static string DownloadPicturePath => $"{UrlApi}/{DownloadPictureRoute}";

        internal static string ValidateCapturaCitypointsPath => $"{UrlApi}/{ValidateCapturaCitypointsRoute}";
        internal static string ObtenerCuponesActivosPath => $"{UrlApi}/{ObtenerCuponesActivosRoute}";
        internal static string EliminarRecompensaPath => $"{UrlApi}/{EliminarRecompensaRoute}";

        internal static string ObtenerColoniasCiudadidPath => $"{UrlApi}/{ObtenerColoniasCiudadidRoute}";
        internal static string LimpiarNotificacionesPath => $"{UrlApi}/{LimpiarNotificacionesRoute}";

        internal static string ObtenerFacturasPath => $"{UrlApi}/{ObtenerFacturasRoute}";
        internal static string ValidarTicketPath => $"{UrlApi}/{ValidarTicketRoute}";
        internal static string SolicitarFacturaPath => $"{UrlApi}/{SolicitarFacturaRoute}";
        internal static string RemoverDatosFiscalesPath => $"{UrlApi}/{RemoverDatosFiscales}";
        internal static string ReenviarFacturaPath => $"{UrlApi}/{ReenviarFactura}";

        internal static string BuscarClienteCallCenterPath => $"{UrlApi}/{BuscarClienteCallCenterRoute}";
        internal static string BuscarListaClienteCallCenterPath => $"{UrlApi}/{BuscarListaClienteCallCenterRoute}";
        internal static string ObtenerClienteCallCenterPath => $"{UrlApi}/{ObtenerClienteCallCenterRoute}";
        internal static string GuardarClienteCallCenterPath => $"{UrlApi}/{GuardarClienteCallCenterRoute}";

        #region HAZ TU PEDIDO / QDC

        #region MENU RESTAURANTES
        internal static string ObtenerRestaurantesPath => $"{UrlApi}/{ObtenerRestaurantesRoute}";
        internal static string ObtenerMenuRestaurantePath => $"{UrlApi}/{ObtenerMenuRestauranteRoute}";
        internal static string ObtenerConfiguracionEnsaladasPath => $"{UrlApi}/{ObtenerConfiguracionEnsaladasRoute}";
        internal static string ObtenerPlatillosMenuPath => $"{UrlApi}/{ObtenerPlatillosMenuRoute}";
        internal static string ObtenerDirectorioPath => $"{UrlApi}/{ObtenerDirectorioRoute}";
        #endregion

        #region TARJETAS CONEKTA
        internal static string RegistarTarjetaPath => $"{UrlApi}/{RegistarTarjetaRoute}";
        internal static string ObtenerTarjetasPath => $"{UrlApi}/{ObtenerTarjetasRoute}";
        internal static string RemoverTarjetasPath => $"{UrlApi}/{RemoverTarjetasRoute}";
        #endregion

        #region DIRECCIONES
        internal static string ObtenerColoniasPath => $"{UrlApi}/{ObtenerColoniasRoute}";
        internal static string ObtenerDireccionesPath => $"{UrlApi}/{ObtenerDireccionesRoute}";
        internal static string AgregarDireccionPath => $"{UrlApi}/{AgregarDireccionRoute}";
        internal static string EditarDireccionPath => $"{UrlApi}/{EditarDireccionRoute}";
        #endregion

        #region PEDIDOS
        internal static string RegistrarPedidoPath => $"{UrlApi}/{RegistrarPedidoRoute}";
        internal static string ObtenerPedidosActivosPath => $"{UrlApi}/{ObtenerPedidosActivosRoute}";
        internal static string RegistrarMensajePedidoPath => $"{UrlApi}/{RegistrarMensajePedidoRoute}";
        internal static string ObtenerDetallePedidoPath => $"{UrlApi}/{ObtenerDetallePedidoRoute}";
        internal static string ObtenerHistorialPedidosPath => $"{UrlApi}/{ObtenerHistorialPedidosRoute}";
        internal static string CalificarPedidoPath => $"{UrlApi}/{CalificarPedidoRoute}";
        #endregion

        #region NOTIFICACIONES
        internal static string ObtenerNotificacionesHPPath => $"{UrlApi}/{ObtenerNotificacionesHPRoute}";
        internal static string ActualizarNotificacionesHPPath => $"{UrlApi}/{ActualizarNotificacionesHPRoute}";
        #endregion

        #endregion

        #endregion
    }
}
