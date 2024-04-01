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
    [Register ("DetallePerfil")]
    partial class DetallePerfil
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CerrarSesion { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield ColoniaField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield CorreoField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel nombreLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield PasswordField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield PhoneField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePictureDetalle { get; set; }

        [Action ("CerrarSesion_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CerrarSesion_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CerrarSesion != null) {
                CerrarSesion.Dispose ();
                CerrarSesion = null;
            }

            if (ColoniaField != null) {
                ColoniaField.Dispose ();
                ColoniaField = null;
            }

            if (CorreoField != null) {
                CorreoField.Dispose ();
                CorreoField = null;
            }

            if (nombreLabel != null) {
                nombreLabel.Dispose ();
                nombreLabel = null;
            }

            if (PasswordField != null) {
                PasswordField.Dispose ();
                PasswordField = null;
            }

            if (PhoneField != null) {
                PhoneField.Dispose ();
                PhoneField = null;
            }

            if (ProfilePictureDetalle != null) {
                ProfilePictureDetalle.Dispose ();
                ProfilePictureDetalle = null;
            }
        }
    }
}