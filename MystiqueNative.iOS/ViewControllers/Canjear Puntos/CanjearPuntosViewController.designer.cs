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
    [Register ("CanjearPuntosViewController")]
    partial class CanjearPuntosViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView activityindicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PuntosLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.RecompensasViewController TableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (activityindicator != null) {
                activityindicator.Dispose ();
                activityindicator = null;
            }

            if (PuntosLabel != null) {
                PuntosLabel.Dispose ();
                PuntosLabel = null;
            }

            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }
        }
    }
}