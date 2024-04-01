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
    [Register ("CityPointsViewController")]
    partial class CityPointsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView CanjearPuntosView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView CapturarPuntosView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MiSaldoView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView RecompensasView { get; set; }

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

            if (MiSaldoView != null) {
                MiSaldoView.Dispose ();
                MiSaldoView = null;
            }

            if (RecompensasView != null) {
                RecompensasView.Dispose ();
                RecompensasView = null;
            }
        }
    }
}