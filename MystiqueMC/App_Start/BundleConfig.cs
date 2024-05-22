// Decompiled with JetBrains decompiler
// Type: MystiqueMC.BundleConfig
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using System.Web.Optimization;


namespace MystiqueMC
{
  public class BundleConfig
  {
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-3.1.1.min.js"));
      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate.min.js"));
      bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.min.js"));
      bundles.Add(new ScriptBundle("~/bundles/inspinia").Include("~/Scripts/app/inspinia.js", "~/Scripts/app/RVS.js"));
      bundles.Add(new ScriptBundle("~/plugins/slimScroll").Include("~/Scripts/plugins/slimScroll/jquery.slimscroll.min.js"));
      bundles.Add(new ScriptBundle("~/plugins/metsiMenu").Include("~/Scripts/plugins/metisMenu/metisMenu.min.js"));
      bundles.Add(new ScriptBundle("~/plugins/pace").Include("~/Scripts/plugins/pace/pace.min.js"));
      bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/app.css", "~/Content/bootstrap.min.css", "~/Content/animate.css", "~/Content/style.css"));
      bundles.Add(new StyleBundle("~/font-awesome/css").Include("~/fonts/font-awesome/css/font-awesome.min.css", (IItemTransform) new CssRewriteUrlTransform()));
      bundles.Add(new StyleBundle("~/Content/plugins/dataTables/dataTablesStyles").Include("~/Content/plugins/dataTables/dataTables.bootstrap.css", "~/Content/plugins/dataTables/dataTables.responsive.css", "~/Content/plugins/dataTables/dataTables.tableTools.min.css"));
      bundles.Add(new ScriptBundle("~/plugins/dataTables").Include("~/Scripts/plugins/dataTables/jquery.dataTables.js", "~/Scripts/plugins/dataTables/dataTables.bootstrap.js", "~/Scripts/plugins/dataTables/dataTables.responsive.js", "~/Scripts/plugins/dataTables/dataTables.tableTools.min.js"));
      bundles.Add(new ScriptBundle("~/bundles/dropzonejs").Include("~/Scripts/dropzone/dropzone.js"));
      bundles.Add(new StyleBundle("~/Content/dropzonecss").Include("~/Scripts/dropzone/css/basic.css", "~/Scripts/dropzone/css/dropzone.css"));
      bundles.Add(new ScriptBundle("~/plugins/inputMask").Include("~/Scripts/plugins/inputmask/inputmask.min.js"));
    }
  }
}
