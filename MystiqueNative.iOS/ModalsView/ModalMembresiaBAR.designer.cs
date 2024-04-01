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
    [Register ("ModalMembresiaBAR")]
    partial class ModalMembresiaBAR
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView checkboxView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CloseModalButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView fonditojeje { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ModalView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView qrImage { get; set; }

        [Action ("CloseModalButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseModalButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (checkboxView != null) {
                checkboxView.Dispose ();
                checkboxView = null;
            }

            if (CloseModalButton != null) {
                CloseModalButton.Dispose ();
                CloseModalButton = null;
            }

            if (fonditojeje != null) {
                fonditojeje.Dispose ();
                fonditojeje = null;
            }

            if (ModalView != null) {
                ModalView.Dispose ();
                ModalView = null;
            }

            if (qrImage != null) {
                qrImage.Dispose ();
                qrImage = null;
            }
        }
    }
}