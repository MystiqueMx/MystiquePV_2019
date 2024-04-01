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
    [Register ("FacturaConsumoCell")]
    partial class FacturaConsumoCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CorreoLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView FacturarConsumoView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NombreLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CorreoLabel != null) {
                CorreoLabel.Dispose ();
                CorreoLabel = null;
            }

            if (FacturarConsumoView != null) {
                FacturarConsumoView.Dispose ();
                FacturarConsumoView = null;
            }

            if (NombreLabel != null) {
                NombreLabel.Dispose ();
                NombreLabel = null;
            }
        }
    }
}