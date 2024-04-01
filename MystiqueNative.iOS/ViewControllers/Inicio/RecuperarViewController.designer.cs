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
    [Register ("RecuperarViewController")]
    partial class RecuperarViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield inputEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RecuperarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RegresarButton { get; set; }

        [Action ("RecuperarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RecuperarButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (inputEmail != null) {
                inputEmail.Dispose ();
                inputEmail = null;
            }

            if (RecuperarButton != null) {
                RecuperarButton.Dispose ();
                RecuperarButton = null;
            }

            if (RegresarButton != null) {
                RegresarButton.Dispose ();
                RegresarButton = null;
            }
        }
    }
}