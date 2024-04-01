using CoreGraphics;
using Foundation;
using MystiqueNative.iOS.View;
using System;
using System.Drawing;
using UIKit;
using MystiqueNative.iOS.Helpers;
using System.ComponentModel;
using CoreAnimation;
using AudioToolbox;
using MystiqueNative.Helpers;
using FFImageLoading.Transformations;
using FFImageLoading;

namespace MystiqueNative.iOS
{
    public partial class EditarPerfilViewController : UITableViewController
    {
        string auxbirth;
        bool ErrorInput = false;
        bool registerSuccess = true;
        UIDatePicker datePicker;
        UIImagePickerController imagePicker;
        byte[] profilePicture;
        string extension;
        LoadingOverlay loadPop;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
           
            


            CreateDatePicker();

            editPhone.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 10;
            };


            COLONIA.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 50;
            };
            var bounds2 = UIScreen.MainScreen.Bounds;
            loadPop = new LoadingOverlay(bounds2, "Actualizando Perfil...");

            AppDelegate.Auth.PropertyChanged += Auth_PropertyChanged;
            if (AppDelegate.Auth.Usuario != null)
            {
                if (AppDelegate.Auth.LoggedStatus)
                {

                    editName.Text = AppDelegate.Auth.Usuario.Nombre;
                    editPaterno.Text = AppDelegate.Auth.Usuario.Paterno;
                    editMaterno.Text = AppDelegate.Auth.Usuario.Materno;
                    editBirth.Text = AppDelegate.Auth.Usuario.FechaNacimientoConFormatoEspanyol;
                    editSex.Text = AppDelegate.Auth.Usuario.SexoAsString;
                    editPhone.Text = AppDelegate.Auth.Usuario.Telefono;
                    namelabel.Text = AppDelegate.Auth.Usuario.NombreCompleto;
                    COLONIA.Text = AppDelegate.Auth.Usuario.Colonia;


                }
            }

            //***CLOSE KEYBOARD ON TAP OUTSIDE *** //
            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);

            UIPickerView pickerView = new UIPickerView(new CGRect(0, 44, this.View.Bounds.Width, 216));

            EnableNextKeyForTextFields(new UITextField[] { editName,editPaterno, editMaterno,COLONIA,editBirth,editSex, editPhone,editPass
            ,editPass2 });

            var pickerModel = new PeopleModel(editSex);

            pickerView.Model = pickerModel;

            editSex.InputView = pickerView;

            AddDoneButton(editPhone, editPass);
            AddDoneButton(editSex, editPhone);

        }

        private void Auth_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProfileEditStatus")
            {
                if (AppDelegate.Auth.ProfileEditStatus)
                {
                    BeginInvokeOnMainThread(() =>
                    {

                        View.EndEditing(true);
                        if (string.IsNullOrEmpty(editPass.Text) && string.IsNullOrEmpty(editPass2.Text))
                        {
                            PreferencesHelper.UpdatePassword(editPass.Text);
                        }

                        NavigationController?.PopViewController(animated: true);
                    });
                }
                else
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        if (!string.IsNullOrEmpty(AppDelegate.Auth.ErrorMessage))
                        {

                            var okAlertController = UIAlertController.Create("Actualizar Perfil", AppDelegate.Auth.ErrorMessage, UIAlertControllerStyle.Alert);
                            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                            PresentViewController(okAlertController, true, null);

                            AppDelegate.Auth.ErrorMessage = string.Empty;

                        }
                    });

                }

            }
            if (e.PropertyName == "IsBusy")
            {
                if (AppDelegate.Auth.IsBusy)
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        this.TableView.ContentOffset = new CGPoint(0, 0 - this.TableView.ContentInset.Top);
                        View.EndEditing(true);
                        View.Add(loadPop);

                    });

                }
                else
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        loadPop.Hide();
                    });
                }
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (AppDelegate.Auth.Usuario != null)
            {
                if (AppDelegate.Auth.LoggedStatus)
                {
                    editName.Text = AppDelegate.Auth.Usuario.Nombre;
                    editPaterno.Text = AppDelegate.Auth.Usuario.Paterno;
                    editMaterno.Text = AppDelegate.Auth.Usuario.Materno;
                    editBirth.Text = AppDelegate.Auth.Usuario.FechaNacimientoConFormatoEspanyol;
                    editSex.Text = AppDelegate.Auth.Usuario.SexoAsString;
                    editPhone.Text = AppDelegate.Auth.Usuario.Telefono;
                    COLONIA.Text = AppDelegate.Auth.Usuario.Colonia;
                    if (!string.IsNullOrEmpty(AppDelegate.Auth.ProfilePictureUrl))
                    {
                        FFImageLoading.ImageService.Instance.LoadUrl(AppDelegate.Auth.ProfilePictureUrl).Transform(new CircleTransformation()).DownSample(height: 200).Into(ProfilePictureDetalle);
                    }
                  

                }
            }


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
                    //  string x = DateTime.Parse(editBirth.Text).ToString("dd/MM/yyyy");

                    dateFormatter.DateFormat = "dd/MM/yyyy";

                    string birtdayselect = dateFormatter.ToString(datePicker.Date);
                    auxbirth = dateFormatter2.ToString(datePicker.Date);
                    editBirth.Text = birtdayselect;
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
            //RegistroTable.SetContentOffset(CGPoint.Empty, true);
        }

        public void CreateDatePicker()
        {
            //Date Picker
            datePicker = new UIDatePicker(new CGRect(0, 44, this.View.Bounds.Width, 216));
            datePicker.Mode = UIDatePickerMode.Date;
            datePicker.MaximumDate = NSDate.Now;
            datePicker.Locale = NSLocale.FromLocaleIdentifier("es_MX");

            editBirth.InputView = datePicker;
            AddDoneButton(editBirth, editSex);
        }



        public EditarPerfilViewController(IntPtr handle) : base(handle)
        {
        }

        partial void UpdateSaveButton_Activated(UIBarButtonItem sender)
        {
            registerSuccess = true;
            ErrorInput = false;

            var border = new CALayer();
            nfloat width = 1.5f;


            editName.Layer.BorderWidth = 0;
            editPaterno.Layer.BorderWidth = 0;
            editMaterno.Layer.BorderWidth = 0;
            editBirth.Layer.BorderWidth = 0;
            editSex.Layer.BorderWidth = 0;
            editPhone.Layer.BorderWidth = 0;
            editPass.Layer.BorderWidth = 0;
            editPass2.Layer.BorderWidth = 0;

            if (string.IsNullOrEmpty(editName.Text) || !ValidatorHelper.IsValidName(editName.Text))
            {
                //entryName.Layer.BorderColor = UIColor.Red.CGColor;
                //entryName.Layer.BorderWidth = 2;
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
            if (!string.IsNullOrEmpty(editMaterno.Text))
            {
                if (!ValidatorHelper.IsValidName(editMaterno.Text))
                {
                    //editMaterno.Layer.BorderColor = UIColor.Red.CGColor;
                    //editMaterno.Layer.BorderWidth = 2;
                    var bordereditMaterno = new CALayer();
                    bordereditMaterno.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                    bordereditMaterno.Frame = new CoreGraphics.CGRect(0, editMaterno.Frame.Size.Height - width,
                    editMaterno.Frame.Size.Width, editMaterno.Frame.Size.Height);
                    bordereditMaterno.BorderWidth = width;
                    editMaterno.Layer.AddSublayer(bordereditMaterno);
                    editMaterno.Layer.MasksToBounds = true;

                    SystemSound.Vibrate.PlaySystemSound();
                    ErrorInput = true;
                    registerSuccess = false;
                }
            }
       

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

            if (string.IsNullOrEmpty(editPhone.Text) || !ValidatorHelper.IsValidPhone(editPhone.Text))
            {
                //editPhone.Layer.BorderColor = UIColor.Red.CGColor;
                //editPhone.Layer.BorderWidth = 2;
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
                    //editPass.Layer.BorderColor = UIColor.Red.CGColor;
                    //editPass.Layer.BorderWidth = 2;
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
                    //editPass2.Layer.BorderColor = UIColor.Red.CGColor;
                    //editPass2.Layer.BorderWidth = 2;
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
                string materno = editMaterno.Text;
                string fechanat = editBirth.Text;
                string sexo = editSex.Text;
                string telefono = editPhone.Text;
                string pass = editPass.Text;


                View.Add(loadPop);

                if (string.IsNullOrEmpty(editPass.Text) && string.IsNullOrEmpty(editPass2.Text))
                {
                    if(profilePicture != null && profilePicture.Length >0 && !string.IsNullOrEmpty(extension))
                    {
                        AppDelegate.Auth.ActualizarPerfil(nombre, paterno, materno, fechanat, sexo, telefono, COLONIA.Text, null,profilePicture,extension);
                    } else
                    {
                        AppDelegate.Auth.ActualizarPerfil(nombre, paterno, materno, fechanat, sexo, telefono, COLONIA.Text, null);
                    }
                    
                } else
                {
                    
                    AppDelegate.Auth.ActualizarPerfil(nombre, paterno, materno, fechanat, sexo, telefono, COLONIA.Text, pass);
                }

                

            }
        }

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

            // determine what was selected, video or image
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
    }
}