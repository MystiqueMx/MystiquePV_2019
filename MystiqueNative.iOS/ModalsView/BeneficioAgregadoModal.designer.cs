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
    [Register ("BeneficioAgregadoModal")]
    partial class BeneficioAgregadoModal
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CerrarModal { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView fonditojeje { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MensajeModal { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ModalView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton VerWalletButton { get; set; }

        [Action ("CerrarModal_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CerrarModal_TouchUpInside (UIKit.UIButton sender);

        [Action ("VerWalletButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void VerWalletButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CerrarModal != null) {
                CerrarModal.Dispose ();
                CerrarModal = null;
            }

            if (fonditojeje != null) {
                fonditojeje.Dispose ();
                fonditojeje = null;
            }

            if (MensajeModal != null) {
                MensajeModal.Dispose ();
                MensajeModal = null;
            }

            if (ModalView != null) {
                ModalView.Dispose ();
                ModalView = null;
            }

            if (VerWalletButton != null) {
                VerWalletButton.Dispose ();
                VerWalletButton = null;
            }
        }
    }
}