using AudioToolbox;
using CoreAnimation;
using CoreGraphics;
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
    public partial class RegistroViewController : UITableViewController
    {
        UIImagePickerController imagePicker;
        UIBarButtonItem Search;
        UIDatePicker datePicker;
        LoadingOverlay loadPop;

        List<string> Ciudades = new List<string> {
            "SELECCIONE...",
            "Mexicali",
            "Tijuana",
        };

        List<string> Colonias = new List<string> {
            "San Marcos",
            "Villafontana",
            "27 De Septiembre",
            "Pueblo Nuevo",
            "Venecia",
            "Baja California",
            "San Antonio",
            "Lomas Altas"
        };

        List<string> ColoniasTijuana = new List<string> {
            "Bellas Artes",
            "Hacienda Las Delicias",
            "Islas Mujeres",
            "Otay Colonial",
            "Puente La Joya",
            "Playas de Tijuana",
            "Rancho Escondido",
            "Valle Bonito"
        };

        bool ErrorInput = false;
        bool registerSuccess = true;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AppDelegate.Auth.PropertyChanged += Auth_PropertyChanged;

            inputColonia.UserInteractionEnabled = false;

            UIToolbar toolbarColonias = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            var bounds = UIScreen.MainScreen.Bounds;
            var bounds2 = TableView.Bounds;
            loadPop = new LoadingOverlay(bounds2, "Creando Cuenta...");
            // *** JUST 10 MAX. CHARACTERS ON TEXT FIELD *** //
            entryTel.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 10;
            };

            inputColonia.ShouldChangeCharacters = (textField, range, replacementString) =>
            {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 50;
            };

            UIPickerView pickerView = new UIPickerView(new CGRect(0, 44, View.Bounds.Width, 216));


            View.AddGestureRecognizer(new UITapGestureRecognizer(() => View.EndEditing(true)));
            CreateDatePicker();

            //PICKERVIEW SEXO
            var pickerModel = new PeopleModel(SexInput);
            pickerView.Model = pickerModel;
            SexInput.InputView = pickerView;

            //PICKERVIEW CIUDADES
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            UIPickerView PickerCiudades = new UIPickerView(new CGRect(0, 44, View.Bounds.Width, 216));
            var PickerModelCiudades = new PickerModel(CiudadesHelper.Ciudades);
            PickerCiudades.Model = PickerModelCiudades;
            Ciudad.InputView = PickerCiudades;

            Ciudad.InputAccessoryView = toolbar;
            PickerModelCiudades.ValueChanged += async (sender, e) =>
            {
                inputColonia.UserInteractionEnabled = false;
                inputColonia.Text = string.Empty;
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
                    inputColonia.UserInteractionEnabled = true;
                }
            };

            //PICKER COLONIAS MXLI
            timer = NSTimer.CreateRepeatingScheduledTimer(0.5, (_) =>
          {
              if (string.IsNullOrEmpty(Ciudad.Text))
              {
                  inputColonia.UserInteractionEnabled = false;
                  inputColonia.Text = string.Empty;
              }
              else
              {
                  //  inputColonia.UserInteractionEnabled = true;
                  //inputColonia.Text = string.Empty;
              }
          });
            Ciudad.EditingChanged += (sender, e) =>
            {

                Console.WriteLine("EL TEXTO CAMBIO");
            };
            //PICKER COLONIAS TIJUANA


            EnableNextKeyForTextFields(new UITextField[] { entryName,Paterno, Ciudad, inputColonia, BirthDate, SexInput, entryTel,
            entryEmail, entryPass, entryPass2 });
            AddDoneButton(entryTel, entryEmail);
            AddDoneButton(SexInput, entryTel);

            AddDoneButton(inputColonia, BirthDate);

            //SET BACKGROUND IMAGE TO TABLEVIEW //
            var backgroundimage = new UIImageView(View.Frame);
            backgroundimage.Image = UIImage.FromBundle("Fresco/Images/fondo_limpio.png");
            RegistroTable.BackgroundView = backgroundimage;
            //ButtonRegistrarFB.Layer.BorderWidth = 1;
            //ButtonRegistrarFB.Layer.BorderColor = UIColor.FromRGBA(red: 0.24f, green: 0.24f, blue: 0.24f, alpha: 1.0f).CGColor;


            //toolbarColonias.Items = new UIBarButtonItem[]
            //   {
            //    new UIBarButtonItem (UIBarButtonSystemItem.Search),
            //    Search,
            //   };
        }

        private void Instance_OnIniciarSesionFinished(object sender, LoginEventArgs e)
        {
            loadPop.Hide();
            if (Helpers.ValidarVersiones.ValidarAppVersion())
            {
                if (e.Success)
                {
                    //ABRIR PANTASHA PRINCIPAL
                    PreferencesHelper.SetCredentials(e.Username, e.Password);
                    BeginInvokeOnMainThread(() =>
                    {
                        PerformSegue("PANTALLA_PRINCIPAL_SEGUE", this);
                    });
                }
                else
                {
                    var Alert = UIAlertController.Create("Inicio de sesión", e.Message, UIAlertControllerStyle.Alert);
                    Alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    PresentViewController(Alert, true, null);
                }
            }
            else
            {
                var alert = UIAlertController.Create("Actualización Disponible", "Para seguir disfrutando de los beneficios de Fresco es necesario instalar la nueva versión",
                                   UIAlertControllerStyle.Alert);
                var WalletAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, null);
                var OkAction = UIAlertAction.Create("Ir a App Store", UIAlertActionStyle.Default, (Link) =>
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/mx/app/apple-store/id1460400309"));
                });
                alert.AddAction(OkAction);
                alert.AddAction(WalletAction);
                alert.PreferredAction = WalletAction;
                PresentViewController(alert, true, null);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AuthViewModelV2.Instance.OnIniciarSesionFinished += Instance_OnIniciarSesionFinished;
            AuthViewModelV2.Instance.OnConsultarColoniasFinished += Instance_OnConsultarColoniasFinished;

        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            AuthViewModelV2.Instance.OnIniciarSesionFinished -= Instance_OnIniciarSesionFinished;
            AuthViewModelV2.Instance.OnConsultarColoniasFinished -= Instance_OnConsultarColoniasFinished;
            AppDelegate.Auth.PropertyChanged -= Auth_PropertyChanged;

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

        public static UIStoryboard StoryboardMenu = UIStoryboard.FromName("Menu", null);
        public static UIViewController initialViewController;
        UIWindow window;
        private void Auth_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LoggedStatus")
            {
                //if (AppDelegate.Auth.LoggedStatus)
                //{
                //    BeginInvokeOnMainThread(() =>
                //    {
                //       // ButtonRegistrar.Enabled = true;

                //        loadPop.Hide();
                //        string user = entryEmail.Text;
                //        string password = entryPass.Text;
                //        PreferencesHelper.SetMostrarMensaje(true);
                //        PreferencesHelper.SetCredentials(user, password);

                //        window = new UIWindow(UIScreen.MainScreen.Bounds);
                //        initialViewController = StoryboardMenu.InstantiateInitialViewController() as UIViewController;
                //        window.RootViewController = initialViewController;
                //        window.MakeKeyAndVisible();

                //        // PerformSegue("REGISTER_MENU", this);

                //    });
                //}
                //else
                //{
                //    BeginInvokeOnMainThread(() =>
                //    {
                //        if (!string.IsNullOrEmpty(AppDelegate.Auth.ErrorMessage))
                //        {
                //            loadPop.Hide();
                //            var okAlertController = UIAlertController.Create("Inicio de Sesión", AppDelegate.Auth.ErrorMessage, UIAlertControllerStyle.Alert);
                //            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                //            PresentViewController(okAlertController, true, null);

                //            SystemSound.Vibrate.PlaySystemSound();
                //            AppDelegate.Auth.ErrorMessage = string.Empty;

                //        }
                //    });

                //}
            }

            //if (e.PropertyName == "ColoniasStatus" && AppDelegate.Auth.ColoniasStatus)
            //{
            //    BeginInvokeOnMainThread(() =>
            //   {
            //       SetColonias(AppDelegate.Auth.Colonias);
            //   });
            //}
            if (e.PropertyName == "IsBusy")
            {
                if (AppDelegate.Auth.IsBusy)
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        // ButtonRegistrar.Enabled = false;

                    });
                }
                else
                {
                    BeginInvokeOnMainThread(() =>
                    {



                    });
                }
            }
        }

        private void SetColonias(List<string> colonias)
        {

            UIPickerView PickerColonias = new UIPickerView(new CGRect(0, 44, View.Bounds.Width, 216));
            var PickerModelColonias = new PickerModel(colonias);
            PickerColonias.Model = PickerModelColonias;
            inputColonia.InputView = PickerColonias;

            PickerModelColonias.ValueChanged += delegate
            {
                inputColonia.Text = PickerModelColonias.SelectedValue;
            };

            //AddDoneButton(Ciudad, inputColonia);
            inputColonia.UserInteractionEnabled = true;
            inputColonia.BecomeFirstResponder();
            // inputColonia.BecomeFirstResponder();
        }
        #region KEYBOARD CONFIG
        //ADD DONE BUTTON

        protected void AddDoneButton(UITextField uiTextField, UITextField NextTextfield)
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            UIBarButtonItem buttonSiguiente = new UIBarButtonItem("Siguiente", UIBarButtonItemStyle.Done, delegate
            {
                if (uiTextField == BirthDate)
                {
                    NSDateFormatter dateFormatter = new NSDateFormatter();
                    dateFormatter.DateFormat = "dd/MM/yyyy";
                    string birtdayselect = dateFormatter.ToString(datePicker.Date);
                    BirthDate.Text = birtdayselect;
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
                        BirthDate.BecomeFirstResponder();
                    }

                }
                else
                {

                }

                if (uiTextField == SexInput)
                {
                    if (string.IsNullOrEmpty(SexInput.Text))
                    {
                        SexInput.Text = "Masculino";
                    }
                }

                NextTextfield.BecomeFirstResponder();
            });


            toolbar.Items = new UIBarButtonItem[] {
                     new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                buttonSiguiente,
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
            entryPass2.ResignFirstResponder();
            //RegistroTable.SetContentOffset(CGPoint.Empty, true);
        }
        #endregion

        public RegistroViewController(IntPtr handle) : base(handle)
        {
        }


        public void CreateDatePicker()
        {
            //Date Picker
            datePicker = new UIDatePicker(new CGRect(0, 44, View.Bounds.Width, 216));
            datePicker.Mode = UIDatePickerMode.Date;
            datePicker.MaximumDate = NSDate.Now;
            datePicker.Locale = NSLocale.FromLocaleIdentifier("es_MX");

            BirthDate.InputView = datePicker;
            AddDoneButton(BirthDate, SexInput);
        }

        partial void ButtonRegistrar_TouchUpInside(UIButton sender)
        {
            this.TableView.ContentOffset = new CGPoint(0, 0 - this.TableView.ContentInset.Top);
            View.EndEditing(true);

            registerSuccess = true;
            ErrorInput = false;

            var border = new CALayer();
            nfloat width = 1.5f;

            #region RESET LAYERS
            entryName.Layer.BorderWidth = 0;
            Paterno.Layer.BorderWidth = 0;
            // Materno.Layer.BorderWidth = 0;
            BirthDate.Layer.BorderWidth = 0;
            SexInput.Layer.BorderWidth = 0;
            entryTel.Layer.BorderWidth = 0;
            entryEmail.Layer.BorderWidth = 0;
            entryPass.Layer.BorderWidth = 0;
            entryPass2.Layer.BorderWidth = 0;
            inputColonia.Layer.BorderWidth = 0;
            #endregion

            if (string.IsNullOrEmpty(entryName.Text) || !ValidatorHelper.IsValidName(entryName.Text))
            {
                //entryName.Layer.BorderColor = UIColor.Red.CGColor;
                //entryName.Layer.BorderWidth = 2;
                var borderName = new CALayer();
                borderName.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                borderName.Frame = new CoreGraphics.CGRect(0, entryName.Frame.Size.Height - width,
                entryName.Frame.Size.Width, entryName.Frame.Size.Height);
                borderName.BorderWidth = width;
                entryName.Layer.AddSublayer(borderName);
                entryName.Layer.MasksToBounds = true;

                SystemSound.Vibrate.PlaySystemSound();
                ErrorInput = true;
                registerSuccess = false;

            }
            if (string.IsNullOrEmpty(Paterno.Text) || !ValidatorHelper.IsValidName(Paterno.Text))
            {
                //    Paterno.Layer.BorderColor = UIColor.Red.CGColor;
                //    Paterno.Layer.BorderWidth = 2;
                var borderPaterno = new CALayer();
                borderPaterno.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                borderPaterno.Frame = new CoreGraphics.CGRect(0, Paterno.Frame.Size.Height - width,
                Paterno.Frame.Size.Width, Paterno.Frame.Size.Height);
                borderPaterno.BorderWidth = width;
                Paterno.Layer.AddSublayer(borderPaterno);
                Paterno.Layer.MasksToBounds = true;

                SystemSound.Vibrate.PlaySystemSound();
                ErrorInput = true;
                registerSuccess = false;
            }
            //if (!string.IsNullOrEmpty(Materno.Text))
            //{
            //    if (!ValidatorHelper.IsValidName(Materno.Text))
            //    {
            //        //Materno.Layer.BorderColor = UIColor.Red.CGColor;
            //        //Materno.Layer.BorderWidth = 2;
            //        var borderMaterno = new CALayer();
            //        borderMaterno.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
            //        borderMaterno.Frame = new CoreGraphics.CGRect(0, Materno.Frame.Size.Height - width,
            //        Materno.Frame.Size.Width, Materno.Frame.Size.Height);
            //        borderMaterno.BorderWidth = width;
            //        Materno.Layer.AddSublayer(borderMaterno);
            //        Materno.Layer.MasksToBounds = true;

            //        SystemSound.Vibrate.PlaySystemSound();
            //        ErrorInput = true;
            //        registerSuccess = false;
            //    }
            //}

            if (!string.IsNullOrEmpty(BirthDate.Text))
            {
                if (!ValidatorHelper.IsValidBirthday(BirthDate.Text))
                {
                    //        BirthDate.Layer.BorderColor = UIColor.Red.CGColor;
                    //        BirthDate.Layer.BorderWidth = 2;
                    var borderDate = new CALayer();
                    borderDate.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                    borderDate.Frame = new CoreGraphics.CGRect(0, BirthDate.Frame.Size.Height - width,
                    BirthDate.Frame.Size.Width, BirthDate.Frame.Size.Height);
                    borderDate.BorderWidth = width;
                    BirthDate.Layer.AddSublayer(borderDate);
                    BirthDate.Layer.MasksToBounds = true;

                    SystemSound.Vibrate.PlaySystemSound();
                    ErrorInput = true;
                    registerSuccess = false;

                }

            }
            if (!string.IsNullOrEmpty(entryTel.Text))
            {
                if (!ValidatorHelper.IsValidPhone(entryTel.Text))
                {
                    //entryTel.Layer.BorderColor = UIColor.Red.CGColor;
                    //entryTel.Layer.BorderWidth = 2;
                    var borderTel = new CALayer();
                    borderTel.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                    borderTel.Frame = new CoreGraphics.CGRect(0, entryTel.Frame.Size.Height - width,
                    entryTel.Frame.Size.Width, entryTel.Frame.Size.Height);
                    borderTel.BorderWidth = width;
                    entryTel.Layer.AddSublayer(borderTel);
                    entryTel.Layer.MasksToBounds = true;

                    SystemSound.Vibrate.PlaySystemSound();
                    ErrorInput = true;
                    registerSuccess = false;
                }
            }

            if (string.IsNullOrEmpty(entryEmail.Text) || !ValidatorHelper.IsValidEmail(entryEmail.Text))
            {
                //entryEmail.Layer.BorderColor = UIColor.Red.CGColor;
                //entryEmail.Layer.BorderWidth = 2;
                var borderEmail = new CALayer();
                borderEmail.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                borderEmail.Frame = new CoreGraphics.CGRect(0, entryEmail.Frame.Size.Height - width,
                entryEmail.Frame.Size.Width, entryEmail.Frame.Size.Height);
                borderEmail.BorderWidth = width;
                entryEmail.Layer.AddSublayer(borderEmail);
                entryEmail.Layer.MasksToBounds = true;

                SystemSound.Vibrate.PlaySystemSound();
                ErrorInput = true;
                registerSuccess = false;
            }
            if (entryPass.Text.Length > 5)
            {
                if (string.IsNullOrEmpty(entryPass.Text) || !ValidatorHelper.IsValidPassword(entryPass.Text))
                {
                    //entryPass.Layer.BorderColor = UIColor.Red.CGColor;
                    //entryPass.Layer.BorderWidth = 2;
                    var borderPass = new CALayer();
                    borderPass.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                    borderPass.Frame = new CoreGraphics.CGRect(0, entryPass.Frame.Size.Height - width,
                    entryPass.Frame.Size.Width, entryPass.Frame.Size.Height);
                    borderPass.BorderWidth = width;
                    entryPass.Layer.AddSublayer(borderPass);
                    entryPass.Layer.MasksToBounds = true;

                    var borderPass2 = new CALayer();
                    borderPass2.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                    borderPass2.Frame = new CoreGraphics.CGRect(0, entryPass2.Frame.Size.Height - width,
                    entryPass2.Frame.Size.Width, entryPass2.Frame.Size.Height);
                    borderPass2.BorderWidth = width;
                    entryPass2.Layer.AddSublayer(borderPass2);
                    entryPass2.Layer.MasksToBounds = true;
                    SystemSound.Vibrate.PlaySystemSound();
                    ErrorInput = true;
                    registerSuccess = false;
                }
                else
                     if (string.IsNullOrEmpty(entryPass2.Text) || !ValidatorHelper.IsValidPassword(entryPass2.Text))
                {
                    //entryPass2.Layer.BorderColor = UIColor.Red.CGColor;
                    //entryPass2.Layer.BorderWidth = 2;
                    var borderPass2 = new CALayer();
                    borderPass2.BorderColor = UIColor.FromRGBA(red: 0.93f, green: 0.15f, blue: 0.22f, alpha: 1.0f).CGColor;
                    borderPass2.Frame = new CoreGraphics.CGRect(0, entryPass2.Frame.Size.Height - width,
                    entryPass2.Frame.Size.Width, entryPass2.Frame.Size.Height);
                    borderPass2.BorderWidth = width;
                    entryPass2.Layer.AddSublayer(borderPass2);
                    entryPass2.Layer.MasksToBounds = true;

                    SystemSound.Vibrate.PlaySystemSound();
                    ErrorInput = true;
                    registerSuccess = false;
                }
                else
                            if (entryPass.Text == entryPass2.Text)
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
            else
            {
                var okAlertController = UIAlertController.Create("Contraseña", "La contraseña debe contener más de seis caracteres", UIAlertControllerStyle.Alert);
                okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                PresentViewController(okAlertController, true, null);
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
                string nombre = entryName.Text;
                string paterno = Paterno.Text;
                string fechanat = BirthDate.Text;
                string sexo = SexInput.Text;
                string telefono = entryTel.Text;
                string email = entryEmail.Text;
                string pass = entryPass.Text;

                // ButtonRegistrar.Enabled = true;
                //View.Add(loadPop);
                View.Add(loadPop);
                AppDelegate.Auth.Registrar(new Models.Configuration.RegisterUser
                {
                    Metodo = AuthMethods.Email,
                    Nombre = nombre,
                    Paterno = paterno,
                    Materno = "",
                    FechaNacimiento = fechanat,
                    Sexo = sexo,
                    Telefono = telefono,
                    Colonia = inputColonia.Text,
                    Email = email,
                    Password = pass

                });
            }
        }

    }
}

