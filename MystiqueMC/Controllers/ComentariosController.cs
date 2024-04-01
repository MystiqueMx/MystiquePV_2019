using MystiqueMC.DAL;
using MystiqueMC.Helpers;
using MystiqueMC.Helpers.Emails;
using MystiqueMC.Helpers.Pagination;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace MystiqueMC.Controllers
{
    [ValidatePermissions]
    public class ComentariosController : BaseController
    {
        #region VARS

        #endregion
        #region GET
        [HttpGet]
        public ActionResult Index(int? page, string search = "", int filter = 0)
        {
            try
            {
                var Usuario = Session.ObtenerUsuario();
                if (!Request.IsAuthenticated || Usuario == null)
                    return new { success = false }.ToJsonResult();

                var comentarios = ObtenerComentarios(Usuario.empresaId, search, filter);
                ViewData["NoLeidos"] = ObtenerConteoComentariosNoLeidos(Usuario.empresaId);
                ViewData["Categorias"] = ObtenerCategorias();
                ViewData["Busqueda"] = search;

                return PartialView(PaginatedListImpl<comentarios>.Create(comentarios, page ?? 1));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new
                {
                    exception = e
                });
            }
        }
        [HttpGet]
        public ActionResult Comentario(int? id)
        {
            try
            {
                var Usuario = Session.ObtenerUsuario();
                if (!id.HasValue || id.Value == 0) throw new ArgumentException(nameof(id));
                ViewData["NoLeidos"] = ObtenerConteoComentariosNoLeidos(Usuario.empresaId);
                ViewData["Categorias"] = ObtenerCategorias();
                var Comentario = Contexto.comentarios.Find(id);
                return View(Comentario);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new
                {
                    exception = e
                });
            }
        }
        [HttpGet]
        public ActionResult Responder(int? id)
        {
            try
            {
                var Usuario = Session.ObtenerUsuario();
                if (!id.HasValue || id.Value == 0) throw new ArgumentException(nameof(id));
                ViewData["NoLeidos"] = ObtenerConteoComentariosNoLeidos(Usuario.empresaId);
                ViewData["Categorias"] = ObtenerCategorias();
                var Comentario = Contexto.comentarios.Find(id);
                return View(Comentario);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new
                {
                    exception = e
                });
            }
        }
        #endregion
        #region POST
        [HttpPost, ValidateInput(false)]
        public ActionResult Responder(int IdComentario, string Asunto, string Body)
        {
            try
            {
                var Usuario = Session.ObtenerUsuario();
                var Comentario = Contexto.comentarios.Find(IdComentario);
                var Contenido = new Ganss.XSS.HtmlSanitizer().Sanitize(Body);
                var Destinatario = Comentario.clientes.email;
                var Remitente = Contexto.configuracionSistema
                    .Where(c => c.empresaId == Usuario.empresaId)
                    .Select(c => c.correoContacto)
                    .FirstOrDefault();

                SendEmailDelegate @delegate = new SendEmailDelegate();
                @delegate.SendEmail(Para: Destinatario, Asunto: Asunto, Contenido: Contenido, De: Remitente, ResponderA: Remitente);

                Comentario.leido = true;
                Contexto.Entry(Comentario).State = EntityState.Modified;
                Contexto.SaveChanges();

                return RedirectToAction("Fidelizacion", "Home", new { tabID = "comentarios_button" });
                //return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Fidelizacion", "Home", new { tabID = "comentarios_button" });
                //return RedirectToAction("Index", "Home", new
                //{
                //    exception = e
                //});
            }
        }
        [HttpPost]
        public ActionResult Eliminar(int IdComentario)
        {

            try
            {
                var Comentario = Contexto.comentarios.Find(IdComentario);

                Comentario.activo = false;
                Contexto.Entry(Comentario).State = EntityState.Modified;
                Contexto.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ShowAlertException("");
                return RedirectToAction("Index", "Home", new
                {
                    exception = e
                });
            }
        }
        [HttpPost]
        public ActionResult MarcarEliminados(int[] IdComentarios)
        {
            if (IdComentarios.Length == 0) return Json(new { success = false, message = "No se recibieron comentarios" });
            try
            {
                var comentarios = Contexto.comentarios.Where(c => IdComentarios.Contains(c.idComentario)).ToList();
                comentarios.ForEach(c =>
                {
                    c.activo = false;
                    Contexto.Entry(c).State = EntityState.Modified;
                });
                Contexto.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(new { success = false, message = "Ocurrió un error al marcar los comentarios" });
            }
        }
        [HttpPost]
        public ActionResult MarcarLeidos(int[] IdComentarios)
        {
            if (IdComentarios.Length == 0) return Json(new { success = false, message = "No se recibieron comentarios" });
            try
            {
                var comentarios = Contexto.comentarios.Where(c => IdComentarios.Contains(c.idComentario)).ToList();
                comentarios.ForEach(c =>
                {
                    c.leido = true;
                    Contexto.Entry(c).State = EntityState.Modified;
                });
                Contexto.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(new { success = false, message = "Ocurrió un error al marcar los comentarios" });
            }
        }
        public ActionResult MarcarImportantes(int[] IdComentarios)
        {
            if (IdComentarios.Length == 0) return Json(new { success = false, message = "No se recibieron comentarios" });
            try
            {
                var comentarios = Contexto.comentarios.Where(c => IdComentarios.Contains(c.idComentario)).ToList();
                comentarios.ForEach(c =>
                {
                    c.importante = true;
                    Contexto.Entry(c).State = EntityState.Modified;
                });
                Contexto.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(new { success = false, message = "Ocurrió un error al marcar los comentarios" });
            }
        }
        #endregion
        #region Helpers
        private int ObtenerConteoComentariosNoLeidos(int EmpresaId)
        {
            return Contexto.comentarios
                .Where(c => c.activo
                    && c.clientes.empresaId == EmpresaId
                    && !c.leido)
                .Count();
        }

        private List<catTipoComentario> ObtenerCategorias()
        {
            return Contexto.catTipoComentario.ToList();
        }

        private IQueryable<comentarios> ObtenerComentarios(int EmpresaId, string search, int filter)
        {
            var comentarios = Contexto.comentarios
                .Where(c => c.activo
                    && c.clientes.empresaId == EmpresaId)
                    .AsQueryable();
            if (filter != 0)
                comentarios = comentarios.Where(c => c.catTipoComentarioId == filter);

            if (!string.IsNullOrEmpty(search))
                comentarios = comentarios.Where(c => c.mensaje.Contains(search));

            return comentarios.OrderByDescending(c => c.fechaRegistro);
        }
        #endregion
    }

}