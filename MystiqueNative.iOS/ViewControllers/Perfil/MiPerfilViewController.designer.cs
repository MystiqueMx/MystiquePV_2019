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
    [Register ("MiPerfilViewController")]
    partial class MiPerfilViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield Colonia { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield Correo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialControls.MDButton LogOut { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Nombre { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield Password { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePictureDetalle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield Telefono { get; set; }

        [Action ("LogOut_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LogOut_TouchUpInside (MaterialControls.MDButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (Colonia != null) {
                Colonia.Dispose ();
                Colonia = null;
            }

            if (Correo != null) {
                Correo.Dispose ();
                Correo = null;
            }

            if (LogOut != null) {
                LogOut.Dispose ();
                LogOut = null;
            }

            if (Nombre != null) {
                Nombre.Dispose ();
                Nombre = null;
            }

            if (Password != null) {
                Password.Dispose ();
                Password = null;
            }

            if (ProfilePictureDetalle != null) {
                ProfilePictureDetalle.Dispose ();
                ProfilePictureDetalle = null;
            }

            if (Telefono != null) {
                Telefono.Dispose ();
                Telefono = null;
            }
        }
    }
}