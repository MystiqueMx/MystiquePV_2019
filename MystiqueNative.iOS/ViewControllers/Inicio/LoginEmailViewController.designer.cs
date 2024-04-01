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
    [Register ("LoginEmailViewController")]
    partial class LoginEmailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield CorreoTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel EmptyLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton IniciarSesionButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield PasswordTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RecuperarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RegistrarmeButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RegresarButton { get; set; }

        [Action ("IniciarSesionButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void IniciarSesionButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CorreoTextField != null) {
                CorreoTextField.Dispose ();
                CorreoTextField = null;
            }

            if (EmptyLabel != null) {
                EmptyLabel.Dispose ();
                EmptyLabel = null;
            }

            if (IniciarSesionButton != null) {
                IniciarSesionButton.Dispose ();
                IniciarSesionButton = null;
            }

            if (PasswordTextField != null) {
                PasswordTextField.Dispose ();
                PasswordTextField = null;
            }

            if (RecuperarButton != null) {
                RecuperarButton.Dispose ();
                RecuperarButton = null;
            }

            if (RegistrarmeButton != null) {
                RegistrarmeButton.Dispose ();
                RegistrarmeButton = null;
            }

            if (RegresarButton != null) {
                RegresarButton.Dispose ();
                RegresarButton = null;
            }
        }
    }
}