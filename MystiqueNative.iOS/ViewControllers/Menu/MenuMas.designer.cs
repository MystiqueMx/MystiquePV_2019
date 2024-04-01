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
    [Register ("MenuMas")]
    partial class MenuMas
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell CellCerrarSesion { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell CellQuieroDeComer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel nombreMenu { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.ToCircleImage ProfilePic { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView StackQuierodeComer { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CellCerrarSesion != null) {
                CellCerrarSesion.Dispose ();
                CellCerrarSesion = null;
            }

            if (CellQuieroDeComer != null) {
                CellQuieroDeComer.Dispose ();
                CellQuieroDeComer = null;
            }

            if (nombreMenu != null) {
                nombreMenu.Dispose ();
                nombreMenu = null;
            }

            if (ProfilePic != null) {
                ProfilePic.Dispose ();
                ProfilePic = null;
            }

            if (StackQuierodeComer != null) {
                StackQuierodeComer.Dispose ();
                StackQuierodeComer = null;
            }
        }
    }
}