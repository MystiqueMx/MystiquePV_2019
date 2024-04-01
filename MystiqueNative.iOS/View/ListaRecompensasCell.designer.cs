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
    [Register ("ListaRecompensasCell")]
    partial class ListaRecompensasCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView Disponibilidad { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton Eliminar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Estatus { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NombreRecompensa { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView RecompensaImg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SolicitarButton { get; set; }

        [Action ("SolicitarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SolicitarButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (Disponibilidad != null) {
                Disponibilidad.Dispose ();
                Disponibilidad = null;
            }

            if (Eliminar != null) {
                Eliminar.Dispose ();
                Eliminar = null;
            }

            if (Estatus != null) {
                Estatus.Dispose ();
                Estatus = null;
            }

            if (NombreRecompensa != null) {
                NombreRecompensa.Dispose ();
                NombreRecompensa = null;
            }

            if (RecompensaImg != null) {
                RecompensaImg.Dispose ();
                RecompensaImg = null;
            }

            if (SolicitarButton != null) {
                SolicitarButton.Dispose ();
                SolicitarButton = null;
            }
        }
    }
}