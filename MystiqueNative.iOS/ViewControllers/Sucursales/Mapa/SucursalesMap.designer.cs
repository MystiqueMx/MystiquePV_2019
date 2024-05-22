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
    [Register ("SucursalesMap")]
    partial class SucursalesMap
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MystiqueNative.iOS.textfieldBuscar BuscarInput { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView contaierMap { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BuscarInput != null) {
                BuscarInput.Dispose ();
                BuscarInput = null;
            }

            if (contaierMap != null) {
                contaierMap.Dispose ();
                contaierMap = null;
            }
        }
    }
}