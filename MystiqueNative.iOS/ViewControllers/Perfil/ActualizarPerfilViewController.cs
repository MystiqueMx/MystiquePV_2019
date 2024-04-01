using AudioToolbox;
using CoreAnimation;
using CoreGraphics;
using FFImageLoading;
using FFImageLoading.Transformations;
using Foundation;
using MystiqueNative.Helpers;
using MystiqueNative.iOS.Helpers;
using MystiqueNative.iOS.View;
using MystiqueNative.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using UIKit;

namespace MystiqueNative.iOS
{
    public partial class ActualizarPerfilViewController : UITableViewController, iAPRSuggestionsTextFieldDelegate
    {
        string auxbirth;
        string sexo;
        bool ErrorInput = false;
        bool registerSuccess = true;
        bool pendingToUpdate = false;

        UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

        UIDatePicker datePicker;
        UIImagePickerController imagePicker;
        byte[] profilePicture;
        string extension;
        LoadingOverlay loadPop;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            COLONIA.UserInteractionEnabled = false;
            UIPickerView PickerCiudades = new UIPickerView(new CGRect(0, 44, View.Bounds.Width, 216));

            //PATCH 21/05/2019
            if (MystiqueApp.Usuario.AuthMethod != AuthMethods.Email)
            {
                editPass2.Hidden = true;
                editPass.Hidden = true;
            }

            var PickerModelCiudades = new PickerModel(CiudadesHelper.Ciudades);
            PickerCiudades.Model = PickerModelCiudades;
            Ciudad.InputView = PickerCiudades;

            PickerModelCiudades.ValueChanged += async (sender, e) =>
            {
                COLONIA.UserInteractionEnabled = false;
                COLONIA.Text = string.Empty;
                if (PickerModelCiudades.SelectedValue == "Seleccione ciudad")
                {
                    Ciudad.Text = string.Empty;
                }
                else
                {
                    Ciudad.Text = PickerModelCiudades.SelectedValue;
                    await AppDelegate.Auth.ConsultarColonias(PickerModelCiudades.SelectedValue);
                }
                if (Ciudad.Text == CiudadesHelper.Ciudades[1] || Ciudad.Text == CiudadesHelper.Ciudades[2])
                {
                    COLONIA.UserInteractionEnabled = true;
                }
            };
            Ciudad.InputAccessoryView = toolbar;
            //SET BACKGROUND IMAGE TO TABLEVIEW //

            var backgroundimage = new UIImageView(View.Frame);
            backgroundimage.Image = UIImage.FromBundle("Fresco/Images/fondo_limpio.png");
            TableView.BackgroundView = backgroundimage;


            //PICKER COLONIAS MXLI
            timer = NSTimer.CreateRepeatingScheduledTimer(0.5, (_) =>
            {
                if (string.IsNullOrEmpty(Ciudad.Text))
                {
                    COLONIA.UserInteractionEnabled = false;
                    COLONIA.Text = string.Empty;
                }
                else
                {
                    //  COLONIA.UserInteractionEnabled = true;
                }

            });
            Ciudad.EditingChanged += (sender, e) =>
            {

                Console.WriteLine("EL TEXTO CAMBIO");
            };

            CreateDatePicker();
            namelabel.AdjustsFontSizeToFitWidth = true;
            editPhone.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 10;
            };

            var bounds2 = TableView.Bounds;
            loadPop = new LoadingOverlay(bounds2, "Actualizando Perfil...");
            Ciudad.Text = CiudadesHelper.IntToCiudades[AppDelegate.Auth.Usuario.CiudadAsInt];
            namelabel.Text = AppDelegate.Auth.Usuario.NombreCompleto;

