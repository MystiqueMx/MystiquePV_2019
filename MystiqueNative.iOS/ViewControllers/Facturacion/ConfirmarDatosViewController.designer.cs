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
    [Register ("ConfirmarDatosViewController")]
    partial class ConfirmarDatosViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CFDILabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CodigoPostalLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ConfirmarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DireccionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EditarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel EmailLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RazonSocialLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RFCLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SucursalLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TotalLabel { get; set; }

        [Action ("ConfirmarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ConfirmarButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("EditarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EditarButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CFDILabel != null) {
                CFDILabel.Dispose ();
                CFDILabel = null;
            }

            if (CodigoPostalLabel != null) {
                CodigoPostalLabel.Dispose ();
                CodigoPostalLabel = null;
            }

            if (ConfirmarButton != null) {
                ConfirmarButton.Dispose ();
                ConfirmarButton = null;
            }

            if (DireccionLabel != null) {
                DireccionLabel.Dispose ();
                DireccionLabel = null;
            }

            if (EditarButton != null) {
                EditarButton.Dispose ();
                EditarButton = null;
            }

            if (EmailLabel != null) {
                EmailLabel.Dispose ();
                EmailLabel = null;
            }

            if (RazonSocialLabel != null) {
                RazonSocialLabel.Dispose ();
                RazonSocialLabel = null;
            }

            if (RFCLabel != null) {
                RFCLabel.Dispose ();
                RFCLabel = null;
            }

            if (SucursalLabel != null) {
                SucursalLabel.Dispose ();
                SucursalLabel = null;
            }

            if (TotalLabel != null) {
                TotalLabel.Dispose ();
                TotalLabel = null;
            }
        }
    }
}