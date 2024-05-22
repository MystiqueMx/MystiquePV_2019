// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.ControlInventario
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using log4net;
using MystiqueMC.DAL;
using MystiqueMC.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace MystiqueMC.Helpers
{
  public class ControlInventario
  {
    private MystiqueMeEntities _context;
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public MystiqueMeEntities Contexto
    {
      get
      {
        if (this._context != null)
          return this._context;
        this._context = new MystiqueMeEntities();
        this._context.Database.Log = (Action<string>) (sql => this.Logger.Debug((object) sql));
        return this._context;
      }
    }

    public ILog Logger => this._logger;

    public ControlInventariosResponse registroMovimientoInventario(
      MystiqueMeEntities Contexto,
      int? catMovimientoId,
      Decimal cantidad,
      int insumoId,
      int sucursalId,
      Decimal cantidadNueva,
      usuarios UsuarioActual,
      TiposMovimientosInventario tipoMovimiento,
      int? detalleCompraId,
      string observaciones,
      bool saveChanges = true)
    {
      ControlInventariosResponse inventariosResponse = new ControlInventariosResponse();
      try
      {
        Insumos insumos = Contexto.Insumos.Where<Insumos>((Expression<Func<Insumos, bool>>) (i => i.idInsumo == insumoId)).FirstOrDefault<Insumos>();
        Contexto.MovimientoInventarios.Add(new MovimientoInventarios()
        {
          catMovimientoId = catMovimientoId,
          insumoId = insumoId,
          cantidad = cantidad,
          sucursalId = sucursalId,
          fechaRegistro = DateTime.Now,
          usuarioRegistroId = new int?(UsuarioActual.idUsuario),
          tipoMovimiento = tipoMovimiento.ToString(),
          detalleCompraId = detalleCompraId,
          observaciones = observaciones,
          unidadMedidaCompraId = new int?(insumos.UnidadMedida1.idUnidadMedida),
          unidadMedidaCompraDesc = insumos.UnidadMedida1.descripcion,
          unidadMedidaConsumoId = new int?(insumos.UnidadMedida.idUnidadMedida),
          unidadMedidaConsumoaDesc = insumos.UnidadMedida.descripcion
        });
        if (Contexto.Inventarios.Where<Inventarios>((Expression<Func<Inventarios, bool>>) (i => i.insumoId == insumoId && i.sucursalId == sucursalId)).FirstOrDefault<Inventarios>() == null)
          Contexto.Inventarios.Add(new Inventarios()
          {
            activo = true,
            cantidad = cantidadNueva,
            fechaRegistro = DateTime.Now,
            insumoId = insumoId,
            maximo = 0M,
            minimo = 0M,
            precio = 0M,
            sucursalId = sucursalId
          });
        if (saveChanges)
          Contexto.SaveChanges();
        inventariosResponse.isSuccess = true;
        inventariosResponse.hasError = false;
        inventariosResponse.isRedirectToAction = true;
        inventariosResponse.actionName = "Index";
        inventariosResponse.controllerName = "Inventarios";
        inventariosResponse.routeValue = (object) new
        {
          sucursaId = sucursalId
        };
      }
      catch (Exception ex)
      {
        this.Logger.Error((object) ex);
        inventariosResponse.isSuccess = false;
        inventariosResponse.hasError = true;
        inventariosResponse.isRedirectToAction = true;
        inventariosResponse.actionName = "Index";
        inventariosResponse.controllerName = "Inventarios";
        inventariosResponse.routeValue = (object) new
        {
          sucursaId = sucursalId
        };
      }
      return inventariosResponse;
    }
  }
}
