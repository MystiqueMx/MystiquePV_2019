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
    [Register ("MiSaldoViewController")]
    partial class MiSaldoViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Canjeados { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView CanjeadosView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PuntosActuales { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Sumados { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SumadosView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Canjeados != null) {
                Canjeados.Dispose ();
                Canjeados = null;
            }

            if (CanjeadosView != null) {
                CanjeadosView.Dispose ();
                CanjeadosView = null;
            }

            if (PuntosActuales != null) {
                PuntosActuales.Dispose ();
                PuntosActuales = null;
            }

            if (Sumados != null) {
                Sumados.Dispose ();
                Sumados = null;
            }

            if (SumadosView != null) {
                SumadosView.Dispose ();
                SumadosView = null;
            }
        }
    }
}