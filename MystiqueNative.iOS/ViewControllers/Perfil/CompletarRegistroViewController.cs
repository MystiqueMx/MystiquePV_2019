using AudioToolbox;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MystiqueNative.Helpers;
using MystiqueNative.iOS.View;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class CompletarRegistroViewController : UITableViewController
    {
        #region DECLARACIONES
        UIDatePicker datePicker;
        LoadingOverlay loadPop;
        private UITextField[] formTextfields;
        private NSTimer timer;
        #endregion

        #region CONSTRUCTOR
        public CompletarRegistroViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion

        #region LIFECYCLE

        #region View Did Load
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            NSDateFormatter dateFormat = new NSDateFormatter();
            dateFormat.DateFormat = "dd/MM/yyyy";
            //ON ACTUALIZAR PERFIL FINISH
            #region Llenar datos

            #endregion

            #region Show Fields
            if (!string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.CiudadId) && AuthViewModelV2.Instance.Usuario.CiudadAsInt > 0)
            {
                CiudadView.Hidden = true;
                CiudadTextField.Text = CiudadesHelper.IntToCiudades[AppDelegate.Auth.Usuario.CiudadAsInt];
            }
            if (!string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Colonia))
            {
                ColoniaView.Hidden = true;
                ColoniaTextField.Text = AuthViewModelV2.Instance.Usuario.Colonia;
            }
            if (!string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.FechaNacimiento))
            {
                FechaNacimientoView.Hidden = true;
                FechaNacimientoTextField.Text = AuthViewModelV2.Instance.Usuario.FechaNacimiento;
            }
            if (!string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Sexo) && AuthViewModelV2.Instance.Usuario.SexoAsInt > 0)
            {
                SexoView.Hidden = true;
                SexoTextField.Text = AuthViewModelV2.Instance.Usuario.Sexo;
            }
            if (!string.IsNullOrEmpty(AuthViewModelV2.Instance.Usuario.Telefono))
            {
                TelefonoView.Hidden = true;
                TelefonoTextField.Text = AuthViewModelV2.Instance.Usuario.Telefono;
            }
            #endregion

            #region Set Textfields
            ColoniaTextField.UserInteractionEnabled = false;
            UIToolbar ToolbarColonias = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            UIPickerView pickerView = new UIPickerView(new CGRect(0, 44, this.View.Bounds.Width, 216));

            TelefonoTextField.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 10;
            };

            ColoniaTextField.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 50;
            };

            CreateDatePicker();
            //PICKERVIEW SEXO
            var pickerModel = new PeopleModel(SexoTextField);
            pickerView.Model = pickerModel;
            SexoTextField.InputView = pickerView;

            //PICKERVIEW CIUDADES
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            UIPickerView PickerCiudades = new UIPickerView(new CGRect(0, 44, this.View.Bounds.Width, 216));
            var PickerModelCiudades = new PickerModel(CiudadesHelper.Ciudades);
            PickerCiudades.Model = PickerModelCiudades;
            CiudadTextField.InputView = PickerCiudades;

            CiudadTextField.InputAccessoryView = toolbar;
            PickerModelCiudades.ValueChanged += (sender, e) =>
            {
                ColoniaTextField.UserInteractionEnabled = false;
                CiudadTextField.Text = string.Empty;
                if (PickerModelCiudades.SelectedValue == "Seleccione ciudad")
                {
                    CiudadTextField.Text = string.Empty;
                }
                else
                {
                    CiudadTextField.Text = PickerModelCiudades.SelectedValue;
                    BeginInvokeOnMainThread(async () =>
                      {
                          await AuthViewModelV2.Instance.ConsultarColonias(PickerModelCiudades.SelectedValue);
                      });
                }
                if (CiudadTextField.Text == CiudadesHelper.Ciudades[1] || CiudadTextField.Text == CiudadesHelper.Ciudades[2])
                {
                    ColoniaTextField.UserInteractionEnabled = true;
                }
            };

            datePicker.ValueChanged += (sender, e) =>
            {
                FechaNacimientoTextField.Text = dateFormat.ToString(datePicker.Date);
            };
            //EnableNextKeyForTextFields(new UITextField[]
            //{
            //    CiudadTextField,
            //    ColoniaTextField,
            //    FechaNacimientoTextField,
            //    SexoTextField,
            //    TelefonoTextField
            //});

            // AddDoneButton(SexoTextField, TelefonoTextField);
            // AddDoneButton(ColoniaTextField, FechaNacimientoTextField);
            #endregion

            AddDoneButton(FechaNacimientoTextField);
            AddDoneButton(SexoTextField);
            AddDoneButton(ColoniaTextField);
            AddDoneButton(TelefonoTextField);

            #region Gestures
            View.AddGestureRecognizer(new UITapGestureRecognizer(() => View.EndEditing(true)));

            #endregion
        }



        #endregion

        #region View Did Appear
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AuthViewModelV2.Instance.OnConsultarColoniasFinished += Instance_OnConsultarColoniasFinished;
            AuthViewModelV2.Instance.OnActualizarPerfilFinished += Instance_OnActualizarPerfilFinished;
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
            AuthViewModelV2.Instance.OnConsultarColoniasFinished -= Instance_OnConsultarColoniasFinished;
            AuthViewModelV2.Instance.OnActualizarPerfilFinished -= Instance_OnActualizarPerfilFinished;
        }
        #endregion

        #endregion

        #region METODOS INTERNOS

        #region On Consultar Colonias Finish

        #endregion

        #region Create Date Picker
        public void CreateDatePicker()
        {
            //Date Picker
            datePicker = new UIDatePicker(new CGRect(0, 44, this.View.Bounds.Width, 216));
            datePicker.Mode = UIDatePickerMode.Date;
            datePicker.MaximumDate = NSDate.Now;
            datePicker.Locale = NSLocale.FromLocaleIdentifier("es_MX");

            FechaNacimientoTextField.InputView = datePicker;
            //  AddDoneButton(FechaNacimientoTextField, SexoTextField);
        }
        #endregion

        #region Add Done Button
        public void AddDoneButton(UITextField uiTextField)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            UIBarButtonItem buttonSiguiente = new UIBarButtonItem("OK", UIBarButtonItemStyle.Done, delegate
            {
                if (uiTextField == FechaNacimientoTextField)
                {
                    NSDateFormatter dateFormatter = new NSDateFormatter();
                    dateFormatter.DateFormat = "dd/MM/yyyy";
                    string birtdayselect = dateFormatter.ToString(datePicker.Date);
                    FechaNacimientoTextField.Text = birtdayselect;
                }
                if (uiTextField == SexoTextField)
                {
                    if (string.IsNullOrEmpty(SexoTextField.Text))
                    {
                        SexoTextField.Text = "Masculino";
                    }
                }
             

                uiTextField.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                     new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                buttonSiguiente,
            };

            uiTextField.InputAccessoryView = toolbar;
        }
        #endregion

        #region Set Colonias
        private void SetColonias(List<string> colonias)
        {
            UIPickerView PickerColonias = new UIPickerView(new CGRect(0, 44, this.View.Bounds.Width, 216));
            var PickerModelColonias = new PickerModel(colonias);
            PickerColonias.Model = PickerModelColonias;
            ColoniaTextField.InputView = PickerColonias;

            PickerModelColonias.ValueChanged += delegate
            {
                ColoniaTextField.Text = PickerModelColonias.SelectedValue;
            };

            ColoniaTextField.UserInteractionEnabled = true;
        }
        #endregion

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
            TelefonoTextField.ResignFirstResponder();
            //RegistroTable.SetContentOffset(CGPoint.Empty, true);
        }
        #endregion

        #region Validates Fields
        public bool ValidateFields()
        {
            bool Success = true;
            bool ErrorInput = false;


            #region validations
            if (string.IsNullOrEmpty(FechaNacimientoTextField.Text) || ValidatorHelper.IsValidName(FechaNacimientoTextField.Text))
            {
                PrintErrorTextField(FechaNacimientoTextField);
                ErrorInput = true;
                Success = false;
            }
            if (string.IsNullOrEmpty(TelefonoTextField.Text) || ValidatorHelper.IsValidName(TelefonoTextField.Text))
            {
                PrintErrorTextField(TelefonoTextField);
                ErrorInput = true;
                Success = false;
            }
            if (string.IsNullOrEmpty(CiudadTextField.Text))
            {
                PrintErrorTextField(CiudadTextField);
                ErrorInput = true;
                Success = false;
            }
            if (string.IsNullOrEmpty(ColoniaTextField.Text))
            {
                PrintErrorTextField(ColoniaTextField);
                ErrorInput = true;
                Success = false;
            }
            return Success;
            #endregion
        }
        #region Print Error On Textfield
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
        #endregion

        #endregion
        #endregion

        #region EVENTOS

        private void Instance_OnActualizarPerfilFinished(object sender, ActualizarPerfilEventArgs e)
        {
            if (e.Success)
            {
                var Alert = UIAlertController.Create("Inicio de sesión", "Sus datos se han actualizado correctamente", UIAlertControllerStyle.Alert);
                Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                {
                    NavigationController?.PopViewController(true);
                }));
                PresentViewController(Alert, true, null);
            }
            else
            {
                var Alert = UIAlertController.Create("Inicio de sesión", "Error al contactar el servidor", UIAlertControllerStyle.Alert);
                Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                {
                    NavigationController?.PopViewController(true);
                }));
                PresentViewController(Alert, true, null);
            }
        }
        private void Instance_OnConsultarColoniasFinished(object sender, ConsultarColoniasEventArgs e)
        {
            if (e.Success)
            {
                SetColonias(e.Colonias);
            }
            else
            {
                var Alert = UIAlertController.Create("Completar Registro", "Error el en servidor", UIAlertControllerStyle.Alert);
                Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (Out) =>
                {
                    NavigationController?.PopViewController(true);
                }));
                PresentViewController(Alert, true, null);
            }
        }
        partial void GuardarButton_TouchUpInside(UIButton sender)
        {
            #region reset layers
            FechaNacimientoTextField.Layer.BorderWidth = 0;
            SexoTextField.Layer.BorderWidth = 0;
            TelefonoTextField.Layer.BorderWidth = 0;
            ColoniaTextField.Layer.BorderWidth = 0;
            #endregion

            this.TableView.ContentOffset = new CGPoint(0, 0 - this.TableView.ContentInset.Top);
            View.EndEditing(true);
            var user = AuthViewModelV2.Instance.Usuario;
            ValidateFields();
            if (ValidateFields())
            {
                AuthViewModelV2.Instance.ActualizarPerfil
                    (user.Nombre,
                     user.Paterno,
                     user.Materno,
                     FechaNacimientoTextField.Text,
                     SexoTextField.Text,
                     TelefonoTextField.Text,
                     ColoniaTextField.Text,
                     string.Empty);
            }
            else
            {
                SystemSound.Vibrate.PlaySystemSound();
                var Alert = UIAlertController.Create("Inicio de sesión", "Alguno de los campos esta vacío o es incorrecto", UIAlertControllerStyle.Alert);
                Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(Alert, true, null);
            }
        }
        #endregion
    }
}