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
    [Register ("MiSaldoCell")]
    partial class MiSaldoCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FechaRegistro { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImagenChevron { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Puntos { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FechaRegistro != null) {
                FechaRegistro.Dispose ();
                FechaRegistro = null;
            }

            if (ImagenChevron != null) {
                ImagenChevron.Dispose ();
                ImagenChevron = null;
            }

            if (Puntos != null) {
                Puntos.Dispose ();
                Puntos = null;
            }
        }
    }
}