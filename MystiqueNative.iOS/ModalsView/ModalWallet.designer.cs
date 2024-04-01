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
    [Register ("ModalWallet")]
    partial class ModalWallet
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CloseModal { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView fonditojeje { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImagenBeneficio { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ModalView { get; set; }

        [Action ("CloseModal_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseModal_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CloseModal != null) {
                CloseModal.Dispose ();
                CloseModal = null;
            }

            if (fonditojeje != null) {
                fonditojeje.Dispose ();
                fonditojeje = null;
            }

            if (ImagenBeneficio != null) {
                ImagenBeneficio.Dispose ();
                ImagenBeneficio = null;
            }

            if (ModalView != null) {
                ModalView.Dispose ();
                ModalView = null;
            }
        }
    }
}