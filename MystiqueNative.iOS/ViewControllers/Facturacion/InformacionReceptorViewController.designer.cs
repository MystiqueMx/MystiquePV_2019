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
    [Register ("InformacionReceptorViewController")]
    partial class InformacionReceptorViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield CFDIField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield CodigoPostalField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield CorreoField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield DireccionField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GuardarReceptor { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield RazonSocialField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield RFCField { get; set; }

        [Action ("GuardarReceptor_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GuardarReceptor_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CFDIField != null) {
                CFDIField.Dispose ();
                CFDIField = null;
            }

            if (CodigoPostalField != null) {
                CodigoPostalField.Dispose ();
                CodigoPostalField = null;
            }

            if (CorreoField != null) {
                CorreoField.Dispose ();
                CorreoField = null;
            }

            if (DireccionField != null) {
                DireccionField.Dispose ();
                DireccionField = null;
            }

            if (GuardarReceptor != null) {
                GuardarReceptor.Dispose ();
                GuardarReceptor = null;
            }

            if (RazonSocialField != null) {
                RazonSocialField.Dispose ();
                RazonSocialField = null;
            }

            if (RFCField != null) {
                RFCField.Dispose ();
                RFCField = null;
            }
        }
    }
}