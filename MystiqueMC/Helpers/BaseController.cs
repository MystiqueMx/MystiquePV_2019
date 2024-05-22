									   
										  
																			  
											 
																					   

			  
					 
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
							  
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Models;

namespace MystiqueMC.Helpers
{
    public abstract class BaseController : Controller
   
										
																									  

									  
    {
        #region CONTEXT
        private MystiqueMeEntities _context;
        public MystiqueMeEntities Contexto
        {
            get
            {
                if (_context != null) return _context;
	   
	 

																	   

                 _context = new MystiqueMeEntities();
                
                //var objCx = (new MystiqueMeEntities() as IObjectContextAdapter).ObjectContext;
                //objCx.SavingChanges += (s, e) => { Logger.Debug("override"); };
																																			 
	   
	 

                _context.Database.Log = sql => Logger.Debug(sql);   
                                                          
                return _context;
            }           

        }
        #endregion
        #region MYSTIQUE
        public int EmpresaUsuario => Session.EmpresaUsuarioLogueado();
        public empresas Empresa => Contexto.empresas.FirstOrDefault(f => f.idEmpresa == EmpresaUsuario);
        public string RolUsuario => Session.ObtenerRol();
        public IQueryable<comercios> ComerciosFirmados 
        {
            get
            {
                int IdUsuario;
                switch (RolUsuario)
                {
                    case "WebMaster":
                        return Contexto.comercios.AsQueryable();
                    case "Empresa":
                        return Contexto.comercios.Where(c => c.empresaId == EmpresaUsuario).AsQueryable();
                    case "Comercio":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.comercios.Where(c => c.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    case "Analista":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.comercios.Where(c => c.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    default:
                        return Enumerable.Empty<comercios>().AsQueryable();
                }
            }
        }
        
	 

        public IQueryable<sucursales> SucursalesFirmadas
	 
		 
	   
					  
								
        {
            get
            {
                int IdUsuario;
                switch (RolUsuario)
                {
                    case "WebMaster":
                        return Contexto.sucursales.AsQueryable();
                    case "Empresa":
                        return Contexto.sucursales.Where(c => c.comercios.empresaId == EmpresaUsuario).AsQueryable();
                    case "Comercio":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.sucursales.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    case "Analista":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.sucursales.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    default:
                        return Enumerable.Empty<sucursales>().AsQueryable();
                }
            }
        }
	   
	 

        public IQueryable<catProductos> ProductosVisibles
	 
		 
	   
					  
								
        {
            get
            {
                int IdUsuario;
                switch (RolUsuario)
                {
                    case "WebMaster":
                        return Contexto.catProductos.AsQueryable();
                    case "Empresa":
                        return Contexto.catProductos.Where(c => c.comercios.empresaId == EmpresaUsuario).AsQueryable();
                    case "Comercio":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.catProductos.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    case "Analista":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.catProductos.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    default:
                        return Enumerable.Empty<catProductos>().AsQueryable();
                }
            }
        }
	   
	 

        public IQueryable<Productos> ProductosVisibles2 // Idk es media noche y no me interesa porque hay dos tablas de productos
	 
		 
	   
					  
								
        {
            get
            {
                int IdUsuario;
                switch (RolUsuario)
                {
                    case "WebMaster":
                        return Contexto.Productos.AsQueryable();
                    case "Empresa":
                        return Contexto.Productos.Where(c => c.comercios.empresaId == EmpresaUsuario).AsQueryable();
                    case "Comercio":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.Productos.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    case "Analista":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.Productos.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    default:
                        return Enumerable.Empty<Productos>().AsQueryable();
                }
            }
        }
	   
	 

        public IQueryable<Insumos> InsumosVisibles
	 
		 
	   
					  
								
        {
            get
            {
                int IdUsuario;
                switch (RolUsuario)
                {
                    case "WebMaster":
                        return Contexto.Insumos.AsQueryable();
                    case "Empresa":
                        return Contexto.Insumos.Where(c => c.comercios.empresaId == EmpresaUsuario).AsQueryable();
                    case "Comercio":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.Insumos.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    case "Analista":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.Insumos.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    default:
                        return Enumerable.Empty<Insumos>().AsQueryable();
                }
            }
        }
	   
	 

        public IQueryable<recompensas> RecompensasVisibles
	 
		 
	   
					  
								
        {
            get
            {
                int IdUsuario;
                switch (RolUsuario)
                {
                    case "WebMaster":
                        return Contexto.recompensas.AsQueryable();
                    case "Empresa":
                        return Contexto.recompensas.Where(c => c.catProductos.comercios.empresaId == EmpresaUsuario).AsQueryable();
                    case "Comercio":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.recompensas.Where(c => c.catProductos.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    case "Analista":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.recompensas.Where(c => c.catProductos.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    default:
                        return Enumerable.Empty<recompensas>().AsQueryable();
                }
            }
        }
	   
	 

        public IQueryable<beneficios> PromocionesVisibles
	 
		 
	   
					  
								
        {
            get
            {
                int IdUsuario;
                switch (RolUsuario)
                {
                    case "WebMaster":
                        return Contexto.beneficios.AsQueryable();
                    case "Empresa":
                        return Contexto.beneficios.Where(c => c.comercios.empresaId == EmpresaUsuario).AsQueryable();
                    case "Comercio":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.beneficios.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    case "Analista":
                        IdUsuario = Session.IdUsuarioLogueado();
                        return Contexto.beneficios.Where(c => c.comercios.confUsuarioComercio.Any(d => d.usuarioId == IdUsuario)).AsQueryable();
                    default:
                        return Enumerable.Empty<beneficios>().AsQueryable();
                }
            }
        }
	   
	 

        public IQueryable<AspNetRoles> RolesAsignables
	 
		 
	   
								
        {
            get
            {
                switch (RolUsuario)
                {
                    case "WebMaster":// Rol webMaster puede crear usuarios con el rol de Empresa y comercio
                        return Contexto.AspNetRoles.AsQueryable();
                    case "Empresa":// Rol de empresa puede crear usuarios con el rol de Comercio
                        return Contexto.AspNetRoles.Where(c => c.Name == "Comercio").AsQueryable();
                    case "Comercio": // Rol de comercio puede crear usuarios con el rol de Comercio o WebService
                        return Contexto.AspNetRoles.Where(c => c.Name == "Comercio" || c.Name == "WebService").AsQueryable();
                    case "Analista":
                        return Enumerable.Empty<AspNetRoles>().AsQueryable();
                    default:
                        return Enumerable.Empty<AspNetRoles>().AsQueryable();
                }
            }
        }
	   
	 

        public IQueryable<empresas> EmpresasAsignables
	 
		 
	   
								
        {
            get
            {
                switch (RolUsuario)
                {
                    case "WebMaster":// Rol webMaster puede crear usuarios para todas las empresas
                        return Contexto.empresas.AsQueryable();
                    case "Empresa":// Rol de empresa puede crear usuarios solo de su empresa
                        return Contexto.empresas.Where(c => c.idEmpresa == UsuarioActual.empresaId).AsQueryable();
                    case "Comercio": // Rol de comercio puede crear usuarios solo de su empresa
                        return Contexto.empresas.Where(c => c.idEmpresa == UsuarioActual.empresaId).AsQueryable();
                    case "Analista":
                        return Enumerable.Empty<empresas>().AsQueryable();
                    default:
                        return Enumerable.Empty<empresas>().AsQueryable();
                }
            }
        }
	   
	 

        public IQueryable<clientes> ClientesVisibles
	 
		 
	   
								
        {
            get
            {
                switch (RolUsuario)
                {
                    case "WebMaster":
                        return Contexto.clientes.AsQueryable();
                    case "Empresa":
                        return Contexto.clientes.Where(c => c.empresaId == EmpresaUsuario).AsQueryable();
                    case "Comercio":
                        return Contexto.clientes.Where(c => c.empresaId == EmpresaUsuario).AsQueryable();
                    case "Analista":
                        return Contexto.clientes.Where(c => c.empresaId == EmpresaUsuario).AsQueryable();
                    default:
                        return Enumerable.Empty<clientes>().AsQueryable();
                }
            }
            
        }
		
		
		
        public IQueryable<catTipoMembresias> MembresiasVisibles
	 

																   

							  
	 
																								  
	 

																

													
	 
		 
	   
									  
        {
            get
            {
                switch (RolUsuario)
                {
                    case "WebMaster":
                        return Contexto.catTipoMembresias.AsQueryable();
                    case "Empresa":
                        return Contexto.catTipoMembresias.Where(c => c.empresaId == EmpresaUsuario).AsQueryable();
																																												   
                    case "Comercio":
                        return Contexto.catTipoMembresias.Where(c => c.empresaId == EmpresaUsuario).AsQueryable();
                    case "Analista":
                        return Contexto.catTipoMembresias.Where(c => c.empresaId == EmpresaUsuario).AsQueryable();
																
																																											
                    default:
                        return Enumerable.Empty<catTipoMembresias>().AsQueryable();
                }
            }
        }
        #endregion
        #region FACADES
        public MystiqueMC.DAL.usuarios UsuarioActual => Session.ObtenerUsuario();
        public int IdUsuarioActual => Session.ObtenerUsuario() == null ? -1 : Session.ObtenerUsuario().idUsuario;
        public string RolUsuarioActual => Session.ObtenerRol();
        public IQueryable<sucursales> SucursalesActuales























































									   




						  
        {
            get
            {
                switch (RolUsuarioActual)
                {
                    case "Operador Plataforma":
                        return Contexto.sucursales.AsQueryable();
                    case "Administrador Empresa":
                        var empresas = Session.ObtenerEmpresas();
                        return Contexto.sucursales.Where(c => empresas.Contains(c.comercios.empresaId)).AsQueryable();
                    case "Administrador Comercio":
                        var comercios = Session.ObtenerComercios();
                        return Contexto.sucursales.Where(c => comercios.Contains(c.comercioId)).AsQueryable();
                    case "Administrador Sucursal":
                        var sucursales = Session.ObtenerSucursales();
                        return Contexto.sucursales.Where(c => sucursales.Contains(c.idSucursal)).AsQueryable();
                    default:
                        return Enumerable.Empty<sucursales>().AsQueryable();
                }
            }
        }

        public IQueryable<sucursales> ComercioSucursales
        {
            get
            {
                // TODO definir las acciones de los roles, y las relaciones de los usuarios com empresas y comercios
                switch (RolUsuarioActual)
                {
                    case "AdmEmpresa":
                        var empresas = Session.ObtenerEmpresas();
                        return Contexto.sucursales.Where(c => empresas.Contains(c.comercios.empresaId)).AsQueryable();
                    case "Comercio":
                        return Contexto.sucursales.Where(c => c.comercios.empresaId == UsuarioActual.empresaId).AsQueryable();
                    default:
                        return Enumerable.Empty<sucursales>().AsQueryable();
											   
																												   
                }
            }
        }

        //public IQueryable<comercios> ComerciosActuales
        //{
        //    get
        //    {
        //        switch (RolUsuarioActual)
        //        {
        //            case "Operador Plataforma":
        //                return Contexto.comercios.AsQueryable();
        //            case "Administrador Empresa":
        //                var empresas = Session.ObtenerEmpresas();
        //                return Contexto.comercios.Where(c => empresas.Contains(c.empresaId)).AsQueryable();
        //            case "Administrador Comercio":
        //                var comercios = Session.ObtenerComercios();
        //                return Contexto.comercios.Where(c => comercios.Contains(c.idComercio)).AsQueryable();
        //            case "Administrador Sucursal":
        //            default:
        //                return Enumerable.Empty<comercios>().AsQueryable();
        //        }
        //    }
        //}
        //public IQueryable<Empresas> EmpresasActuales
        //{
        //    get
        //    {
        //        switch (RolUsuarioActual)
        //        {
        //            case "Operador Plataforma":
        //                return Contexto.Empresas.AsQueryable();
        //            case "Operador Empresa":
        //            case "Administrador Empresa":
        //                var empresas = Session.ObtenerEmpresas();
        //                return Contexto.Empresas.Where(c => empresas.Contains(c.idEmpresa)).AsQueryable();
        //            default:
        //                return Enumerable.Empty<Empresas>().AsQueryable();
        //        }
        //    }
        //}
        //public IQueryable<Usuarios> UsuariosVisibles
        //{
        //    get
        //    {
        //        switch (RolUsuarioActual)
        //        {
        //            case "Operador Plataforma":
        //                var rolesVisiblesOp = new[] { "Operador Plataforma", "Operador Empresa", "Administrador Empresa", "Administrador Empresa" };
        //                return Contexto.Usuarios.Where(c => rolesVisiblesOp.Contains(c.rol)).AsQueryable();
        //            case "Operador Empresa":
        //                var empresas = Session.ObtenerEmpresas();
        //                var rolesVisiblesOe = new[] { "Administrador Empresa", "Operador Empresa" };
        //                return Contexto.Usuarios.Where(c => rolesVisiblesOe.Contains(c.rol) && c.ConfUsuarioEmpresas.All(d => empresas.Contains(d.empresaId))).AsQueryable();
        //            case "Administrador Empresa":
        //                var empresasVisiblesAe = Session.ObtenerEmpresas();
        //                var rolesVisiblesAe = new[] { "Administrador Comercio" };
        //                return Contexto.Usuarios.Where(c => rolesVisiblesAe.Contains(c.rol) && c.ConfUsuarioComercio.Any(d => empresasVisiblesAe.Contains(d.Comercios.empresaId))).AsQueryable();
        //            case "Administrador Comercio":
        //                var comercios = Session.ObtenerComercios();
        //                var rolesVisiblesAs = new[] { "Administrador Sucursal" };
        //                return Contexto.Usuarios.Where(c => rolesVisiblesAs.Contains(c.rol) && c.ConfUsuarioSucursales.Any(d => comercios.Contains(d.Sucursales.comercioId))).AsQueryable();
        //            case "Administrador Sucursal":
        //            default:
        //                return Enumerable.Empty<Usuarios>().AsQueryable();
        //        }
        //    }
        //}
        //public IQueryable<Productos> ProductosVisibles
        //{
        //    get
        //    {
        //        switch (RolUsuarioActual)
        //        {

        //            case "Administrador Comercio":
        //                var comercios = Session.ObtenerComercios();
        //                return Contexto.Productos
        //                        .Where(c => comercios.Contains(c.comercioId)).AsQueryable();
        //            case "Administrador Sucursal":
        //                var sucursales = Session.ObtenerSucursales();
        //                return Contexto.Productos
        //                    .Where(c => c.SucursalProductos
        //                                .Any(d => sucursales.Contains(d.sucursalId)))
        //                    .AsQueryable();
        //            case "Operador Plataforma":
        //            case "Operador Empresa":
        //            case "Administrador Empresa":
        //            default:
        //                return Enumerable.Empty<Productos>().AsQueryable();
        //        }
        //    }
        //}
        //public IQueryable<Insumos> InsumosVisibles
        //{
        //    get
        //    {
        //        switch (RolUsuarioActual)
        //        {

        //            case "Administrador Comercio":
        //                var comercios = Session.ObtenerComercios();
        //                return Contexto.Insumos
        //                    .Where(c => comercios.Contains(c.comercioId)).AsQueryable();
        //            case "Administrador Sucursal":
        //                var sucursales = Session.ObtenerSucursales();
        //                return Contexto.Insumos
        //                    .Where(c => c.Comercios.Sucursales
        //                        .Any(d => sucursales.Contains(d.idSucursal)))
        //                    .AsQueryable();
        //            case "Operador Plataforma":
        //            case "Operador Empresa":
        //            case "Administrador Empresa":
        //            default:
        //                return Enumerable.Empty<Insumos>().AsQueryable();
        //        }
        //    }
        //}
        #endregion
        #region LOGS
        private readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ILog Logger { get => _logger; }
        #endregion
        #region ALERTS
        public bool showAlerts
        {
            get
            {
                string KeyValue = System.Web.Configuration.WebConfigurationManager.AppSettings["ShowAlerts"];
                return !string.IsNullOrEmpty(KeyValue) ? Convert.ToBoolean(KeyValue) : true;
            }
        }
        public void ShowAlertSuccess(string message, bool visible = true)
        {
            AddAlert(AlertStyles.Success, message, visible);
        }
        public void ShowAlertInformation(string message, bool visible = true)
        {
            AddAlert(AlertStyles.Information, message, visible);
        }
        public void ShowAlertWarning(string message, bool visible = true)
        {
            AddAlert(AlertStyles.Warning, message, visible);
        }
        public void ShowAlertDanger(string message, bool visible = true)
        {
            AddAlert(AlertStyles.Danger, message, visible);
        }
        public void ShowAlertException(Exception ex, bool visible = true)
        {
            TempData["Exception"] = ex;
            AddAlert(AlertStyles.Danger, "Ocurrió un error, favor de contactar con el administrador." + ex.Message, visible);
        }
        public void ShowAlertException(string message, bool visible = true)
        {
            AddAlert(AlertStyles.Danger, "Ocurrió un error, favor de contactar con el administrador." + message, visible);
        }
        private void AddAlert(string alertStyle,string message,bool visible)
        {
            var alerts = TempData.ContainsKey(Alert.TempDataKey)?(List<Alert>) TempData[Alert.TempDataKey] :new List<Alert>();

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = message,
                Visible = visible
            });
            TempData[Alert.TempDataKey] = alerts;
        }
        #endregion

        #region HELPERS
        public string ObtenerExInfo(Exception ex)
        {
            string name = string.Empty;
            String type = this.GetType().ToString();
            StackTrace stackTrace = new StackTrace(ex);
            Assembly assembly = Assembly.GetExecutingAssembly();
            StackFrame[] stackFrameArray = stackTrace.GetFrames();
            if (stackFrameArray != null)
            {
                var methodsArray = stackFrameArray.Select(f => f.GetMethod());
                if (methodsArray != null)
                {
                    MethodBase methodBase = methodsArray.FirstOrDefault(m => m.Module?.Assembly == assembly);
                    name = methodBase == null ? string.Empty : methodBase.Name;
                }
            }

            string msg = "Ocurrió un error, favor de contactar con el administrador." + System.Environment.NewLine + $"[{type}.{name}] {System.Environment.NewLine}{ex.Message} {System.Environment.NewLine}{ex.InnerException?.Message}";
            Logger.Error(msg);
            return msg;
        }

        public SelectListItem[] ObtenerProveedores(int? selected = null)
	 
								 
											  
												 
														  
												   
						 
	   
																																								
						   
        {
            var lista = ComerciosFirmados
                .Include(c => c.Proveedores)
                .SelectMany(c => c.Proveedores)
                .Select(c => new SelectListItem
                {
                    Text = c.descripcion,
                    Value = c.idProveedor.ToString(),
                    Selected = c.idProveedor == selected,
                }).ToArray();
            return lista;
        }
	   
																																																																										  
										  
					 
	 

        public SelectListItem[] ObtenerSucursales(int? selected = null)
        {
            return SucursalesFirmadas
                .Select(c => new SelectListItem
                {
                    Text = c.nombre,
                    Value = c.idSucursal.ToString(),
                    Selected = c.idSucursal == selected,
                }).ToArray();
								
								
								
								
							   
																																																																																																																																																				
        }


















        public bool EntidadTieneRelacion(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
																															 
													   
										   
											  
								
								
								
								
								
							   
																																																																																								   
	 

												  
	 
									   
							   
            return ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint");
        }
        
        #endregion
    }
   
}
