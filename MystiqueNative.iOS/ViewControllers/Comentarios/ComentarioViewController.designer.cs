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
    [Register ("ComentarioViewController")]
    partial class ComentarioViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Comentario { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ComentarioField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CountLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EnviarComment { get; set; }

        [Action ("EnviarComment_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EnviarComment_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (Comentario != null) {
                Comentario.Dispose ();
                Comentario = null;
            }

            if (ComentarioField != null) {
                ComentarioField.Dispose ();
                ComentarioField = null;
            }

            if (CountLabel != null) {
                CountLabel.Dispose ();
                CountLabel = null;
            }

            if (EnviarComment != null) {
                EnviarComment.Dispose ();
                EnviarComment = null;
            }
        }
    }
}