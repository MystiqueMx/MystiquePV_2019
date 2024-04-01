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
    [Register ("DetalleCanjearVC")]
    partial class DetalleCanjearVC
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CanjearButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView CanjeoStackl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CostoRecompensa { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CostoRecompensaCanjeo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DescripcionRecompensa { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GenerarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImagenRecompensa { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NombreCanjeo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NombreRecompensa { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView NombreRecompensaCanjeo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PointsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView RecompensaStack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SaldoDisponible { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SaldoRestante { get; set; }

        [Action ("CanjearButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CanjearButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("GenerarButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GenerarButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (CanjearButton != null) {
                CanjearButton.Dispose ();
                CanjearButton = null;
            }

            if (CanjeoStackl != null) {
                CanjeoStackl.Dispose ();
                CanjeoStackl = null;
            }

            if (CostoRecompensa != null) {
                CostoRecompensa.Dispose ();
                CostoRecompensa = null;
            }

            if (CostoRecompensaCanjeo != null) {
                CostoRecompensaCanjeo.Dispose ();
                CostoRecompensaCanjeo = null;
            }

            if (DescripcionRecompensa != null) {
                DescripcionRecompensa.Dispose ();
                DescripcionRecompensa = null;
            }

            if (GenerarButton != null) {
                GenerarButton.Dispose ();
                GenerarButton = null;
            }

            if (ImagenRecompensa != null) {
                ImagenRecompensa.Dispose ();
                ImagenRecompensa = null;
            }

            if (NombreCanjeo != null) {
                NombreCanjeo.Dispose ();
                NombreCanjeo = null;
            }

            if (NombreRecompensa != null) {
                NombreRecompensa.Dispose ();
                NombreRecompensa = null;
            }

            if (NombreRecompensaCanjeo != null) {
                NombreRecompensaCanjeo.Dispose ();
                NombreRecompensaCanjeo = null;
            }

            if (PointsLabel != null) {
                PointsLabel.Dispose ();
                PointsLabel = null;
            }

            if (RecompensaStack != null) {
                RecompensaStack.Dispose ();
                RecompensaStack = null;
            }

            if (SaldoDisponible != null) {
                SaldoDisponible.Dispose ();
                SaldoDisponible = null;
            }

            if (SaldoRestante != null) {
                SaldoRestante.Dispose ();
                SaldoRestante = null;
            }
        }
    }
}