// Decompiled with JetBrains decompiler
// Type: MystiqueMC.HMTLHelperExtensions
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System.Web.Mvc;


namespace MystiqueMC
{
  public static class HMTLHelperExtensions
  {
    private const int TIPO_SUGERENCIA = 1;
    private const int TIPO_COMENTARIO = 2;
    private const int TIPO_QUEJA = 3;

    public static string IsSelected(
      this HtmlHelper html,
      string controller = null,
      string action = null,
      string cssClass = null)
    {
      if (string.IsNullOrEmpty(cssClass))
        cssClass = "active";
      string str1 = (string) html.ViewContext.RouteData.Values[nameof (action)];
      string str2 = (string) html.ViewContext.RouteData.Values[nameof (controller)];
      if (string.IsNullOrEmpty(controller))
        controller = str2;
      if (string.IsNullOrEmpty(action))
        action = str1;
      return !(controller == str2) || !(action == str1) ? string.Empty : cssClass;
    }

    public static string PageClass(this HtmlHelper html)
    {
      return (string) html.ViewContext.RouteData.Values["action"];
    }

    public static string ClaseTipoComentario(
      this HtmlHelper html,
      int TipoComentario = 0,
      bool IsCircle = true)
    {
      string str;
      switch (TipoComentario)
      {
        case 1:
          str = !IsCircle ? "label-info" : "text-info";
          break;
        case 2:
          str = !IsCircle ? "label-warning" : "text-warning";
          break;
        case 3:
          str = !IsCircle ? "label-danger" : "text-danger";
          break;
        default:
          str = !IsCircle ? "label-info" : "text-info";
          break;
      }
      return str;
    }

    public static string ClaseComentarioLeido(this HtmlHelper html, bool leido)
    {
      return !leido ? "unread" : "read";
    }

    public static string CheckBoxEditar(this HtmlHelper html, bool Marcado)
    {
      return !Marcado ? "" : "checked";
    }

    public static string OptionIsSelected(this HtmlHelper html, bool Marcado, string Clase = "selected")
    {
      return !Marcado ? "" : Clase;
    }
  }
}
