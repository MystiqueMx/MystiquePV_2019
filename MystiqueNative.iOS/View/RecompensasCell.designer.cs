// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace MystiqueNative.iOS
{
    [Register ("RecompensasCell")]
    partial class RecompensasCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NombreRecompensa { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PuntosRecompensa { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RecompensaButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView RecompensaImg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView RecompensaView { get; set; }

        [Action ("RecompensaButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RecompensaButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (NombreRecompensa != null) {
                NombreRecompensa.Dispose ();
                NombreRecompensa = null;
            }

            if (PuntosRecompensa != null) {
                PuntosRecompensa.Dispose ();
                PuntosRecompensa = null;
            }

            if (RecompensaButton != null) {
                RecompensaButton.Dispose ();
                RecompensaButton = null;
            }

            if (RecompensaImg != null) {
                RecompensaImg.Dispose ();
                RecompensaImg = null;
            }

            if (RecompensaView != null) {
                RecompensaView.Dispose ();
                RecompensaView = null;
            }
        }
    }
}