            //***CLOSE KEYBOARD ON TAP OUTSIDE *** //
            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);

            UIPickerView pickerView = new UIPickerView(new CGRect(0, 44, View.Bounds.Width, 216));

            if (MystiqueApp.Usuario.AuthMethod != AuthMethods.Email)
            {
                EnableNextKeyForTextFields(new UITextField[] { editName, editPaterno, Ciudad, COLONIA, editBirth, editSex, editPhone });
            }
            else
            {
                EnableNextKeyForTextFields(new UITextField[] { editName, editPaterno, Ciudad, COLONIA, editBirth, editSex, editPhone, editPass, editPass2 });
            }

            var pickerModel = new PeopleModel(editSex);

            pickerView.Model = pickerModel;
            editSex.InputView = pickerView;

            AddDoneButton(editPhone, editPass);
            AddDoneButton(editSex, editPhone);
            AddDoneButton(COLONIA, editBirth);
        }

        private void Instance_OnConsultarColoniasFinished(object sender, ConsultarColoniasEventArgs e)
        {
            if (e.Success)
            {
                SetColonias(e.Colonias);
            }
            else
            {
                var Alert = UIAlertController.Create("Actualizar Perfil", "Error el en servidor", UIAlertControllerStyle.Alert);
                Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (Out) =>
                {
                    NavigationController.PopViewController(true);
                }));
                PresentViewController(Alert, true, null);
            }
        }



        APRTextFieldSuggestions suggestionsColonias;
        private void FillList(List<string> colonias)
        {
            COLONIA.BringSubviewToFront(View);
            suggestionsColonias = new APRTextFieldSuggestions();
            suggestionsColonias.suggestionRowHeight = 30;
            suggestionsColonias.suggestionFontSize = 13;
            suggestionsColonias.initializeSuggestions(COLONIA, colonias.ToArray());
        }

        private void Auth_OnActualizarPerfilFinished(object sender, ViewModels.ActualizarPerfilEventArgs e)
        {
            if (e.Success)
                if (e.Success)
                {
                    //preferences
                    var alert = UIAlertController.Create("Actualizar Perfil", "Sus datos se han actualizado correctamente", UIAlertControllerStyle.Alert);
                    var WalletAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (OK) =>
                    {
                        NavigationController?.PopViewController(true);
                    });
                    alert.AddAction(WalletAction);
                    alert.PreferredAction = WalletAction;

                    PresentViewController(alert, true, null);
                }
                else
                {
                    var alert = UIAlertController.Create("Actualizar Perfil", e.Message, UIAlertControllerStyle.Alert);
                    var WalletAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
                    alert.AddAction(WalletAction);
                    alert.PreferredAction = WalletAction;
                    PresentViewController(alert, true, null);
                }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!pendingToUpdate)
            {
                if (!string.IsNullOrEmpty(AppDelegate.Auth.ProfilePictureUrl))
                {
                    ImageService.Instance.LoadUrl(AppDelegate.Auth.ProfilePictureUrl).Transform(new CircleTransformation()).DownSample(height: 200).Into(ProfilePictureDetalle);
                }

                editName.Text = AppDelegate.Auth.Usuario.Nombre;
                editPaterno.Text = AppDelegate.Auth.Usuario.Paterno;
                editBirth.Text = AppDelegate.Auth.Usuario.FechaNacimientoConFormatoEspanyol;

                if (AppDelegate.Auth.Usuario.SexoAsInt == 0)
                {
                    editSex.Placeholder = "SEXO*";
                }
                else
                {
                    sexo = AppDelegate.Auth.Usuario.SexoAsString;
                }

                editSex.Text = sexo;
                editPhone.Text = AppDelegate.Auth.Usuario.Telefono;
                COLONIA.Text = AppDelegate.Auth.Usuario.Colonia;
            }

        }

        private void SetColonias(List<string> colonias)
        {
            UIPickerView PickerColonias = new UIPickerView(new CGRect(0, 44, View.Bounds.Width, 216));
            var PickerModelColonias = new PickerModel(colonias);
            PickerColonias.Model = PickerModelColonias;
            COLONIA.InputView = PickerColonias;

            PickerModelColonias.ValueChanged += delegate
            {
                COLONIA.Text = PickerModelColonias.SelectedValue;
            };

            COLONIA.UserInteractionEnabled = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!string.IsNullOrEmpty(Ciudad.Text))
            {
                BeginInvokeOnMainThread(async () =>
                {
                    await AppDelegate.Auth.ConsultarColonias(Ciudad.Text);
                });
            }

            AppDelegate.Auth.OnActualizarPerfilFinished += Auth_OnActualizarPerfilFinished;
            AuthViewModelV2.Instance.OnConsultarColoniasFinished += Instance_OnConsultarColoniasFinished;

        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            AppDelegate.Auth.OnActualizarPerfilFinished -= Auth_OnActualizarPerfilFinished;
            AuthViewModelV2.Instance.OnConsultarColoniasFinished -= Instance_OnConsultarColoniasFinished;
        }
        protected void AddDoneButton(UITextField uiTextField, UITextField NextTextfield)
        {

            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            UIBarButtonItem buttonSiguiente = new UIBarButtonItem("Siguiente", UIBarButtonItemStyle.Done, delegate
            {
                if (uiTextField == editBirth)
                {
                    NSDateFormatter dateFormatter = new NSDateFormatter();
                    NSDateFormatter dateFormatter2 = new NSDateFormatter();
                    dateFormatter.DateFormat = "dd/MM/yyyy";

                    string birtdayselect = dateFormatter.ToString(datePicker.Date);
                    auxbirth = dateFormatter2.ToString(datePicker.Date);
                    editBirth.Text = birtdayselect;
                }
                if (uiTextField == editSex)
                {
                    if (string.IsNullOrEmpty(editSex.Text))
                    {
                        editSex.Text = "Masculino";
                    }
                }

                if (uiTextField == Ciudad)
                {
                    if (!string.IsNullOrEmpty(Ciudad.Text))
                    {
                        if (Ciudad.Text == "Seleccione ciudad")
                        {
                            Ciudad.Text = string.Empty;
                        }
                    }
                    else
                    {
                        editBirth.BecomeFirstResponder();
                    }
                }


                NextTextfield.BecomeFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                buttonSiguiente
            };

            uiTextField.InputAccessoryView = toolbar;
        }


        private UITextField[] formTextfields;
        private NSTimer timer;

        //PUT TXTFIELD ON ARRAY
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
            editPass2.ResignFirstResponder();
        }

        public void CreateDatePicker()
        {
            //Date Picker
            datePicker = new UIDatePicker(new CGRect(0, 44, View.Bounds.Width, 216));
            datePicker.Mode = UIDatePickerMode.Date;
            datePicker.MaximumDate = NSDate.Now;
            datePicker.Locale = NSLocale.FromLocaleIdentifier("es_MX");

            editBirth.InputView = datePicker;
            AddDoneButton(editBirth, editSex);
        }

        #region CAMERA 
        partial void CameraButton_TouchUpInside(UIButton sender)
        {
            Camera.TakePicture(this, (obj) =>
            {
                var photo = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
                var meta = obj.ValueForKey(new NSString("UIImagePickerControllerMediaMetadata")) as NSDictionary;

                if (photo != null)
                {
                    SetToImage(photo);
                }

            }); ;
        }

        partial void GalleryButton_TouchUpInside(UIButton sender)
        {
            imagePicker = new UIImagePickerController();
            imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;
            PresentViewControllerAsync(imagePicker, true);
        }

        public void SetToImage(UIImage imagep)
        {
            var compressImage = imagep.OrientationFix().ZipToJpeg(1000);

            InvokeInBackground(() =>
            {
                profilePicture = compressImage.ToArray();
                extension = ".jpg";
            });

            ProfilePictureDetalle.Image = ToCircleTransformation.ToRounded(new UIImage(data: compressImage, scale: 1), 0f, 1f, 1f, 0d, null);
        }

        void Handle_Canceled(object sender, EventArgs e)
        {
            Console.WriteLine("picker cancelled");
            imagePicker.DismissModalViewController(true);
        }
        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            pendingToUpdate = true;
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                    Console.WriteLine("Video selected");
                    break;
            }

            NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceURL")] as NSUrl;
            if (referenceURL != null)
                Console.WriteLine("Url:" + referenceURL.ToString());


            if (isImage)
            {
                // get the original image
                if (e.Info[UIImagePickerController.OriginalImage] is UIImage originalImage)
                {
                    SetToImage(originalImage);
                }
                else
                {
                    NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
                    if (mediaURL != null)
                    {
                        Console.WriteLine(mediaURL.ToString());
                    }
                }
                imagePicker.DismissModalViewController(true);
            }
        }
        #endregion
        private UIViewController GetVisibleViewController(UIViewController controller = null)
        {
            controller = controller ?? UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (controller.PresentedViewController == null)
                return controller;

            if (controller.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)controller.PresentedViewController).VisibleViewController;
            }

            if (controller.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)controller.PresentedViewController).SelectedViewController;
            }

            return GetVisibleViewController(controller.PresentedViewController);
        }

        public void suggestionTextField_userSelectedItem(int itemIndex, string itemVal)
        {
            COLONIA.Text = itemVal;
        }

        public ActualizarPerfilViewController(IntPtr handle) : base(handle)
        {
        }

        partial void GuardarButton_TouchUpInside(UIButton sender)
        {
            registerSuccess = true;
            ErrorInput = false;

            var border = new CALayer();
            nfloat width = 1.5f;
            editName.Layer.BorderWidth = 0;
            editPaterno.Layer.BorderWidth = 0;
            editBirth.Layer.BorderWidth = 0;
            editSex.Layer.BorderWidth = 0;
            editPhone.Layer.BorderWidth = 0;
            editPass.Layer.BorderWidth = 0;
            editPass2.Layer.BorderWidth = 0;

            if (string.IsNullOrEmpty(editName.Text) || !ValidatorHelper.IsValidName(editName.Text))
            {
                var borderName = new CALayer();
                borderName.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                borderName.Frame = new CoreGraphics.CGRect(0, editName.Frame.Size.Height - width,
                editName.Frame.Size.Width, editName.Frame.Size.Height);
                borderName.BorderWidth = width;
                editName.Layer.AddSublayer(borderName);
                editName.Layer.MasksToBounds = true;

                SystemSound.Vibrate.PlaySystemSound();
                ErrorInput = true;
                registerSuccess = false;

            }
            if (string.IsNullOrEmpty(editPaterno.Text) || !ValidatorHelper.IsValidName(editPaterno.Text))
            {
                //    Paterno.Layer.BorderColor = UIColor.Red.CGColor;
                //    Paterno.Layer.BorderWidth = 2;
                var borderPaterno = new CALayer();
                borderPaterno.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                borderPaterno.Frame = new CoreGraphics.CGRect(0, editPaterno.Frame.Size.Height - width,
                editPaterno.Frame.Size.Width, editPaterno.Frame.Size.Height);
                borderPaterno.BorderWidth = width;
                editPaterno.Layer.AddSublayer(borderPaterno);
                editPaterno.Layer.MasksToBounds = true;

                SystemSound.Vibrate.PlaySystemSound();
                ErrorInput = true;
                registerSuccess = false;
            }
            //if (!string.IsNullOrEmpty(editMaterno.Text))
            //{
            //    if (!ValidatorHelper.IsValidName(editMaterno.Text))
            //    {
            //        //editMaterno.Layer.BorderColor = UIColor.Red.CGColor;
            //        //editMaterno.Layer.BorderWidth = 2;
            //        var bordereditMaterno = new CALayer();
            //        bordereditMaterno.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
            //        bordereditMaterno.Frame = new CoreGraphics.CGRect(0, editMaterno.Frame.Size.Height - width,
            //        editMaterno.Frame.Size.Width, editMaterno.Frame.Size.Height);
            //        bordereditMaterno.BorderWidth = width;
            //        editMaterno.Layer.AddSublayer(bordereditMaterno);
            //        editMaterno.Layer.MasksToBounds = true;

            //        SystemSound.Vibrate.PlaySystemSound();
            //        ErrorInput = true;
            //        registerSuccess = false;
            //    }
            //}

            if (string.IsNullOrEmpty(editBirth.Text) || !ValidatorHelper.IsValidBirthday(editBirth.Text))
            {
                //        editBirth.Layer.BorderColor = UIColor.Red.CGColor;
                //        editBirth.Layer.BorderWidth = 2;
                var borderDate = new CALayer();
                borderDate.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                borderDate.Frame = new CoreGraphics.CGRect(0, editBirth.Frame.Size.Height - width,
                editBirth.Frame.Size.Width, editBirth.Frame.Size.Height);
                borderDate.BorderWidth = width;
                editBirth.Layer.AddSublayer(borderDate);
                editBirth.Layer.MasksToBounds = true;

                SystemSound.Vibrate.PlaySystemSound();
                ErrorInput = true;
                registerSuccess = false;

            }
            if (string.IsNullOrEmpty(editSex.Text))
            {
                var borderDate = new CALayer();
                borderDate.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                borderDate.Frame = new CoreGraphics.CGRect(0, editSex.Frame.Size.Height - width,
                editSex.Frame.Size.Width, editSex.Frame.Size.Height);
                borderDate.BorderWidth = width;
                editSex.Layer.AddSublayer(borderDate);
                editSex.Layer.MasksToBounds = true;

                SystemSound.Vibrate.PlaySystemSound();
                ErrorInput = true;
                registerSuccess = false;

            }

            if (string.IsNullOrEmpty(COLONIA.Text))
            {
                var borderDate = new CALayer();
                borderDate.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                borderDate.Frame = new CoreGraphics.CGRect(0, COLONIA.Frame.Size.Height - width,
                COLONIA.Frame.Size.Width, COLONIA.Frame.Size.Height);
                borderDate.BorderWidth = width;
                COLONIA.Layer.AddSublayer(borderDate);
                COLONIA.Layer.MasksToBounds = true;

                SystemSound.Vibrate.PlaySystemSound();
                ErrorInput = true;
                registerSuccess = false;

            }

            if (string.IsNullOrEmpty(editPhone.Text) || !ValidatorHelper.IsValidPhone(editPhone.Text))
            {
                var borderTel = new CALayer();
                borderTel.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                borderTel.Frame = new CoreGraphics.CGRect(0, editPhone.Frame.Size.Height - width,
                editPhone.Frame.Size.Width, editPhone.Frame.Size.Height);
                borderTel.BorderWidth = width;
                editPhone.Layer.AddSublayer(borderTel);
                editPhone.Layer.MasksToBounds = true;

                SystemSound.Vibrate.PlaySystemSound();
                ErrorInput = true;
                registerSuccess = false;

            }

            if (!string.IsNullOrEmpty(editPass.Text) && !string.IsNullOrEmpty(editPass2.Text))
            {
                if (string.IsNullOrEmpty(editPass.Text) || !ValidatorHelper.IsValidPassword(editPass.Text))
                {
                    var borderPass = new CALayer();
                    borderPass.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                    borderPass.Frame = new CoreGraphics.CGRect(0, editPass.Frame.Size.Height - width,
                    editPass.Frame.Size.Width, editPass.Frame.Size.Height);
                    borderPass.BorderWidth = width;
                    editPass.Layer.AddSublayer(borderPass);
                    editPass.Layer.MasksToBounds = true;

                    var borderPass2 = new CALayer();
                    borderPass2.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                    borderPass2.Frame = new CoreGraphics.CGRect(0, editPass2.Frame.Size.Height - width,
                    editPass2.Frame.Size.Width, editPass2.Frame.Size.Height);
                    borderPass2.BorderWidth = width;
                    editPass2.Layer.AddSublayer(borderPass2);
                    editPass2.Layer.MasksToBounds = true;
                    SystemSound.Vibrate.PlaySystemSound();
                    ErrorInput = true;
                    registerSuccess = false;
                }
                else
                if (string.IsNullOrEmpty(editPass2.Text) || !ValidatorHelper.IsValidPassword(editPass2.Text))
                {
                    var borderPass2 = new CALayer();
                    borderPass2.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                    borderPass2.Frame = new CoreGraphics.CGRect(0, editPass2.Frame.Size.Height - width,
                    editPass2.Frame.Size.Width, editPass2.Frame.Size.Height);
                    borderPass2.BorderWidth = width;
                    editPass2.Layer.AddSublayer(borderPass2);
                    editPass2.Layer.MasksToBounds = true;

                    SystemSound.Vibrate.PlaySystemSound();
                    ErrorInput = true;
                    registerSuccess = false;
                }
                else
            if (editPass.Text == editPass2.Text)
                {

                }
                else
                {
                    var okAlertController = UIAlertController.Create("Contraseña", "Las contraseñas no coinciden", UIAlertControllerStyle.Alert);
                    okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    PresentViewController(okAlertController, true, null);

                    ErrorInput = true;
                    registerSuccess = false;
                }
            }



            if (ErrorInput == true)
            {

                var okAlertController = UIAlertController.Create("Registro", "Favor de llenar todos los campos", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(okAlertController, true, null);

                registerSuccess = true;
                return;
            }

            if (registerSuccess == true)
            {

                string nombre = editName.Text;
                string paterno = editPaterno.Text;
                string colonia = COLONIA.Text;
                string fechanat = editBirth.Text;
                string sexo = editSex.Text;
                string telefono = editPhone.Text;
                string pass = editPass.Text;



                if (string.IsNullOrEmpty(editPass.Text) && string.IsNullOrEmpty(editPass2.Text))
                {
                    if (profilePicture != null && profilePicture.Length > 0 && !string.IsNullOrEmpty(extension))
                    {
                        View.Add(loadPop);
                        AppDelegate.Auth.ActualizarPerfil(nombre, paterno, string.Empty, fechanat, sexo, telefono, colonia, null, profilePicture, extension);
                    }
                    else
                    {
                        View.Add(loadPop);
                        AppDelegate.Auth.ActualizarPerfil(nombre, paterno, string.Empty, fechanat, sexo, telefono, colonia, null);
                    }

                }
                else
                {
                    View.Add(loadPop);
                    AppDelegate.Auth.ActualizarPerfil(nombre, paterno, string.Empty, fechanat, sexo, telefono, colonia, pass);
                }



            }

        }
    }

}