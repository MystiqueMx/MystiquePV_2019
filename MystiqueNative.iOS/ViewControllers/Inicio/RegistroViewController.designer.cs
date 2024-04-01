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
    [Register ("RegistroViewController")]
    partial class RegistroViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield BirthDate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonRegistrar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield Ciudad { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield entryEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield entryName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield entryPass { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield entryPass2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield entryTel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield inputColonia { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationItem navbar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield Paterno { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Registro { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView RegistroTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RegresarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.UnderlineUITextfield SexInput { get; set; }

        [Action ("ButtonRegistrar_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ButtonRegistrar_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BirthDate != null) {
                BirthDate.Dispose ();
                BirthDate = null;
            }

            if (ButtonRegistrar != null) {
                ButtonRegistrar.Dispose ();
                ButtonRegistrar = null;
            }

            if (Ciudad != null) {
                Ciudad.Dispose ();
                Ciudad = null;
            }

            if (entryEmail != null) {
                entryEmail.Dispose ();
                entryEmail = null;
            }

            if (entryName != null) {
                entryName.Dispose ();
                entryName = null;
            }

            if (entryPass != null) {
                entryPass.Dispose ();
                entryPass = null;
            }

            if (entryPass2 != null) {
                entryPass2.Dispose ();
                entryPass2 = null;
            }

            if (entryTel != null) {
                entryTel.Dispose ();
                entryTel = null;
            }

            if (inputColonia != null) {
                inputColonia.Dispose ();
                inputColonia = null;
            }

            if (navbar != null) {
                navbar.Dispose ();
                navbar = null;
            }

            if (Paterno != null) {
                Paterno.Dispose ();
                Paterno = null;
            }

            if (Registro != null) {
                Registro.Dispose ();
                Registro = null;
            }

            if (RegistroTable != null) {
                RegistroTable.Dispose ();
                RegistroTable = null;
            }

            if (RegresarButton != null) {
                RegresarButton.Dispose ();
                RegresarButton = null;
            }

            if (SexInput != null) {
                SexInput.Dispose ();
                SexInput = null;
            }
        }
    }
}