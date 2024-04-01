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
    [Register ("CompletarRegistroViewController")]
    partial class CompletarRegistroViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField CiudadTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView CiudadView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ColoniaTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView ColoniaView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField FechaNacimientoTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView FechaNacimientoView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GuardarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField SexoTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView SexoView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TelefonoTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView TelefonoView { get; set; }

        [Action ("GuardarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GuardarButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CiudadTextField != null) {
                CiudadTextField.Dispose ();
                CiudadTextField = null;
            }

            if (CiudadView != null) {
                CiudadView.Dispose ();
                CiudadView = null;
            }

            if (ColoniaTextField != null) {
                ColoniaTextField.Dispose ();
                ColoniaTextField = null;
            }

            if (ColoniaView != null) {
                ColoniaView.Dispose ();
                ColoniaView = null;
            }

            if (FechaNacimientoTextField != null) {
                FechaNacimientoTextField.Dispose ();
                FechaNacimientoTextField = null;
            }

            if (FechaNacimientoView != null) {
                FechaNacimientoView.Dispose ();
                FechaNacimientoView = null;
            }

            if (GuardarButton != null) {
                GuardarButton.Dispose ();
                GuardarButton = null;
            }

            if (SexoTextField != null) {
                SexoTextField.Dispose ();
                SexoTextField = null;
            }

            if (SexoView != null) {
                SexoView.Dispose ();
                SexoView = null;
            }

            if (TelefonoTextField != null) {
                TelefonoTextField.Dispose ();
                TelefonoTextField = null;
            }

            if (TelefonoView != null) {
                TelefonoView.Dispose ();
                TelefonoView = null;
            }
        }
    }
}