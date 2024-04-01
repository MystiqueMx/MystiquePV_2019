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
    [Register ("ZonaCityViewController")]
    partial class ZonaCityViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView DescripcionZona { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Latitud { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Longitud { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch ZonaCitySwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ZonaTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DescripcionZona != null) {
                DescripcionZona.Dispose ();
                DescripcionZona = null;
            }

            if (Latitud != null) {
                Latitud.Dispose ();
                Latitud = null;
            }

            if (Longitud != null) {
                Longitud.Dispose ();
                Longitud = null;
            }

            if (ZonaCitySwitch != null) {
                ZonaCitySwitch.Dispose ();
                ZonaCitySwitch = null;
            }

            if (ZonaTitle != null) {
                ZonaTitle.Dispose ();
                ZonaTitle = null;
            }
        }
    }
}