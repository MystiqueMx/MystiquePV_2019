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
    [Register ("BeneficioCell")]
    partial class BeneficioCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AgregarFavoritos { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CanjearButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImageBeneficio { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelBeneficios { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ShareButton { get; set; }

        [Action ("AgregarFavoritos_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AgregarFavoritos_TouchUpInside (UIKit.UIButton sender);

        [Action ("CanjearButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CanjearButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("ShareButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShareButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AgregarFavoritos != null) {
                AgregarFavoritos.Dispose ();
                AgregarFavoritos = null;
            }

            if (CanjearButton != null) {
                CanjearButton.Dispose ();
                CanjearButton = null;
            }

            if (ImageBeneficio != null) {
                ImageBeneficio.Dispose ();
                ImageBeneficio = null;
            }

            if (labelBeneficios != null) {
                labelBeneficios.Dispose ();
                labelBeneficios = null;
            }

            if (ShareButton != null) {
                ShareButton.Dispose ();
                ShareButton = null;
            }
        }
    }
}