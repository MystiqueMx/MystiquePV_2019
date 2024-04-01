// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MystiqueNative.iOS
{
    [Register ("MenuPrincipalViewController")]
    partial class MenuPrincipalViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView CanjearPuntosView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView CapturarPuntosView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView FacturacionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MiSaldo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView PromosView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CanjearPuntosView != null) {
                CanjearPuntosView.Dispose ();
                CanjearPuntosView = null;
            }

            if (CapturarPuntosView != null) {
                CapturarPuntosView.Dispose ();
                CapturarPuntosView = null;
            }

            if (FacturacionView != null) {
                FacturacionView.Dispose ();
                FacturacionView = null;
            }

            if (MiSaldo != null) {
                MiSaldo.Dispose ();
                MiSaldo = null;
            }

            if (PromosView != null) {
                PromosView.Dispose ();
                PromosView = null;
            }
        }
    }
}