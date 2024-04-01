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
    [Register ("SumarPuntosViewController")]
    partial class SumarPuntosViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.ScannerView ScanWindow { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView StackScanView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialControls.MDButton TorchButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint widthFrame { get; set; }

        [Action ("TorchButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TorchButton_TouchUpInside (MaterialControls.MDButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ScanWindow != null) {
                ScanWindow.Dispose ();
                ScanWindow = null;
            }

            if (StackScanView != null) {
                StackScanView.Dispose ();
                StackScanView = null;
            }

            if (TorchButton != null) {
                TorchButton.Dispose ();
                TorchButton = null;
            }

            if (widthFrame != null) {
                widthFrame.Dispose ();
                widthFrame = null;
            }
        }
    }
}