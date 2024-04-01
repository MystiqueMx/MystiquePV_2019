using CoreAnimation;
using CoreGraphics;
using Foundation;
using MystiqueNative.iOS.View;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UIKit;
using MystiqueNative.iOS.View;
using System.Drawing;
using AudioToolbox;

namespace MystiqueNative.iOS
{
    public partial class ComentarioViewController : UIViewController
    {
        private NSTimer timer;
        LoadingOverlay loadPop;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIPickerView pickerView = new UIPickerView(new CGRect(0, 44, this.View.Bounds.Width, 216));
            var pickerModel = new CommentModel(ComentarioField);

            pickerView.Model = pickerModel;

            ComentarioField.InputView = pickerView;
            AddDoneButton(ComentarioField, Comentario);

            Comentario.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 255;
            };

            AppDelegate.Comment.PropertyChanged += CommentPropertyChanged;
            var bounds2 = View.Bounds;
            loadPop = new LoadingOverlay(bounds2, "Enviando comentario...");
            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);

            

            this.timer = NSTimer.CreateRepeatingScheduledTimer(0.1, (_) =>
            {
                int x = Comentario.Text.ToCharArray().Length;

                CountLabel.Text = x + "/255";
            });

        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
         
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AppDelegate.Comment.PropertyChanged -= CommentPropertyChanged;

        }
        protected void AddDoneButton(UITextField uiTextField, UITextField NextTextfield)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            UIBarButtonItem buttonSiguiente = new UIBarButtonItem("Siguiente", UIBarButtonItemStyle.Done, delegate
            {
                if (string.IsNullOrEmpty(uiTextField.Text))
                {
                    uiTextField.Text = "Comentario";
                }
                NextTextfield.BecomeFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                buttonSiguiente
            };

            uiTextField.InputAccessoryView = toolbar;
        }

        private void CommentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {


            if (e.PropertyName == "ErrorStatus")
            {
                if (!string.IsNullOrEmpty(AppDelegate.Comment.ErrorMessage))
                {
                    var okAlertController = UIAlertController.Create("Comentarios", AppDelegate.Comment.ErrorMessage, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                    {
                        NavigationController.PopViewController(true);
                    }));

                    PresentViewController(okAlertController, true, null);
                    AppDelegate.Comment.ErrorMessage = string.Empty;
                }
                else
                {
                    var okAlertController = UIAlertController.Create("Comentarios", AppDelegate.Comment.ErrorMessage, UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    PresentViewController(okAlertController, true, null);
                    AppDelegate.Comment.ErrorMessage = string.Empty;

                }
                if (AppDelegate.Comment.ErrorStatus)
                {
                    ComentarioField.Text = string.Empty;
                }
            }

            if (e.PropertyName == "IsBusy")
            {
                if (AppDelegate.Comment.IsBusy)
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        View.EndEditing(true);


                    });
                }
                else
                {
                    BeginInvokeOnMainThread( () =>
                    {
                       // await Task.Delay(2000);

                        loadPop.Hide();
                    });
                }


            }
        }
        public ComentarioViewController (IntPtr handle) : base (handle)
        {
        }

        partial void EnviarComment_TouchUpInside(UIButton sender)
        {
            var border = new CALayer();
            nfloat width = 1.5f;
            Comentario.Layer.BorderWidth = 0;

            if (!string.IsNullOrEmpty(Comentario.Text))
            {
                AppDelegate.Comment.EnviarComentario(ComentarioField.Text,Comentario.Text );
                View.Add(loadPop);
            } else
            {
                border.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                border.Frame = new CoreGraphics.CGRect(0, Comentario.Frame.Size.Height - width,
                Comentario.Frame.Size.Width, Comentario.Frame.Size.Height);
                border.BorderWidth = width;
                Comentario.Layer.AddSublayer(border);
                Comentario.Layer.MasksToBounds = true;

                var okAlertController = UIAlertController.Create("Comentarios", "Hay campos vacíos", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(okAlertController, true, null);
                SystemSound.Vibrate.PlaySystemSound();
            }

        }
    }
}