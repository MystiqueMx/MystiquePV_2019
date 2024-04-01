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
    [Register ("MembresiaViewController")]
    partial class MembresiaViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BARCODE { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CerrarSesionButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CiudadLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ColoniaLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FechaNacimiento { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Folio { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Nombre { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton QR { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Sexo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Telefono { get; set; }

        [Action ("BARCODE_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BARCODE_TouchUpInside (UIKit.UIButton sender);

        [Action ("CerrarSesionButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CerrarSesionButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("QR_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void QR_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BARCODE != null) {
                BARCODE.Dispose ();
                BARCODE = null;
            }

            if (CerrarSesionButton != null) {
                CerrarSesionButton.Dispose ();
                CerrarSesionButton = null;
            }

            if (CiudadLabel != null) {
                CiudadLabel.Dispose ();
                CiudadLabel = null;
            }

            if (ColoniaLabel != null) {
                ColoniaLabel.Dispose ();
                ColoniaLabel = null;
            }

            if (FechaNacimiento != null) {
                FechaNacimiento.Dispose ();
                FechaNacimiento = null;
            }

            if (Folio != null) {
                Folio.Dispose ();
                Folio = null;
            }

            if (Nombre != null) {
                Nombre.Dispose ();
                Nombre = null;
            }

            if (ProfilePic != null) {
                ProfilePic.Dispose ();
                ProfilePic = null;
            }

            if (QR != null) {
                QR.Dispose ();
                QR = null;
            }

            if (Sexo != null) {
                Sexo.Dispose ();
                Sexo = null;
            }

            if (Telefono != null) {
                Telefono.Dispose ();
                Telefono = null;
            }
        }
    }
}