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
    [Register ("CalificarBeneficio")]
    partial class CalificarBeneficio
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CalificarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CancelarCalificar { get; set; }

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
        UIKit.UIImageView star1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView star2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView star3 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView star4 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView star5 { get; set; }

        [Action ("CalificarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CalificarButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("CancelarCalificar_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CancelarCalificar_TouchUpInside (UIKit.UIButton sender);

        [Action ("CloseModalButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseModalButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CalificarButton != null) {
                CalificarButton.Dispose ();
                CalificarButton = null;
            }

            if (CancelarCalificar != null) {
                CancelarCalificar.Dispose ();
                CancelarCalificar = null;
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

            if (star1 != null) {
                star1.Dispose ();
                star1 = null;
            }

            if (star2 != null) {
                star2.Dispose ();
                star2 = null;
            }

            if (star3 != null) {
                star3.Dispose ();
                star3 = null;
            }

            if (star4 != null) {
                star4.Dispose ();
                star4 = null;
            }

            if (star5 != null) {
                star5.Dispose ();
                star5 = null;
            }
        }
    }
}