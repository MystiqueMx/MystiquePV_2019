using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MystiqueMC
{
    public static class HMTLHelperExtensions
    {
        private const int TIPO_SUGERENCIA = 1;
        private const int TIPO_COMENTARIO = 2;
        private const int TIPO_QUEJA = 3;
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static string ClaseTipoComentario(this HtmlHelper html, int TipoComentario = 0, bool IsCircle = true)
        {
            string clase;
            switch (TipoComentario)
            {
                case TIPO_SUGERENCIA:
                    if(IsCircle)
                        clase = "text-info";
                    else
                        clase = "label-info";
                    break;
                case TIPO_COMENTARIO:
                    if (IsCircle)
                        clase = "text-warning";
                    else
                        clase = "label-warning";
                    break;
                case TIPO_QUEJA:
                    if (IsCircle)
                        clase = "text-danger";
                    else
                        clase = "label-danger";
                    break;
                default:
                    if (IsCircle)
                        clase = "text-info";
                    else
                        clase = "label-info";
                    break;
            }
            return clase;
        }
        public static string ClaseComentarioLeido(this HtmlHelper html, bool leido)
        {
            return leido ? "read" : "unread";
        }
        public static string CheckBoxEditar(this HtmlHelper html, bool Marcado)
        {
            return Marcado ? "checked" : "";
        }
        public static string OptionIsSelected(this HtmlHelper html, bool Marcado, string Clase = "selected")
        {
            return Marcado ? Clase : "";
        }
    }
}