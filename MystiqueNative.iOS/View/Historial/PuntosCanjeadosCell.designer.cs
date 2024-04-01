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
    [Register ("PuntosCanjeadosCell")]
    partial class PuntosCanjeadosCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FechaLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ProductoLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PuntosLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FechaLabel != null) {
                FechaLabel.Dispose ();
                FechaLabel = null;
            }

            if (ProductoLabel != null) {
                ProductoLabel.Dispose ();
                ProductoLabel = null;
            }

            if (PuntosLabel != null) {
                PuntosLabel.Dispose ();
                PuntosLabel = null;
            }
        }
    }
}