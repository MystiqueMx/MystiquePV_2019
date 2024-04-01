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
    [Register ("DetalleBeneficiosViewController")]
    partial class DetalleBeneficiosViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialControls.MDButton AddWaller { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint AlturaImagenBeneficio { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel calificacionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialControls.MDButton CalificarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel cantidadLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView EstrellasImg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageBeneficio { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelBeneficio { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelHorarioBeneficio { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel labelNombreSucursal { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem ShareButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MaterialControls.MDButton SolicitarCodeImage { get; set; }

        [Action ("AddWaller_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AddWaller_TouchUpInside (MaterialControls.MDButton sender);

        [Action ("CalificarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CalificarButton_TouchUpInside (MaterialControls.MDButton sender);

        [Action ("ShareButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShareButton_Activated (UIKit.UIBarButtonItem sender);

        [Action ("SolicitarCodeImage_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SolicitarCodeImage_TouchUpInside (MaterialControls.MDButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AddWaller != null) {
                AddWaller.Dispose ();
                AddWaller = null;
            }

            if (AlturaImagenBeneficio != null) {
                AlturaImagenBeneficio.Dispose ();
                AlturaImagenBeneficio = null;
            }

            if (calificacionLabel != null) {
                calificacionLabel.Dispose ();
                calificacionLabel = null;
            }

            if (CalificarButton != null) {
                CalificarButton.Dispose ();
                CalificarButton = null;
            }

            if (cantidadLabel != null) {
                cantidadLabel.Dispose ();
                cantidadLabel = null;
            }

            if (EstrellasImg != null) {
                EstrellasImg.Dispose ();
                EstrellasImg = null;
            }

            if (imageBeneficio != null) {
                imageBeneficio.Dispose ();
                imageBeneficio = null;
            }

            if (labelBeneficio != null) {
                labelBeneficio.Dispose ();
                labelBeneficio = null;
            }

            if (labelHorarioBeneficio != null) {
                labelHorarioBeneficio.Dispose ();
                labelHorarioBeneficio = null;
            }

            if (labelNombreSucursal != null) {
                labelNombreSucursal.Dispose ();
                labelNombreSucursal = null;
            }

            if (ShareButton != null) {
                ShareButton.Dispose ();
                ShareButton = null;
            }

            if (SolicitarCodeImage != null) {
                SolicitarCodeImage.Dispose ();
                SolicitarCodeImage = null;
            }
        }
    }
}