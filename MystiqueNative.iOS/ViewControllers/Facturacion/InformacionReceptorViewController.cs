using AudioToolbox;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MystiqueNative.iOS.View;
using System;
using System.Drawing;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class InformacionReceptorViewController : UITableViewController
    {
        #region DECLARACIONES
        internal string id;
        internal string razonsocial;
        internal string rfc;
        internal string correo;
        internal string codigopostal;
        internal string direccion;
        internal string cfdi;
        bool CanSave = true;
        internal int indexpath;
        private UITextField[] formTextfields;
        #endregion

        #region CONSTRUCTOR
        public InformacionReceptorViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion

        #region LIFECYCLE

        #region View Did Load
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RazonSocialField.Text = razonsocial;
            RFCField.Text = rfc;
            CorreoField.Text = correo;
            CodigoPostalField.Text = codigopostal;
            DireccionField.Text = direccion;
            CFDIField.Text = cfdi;
            #region Set CFDI FIELD
            UIPickerView CFDIPicker = new UIPickerView(new CGRect(0, 44, this.View.Bounds.Width, 216));

            var CFDIPickerModel = new PickerModel(ViewModels.FacturacionViewModel.Instance.UsosCfdiAsStringList());
            CFDIPicker.Model = CFDIPickerModel;
            CFDIField.InputView = CFDIPicker;
            CFDIPickerModel.ValueChanged += (sender, e) =>
            {
                CFDIField.Text = CFDIPickerModel.SelectedValue;
            };
            AddToolbar(CFDIField);
            #endregion

            EnableNextKeyForTextFields(new UITextField[]
            {
                RazonSocialField,
                RFCField,
                CorreoField,
                CodigoPostalField,
                DireccionField,
                CFDIField
            });

            #region Gestures
            View.AddGestureRecognizer(new UITapGestureRecognizer(() => View.EndEditing(true)));
            #endregion
        }

        #endregion

        #region View Did Appear
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        #endregion

        #region View Will Appear
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        #endregion

        #region View Did Disappear
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        #endregion

        #endregion

        #region METODOS INTERNOS
        private bool ValidarCampos()
        {
            ResetTextFields(new UITextField[] { RazonSocialField, RFCField, CorreoField, CodigoPostalField, DireccionField, CFDIField });
            CanSave = true;

            if (string.IsNullOrEmpty(RazonSocialField.Text))
            {
                var viewShaker = new ViewShaker.ViewShaker(RazonSocialField);
                viewShaker.Shake();
                PrintErrorTextField(RazonSocialField);
                CanSave = false;
            }
            if (string.IsNullOrEmpty(RFCField.Text))
            {
                var viewShaker = new ViewShaker.ViewShaker(RFCField);
                viewShaker.Shake();
                PrintErrorTextField(RFCField);
                CanSave = false;
            }
            if (string.IsNullOrEmpty(CorreoField.Text))
            {
                var viewShaker = new ViewShaker.ViewShaker(CorreoField);
                viewShaker.Shake();
                PrintErrorTextField(CorreoField);
                CanSave = false;
            }
            if (string.IsNullOrEmpty(CodigoPostalField.Text))
            {
                var viewShaker = new ViewShaker.ViewShaker(CodigoPostalField);
                viewShaker.Shake();
                PrintErrorTextField(CodigoPostalField);
                CanSave = false;
            }
            if (string.IsNullOrEmpty(DireccionField.Text))
            {
                var viewShaker = new ViewShaker.ViewShaker(DireccionField);
                viewShaker.Shake();
                PrintErrorTextField(DireccionField);
                CanSave = false;
            }
            if (string.IsNullOrEmpty(CFDIField.Text))
            {
                var viewShaker = new ViewShaker.ViewShaker(CFDIField);
                viewShaker.Shake();
                PrintErrorTextField(CFDIField);
                CanSave = false;
            }
            if (!CanSave)
            {
                SystemSound.Vibrate.PlaySystemSound();
            }
            return CanSave;
        }

        private void PrintErrorTextField(UITextField TextField)
        {
            nfloat width = 1.5f;
            var borderName = new CALayer();
            borderName.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
            borderName.Frame = new CoreGraphics.CGRect(0, TextField.Frame.Size.Height - width,
            TextField.Frame.Size.Width, TextField.Frame.Size.Height);
            borderName.BorderWidth = width;
            TextField.Layer.AddSublayer(borderName);
            TextField.Layer.MasksToBounds = true;
        }

        private void ResetTextFields(UITextField[] TextFields)
        {
            foreach (var item in TextFields)
            {
                nfloat width = 1.5f;
                var borderName = new CALayer();
                borderName.BorderColor = UIColor.FromRGB(200,200,200).CGColor;
                borderName.Frame = new CoreGraphics.CGRect(0, item.Frame.Size.Height - width,
                item.Frame.Size.Width, item.Frame.Size.Height);
                borderName.BorderWidth = width;
                item.Layer.AddSublayer(borderName);
                item.Layer.MasksToBounds = true;
            }
        }

        private void AddToolbar(UnderlineUITextfield TextField)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            UIBarButtonItem buttonSiguiente = new UIBarButtonItem("OK ", UIBarButtonItemStyle.Done, delegate
            {
                View.EndEditing(true);
            });
            toolbar.Items = new UIBarButtonItem[] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), buttonSiguiente };
            TextField.InputAccessoryView = toolbar;
        }

        #region Order TextFields Next
        protected void EnableNextKeyForTextFields(UITextField[] fields)
        {
            formTextfields = fields;

            foreach (var field in fields)
            {
                field.ShouldReturn += ShouldReturn;
            }
        }

        //KEYBOARD KEY RETURN
        private bool ShouldReturn(UITextField textField)
        {
            int index = Array.IndexOf(formTextfields, textField);

            if (index > -1 && index < formTextfields.Length - 1)

            {

                formTextfields[index + 1].BecomeFirstResponder();
                return true;
            }
            else if (index == formTextfields.Length - 1)
            {
                formTextfields[index].ResignFirstResponder();
                FormFinished();
            }

            return false;
        }

        //WHEN IS THE LAST TXTFIELD
        protected virtual void FormFinished()
        {
            CFDIField.ResignFirstResponder();
            //RegistroTable.SetContentOffset(CGPoint.Empty, true);
        }
        #endregion

        #endregion

        #region EVENTOS
        partial void GuardarReceptor_TouchUpInside(UIButton sender)
        {
            ValidarCampos();
            View.EndEditing(true);
        }

        #endregion

    }
}