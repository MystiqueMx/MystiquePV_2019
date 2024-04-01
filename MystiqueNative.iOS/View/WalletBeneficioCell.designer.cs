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
    [Register ("WalletBeneficioCell")]
    partial class WalletBeneficioCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BeneficioDescripcion { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EliminarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imagenBeneficio { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SolicitarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tiempo { get; set; }

        [Action ("EliminarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EliminarButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("SolicitarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SolicitarButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BeneficioDescripcion != null) {
                BeneficioDescripcion.Dispose ();
                BeneficioDescripcion = null;
            }

            if (EliminarButton != null) {
                EliminarButton.Dispose ();
                EliminarButton = null;
            }

            if (imagenBeneficio != null) {
                imagenBeneficio.Dispose ();
                imagenBeneficio = null;
            }

            if (SolicitarButton != null) {
                SolicitarButton.Dispose ();
                SolicitarButton = null;
            }

            if (tiempo != null) {
                tiempo.Dispose ();
                tiempo = null;
            }
        }
    }
}