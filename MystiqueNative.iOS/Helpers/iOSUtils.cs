using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MystiqueNative.iOS.Helpers
{
    public class iOSUtils
    {
        /// <summary>
        /// Establece un límite de caracteres que puede introducir en un UITextField
        /// </summary>
        /// <param name="TextField"></param>
        /// <param name="max">MaxLength</param>
        public static void InputMaxLength(UITextField TextField, int max)
        {
            TextField.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= max;
            };
        }

        public static void InputMaxLengthTextView(UITextView TextView, int max)
        {
            TextView.ShouldChangeText = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= max;
            };
        }
        /// <summary>
        /// Muestra una alerta al usuario con un mensaje en específico
        /// </summary>
        /// <param name="ViewController"></param>
        /// <param name="Title"></param>
        /// <param name="Subtitle"></param>
        public static void MostrarAlerta(UIViewController ViewController, string Title, string Subtitle = null)
        {
            var Alert = UIAlertController.Create(Title, Subtitle, UIAlertControllerStyle.Alert);
            Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            ViewController.PresentViewController(Alert, true, null);
        }
        /// <summary>
        /// Agrega un Toolbar con un UIButton 'Siguiente'
        /// </summary>
        /// <param name="NextTextField"></param>
        /// <param name="ViewController"></param>
        /// <returns></returns>
        public static UIView AddToolbarWButton(UITextField NextTextField = null, UIViewController ViewController = null)
        {
            #region Add toolbar w/ button

            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            toolbar.Translucent = true;
            toolbar.BarTintColor = UIColor.FromRGBA(100, 100, 100, 120);
            UIBarButtonItem buttonSiguiente = new UIBarButtonItem("Siguiente", UIBarButtonItemStyle.Done, delegate
            {
                if (NextTextField == null)
                {
                    if (ViewController != null)
                    {
                        ViewController.View.EndEditing(true);
                    }
                }
                else
                {
                    NextTextField.BecomeFirstResponder();
                }

            });
            buttonSiguiente.TintColor = UIColor.White;

            toolbar.Items = new UIBarButtonItem[] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), buttonSiguiente };
            return toolbar;

            #endregion
        }
        #region TextField FORM (next)
        private static UITextField[] formTextfields;

        public static void TextFieldsForm(UITextField[] fields)
        {
            #region NextKey TextFields
            formTextfields = fields;

            foreach (var field in fields)
            {
                field.ShouldReturn += ShouldReturn;
            }
            #endregion
        }

        private static bool ShouldReturn(UITextField textField)
        {
            #region ShouldReturn Addkey
            int index = Array.IndexOf(formTextfields, textField);

            if (index > -1 && index < formTextfields.Length - 1)

            {

                formTextfields[index + 1].BecomeFirstResponder();
                return true;
            }
            else if (index == formTextfields.Length - 1)
            {
                formTextfields[index].ResignFirstResponder();
            }

            return false;
            #endregion
        }
        /// <summary>
        /// Shake animation to an certain view
        /// </summary>
        /// <param name="View"></param>
        public static void ViewShakerAnimation(UIView View)
        {
            var viewShaker = new ViewShaker.ViewShaker(View);
            viewShaker.Shake();
        }

        #endregion

        public static UIActivityIndicatorView SetLoadingIndicatorLabel(UIView View, UIColor Backgroundcolor)
        {
            #region Set Loading Indicator n' Label
            UIActivityIndicatorView LoadingIndicator;
            nfloat centerX = View.Frame.Width / 2;
            nfloat centerY = View.Frame.Height / 2;
            nfloat labelHeight = 22;
            nfloat labelWidth = View.Frame.Width - 20;

            LoadingIndicator = new UIActivityIndicatorView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Width + 40, UIScreen.MainScreen.Bounds.Size.Height + 40));
            LoadingIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
            LoadingIndicator.Color = UIColor.FromRGB(60, 60, 60);
            LoadingIndicator.Center = new CGPoint(UIScreen.MainScreen.Bounds.Size.Width / 2, (UIScreen.MainScreen.Bounds.Size.Height / 2));
            LoadingIndicator.BackgroundColor = Backgroundcolor;
            LoadingIndicator.HidesWhenStopped = true;

            View.AddSubview(LoadingIndicator);
            LoadingIndicator.StartAnimating();
            return LoadingIndicator;


            #endregion
        }
    }
}