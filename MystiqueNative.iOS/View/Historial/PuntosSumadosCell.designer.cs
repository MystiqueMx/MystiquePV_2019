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
    [Register ("PuntosSumadosCell")]
    partial class PuntosSumadosCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FechaLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MontoCompraLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NoTicketLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PuntosLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ViewStack { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FechaLabel != null) {
                FechaLabel.Dispose ();
                FechaLabel = null;
            }

            if (MontoCompraLabel != null) {
                MontoCompraLabel.Dispose ();
                MontoCompraLabel = null;
            }

            if (NoTicketLabel != null) {
                NoTicketLabel.Dispose ();
                NoTicketLabel = null;
            }

            if (PuntosLabel != null) {
                PuntosLabel.Dispose ();
                PuntosLabel = null;
            }

            if (ViewStack != null) {
                ViewStack.Dispose ();
                ViewStack = null;
            }
        }
    }
}