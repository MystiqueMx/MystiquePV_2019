using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;
using MystiqueNative.Droid.Utils;
using MystiqueNative.Droid.Utils.Views;
using MystiqueNative.EventsArgs;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Location;
using MystiqueNative.ViewModels;

namespace MystiqueNative.Droid.HazPedido.Direccion
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "Información de la ubicación", WindowSoftInputMode = SoftInput.AdjustResize)]
    public class EdicionDireccionActivity : BaseActivity, DetachableResultReceiver.IReceiver
    {
        protected override int LayoutResource => Resource.Layout.activity_haz_pedido_edicion_direccion;
        protected override int BackButtonIcon => Resource.Drawable.ic_close_white_24dp;

        #region EXPORTS

        public const int RequestNuevaDireccion = 3344;
        public const int RequestEditarDireccion = 3349;

        public const string ExtraLatitud = "EdicionDireccionActivity.ExtraLatitud";
        public const string ExtraLongitud = "EdicionDireccionActivity.ExtraLongitud";
        public const string ExtraIdEdicion = "EdicionDireccionActivity.ExtraIdEdicion";
        #endregion

        #region FIELDS

        private double _latitud = 99999;
        private double _longitud = 99999;
        private DetachableResultReceiver _resultReceiver;
        private Direction _direction;
        private readonly List<string> _colonias = new List<string>();
        private bool _coloniasLoaded;
        private bool _direccionLoaded;
        #endregion

        #region VIEWS

        private FrameLayout _progressBarHolder;
        private TextView _labelDireccion;
        private TextInputLayout _layoutNumero;
        private TextInputLayout _layoutCalle;
        private TextInputLayout _layoutReferencia;
        private TextInputEditText _entryNumero;
        private TextInputEditText _entryCalle;
        private TextInputEditText _entryReferencia;
        private Button _buttonGuardar;
        private TextInputLayout _layoutNombre;
        private TextInputEditText _entryNombre;

        private TextInputLayout _layoutColonia;
        private TextInputAutoCompleteTextView _entryColonia;

        #endregion

        #region LIFECYCLE

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _resultReceiver = new DetachableResultReceiver(new Handler());
            GrabViews();
            GrabIntentParameters();
            _resultReceiver.SetReceiver(this);
            _direccionLoaded = false;
            _coloniasLoaded = false;

        }

        private void GrabViews()
        {
            _labelDireccion = FindViewById<TextView>(Resource.Id.editar_direccion_label_direccion);

            _layoutNumero = FindViewById<TextInputLayout>(Resource.Id.editar_direccion_layout_numero);
            _layoutCalle = FindViewById<TextInputLayout>(Resource.Id.editar_direccion_layout_calle);
            _layoutReferencia = FindViewById<TextInputLayout>(Resource.Id.editar_direccion_layout_referencia);
            _layoutNombre = FindViewById<TextInputLayout>(Resource.Id.editar_direccion_layout_nombre);

            _entryNumero = FindViewById<TextInputEditText>(Resource.Id.editar_direccion_entry_numero);
            _entryCalle = FindViewById<TextInputEditText>(Resource.Id.editar_direccion_entry_calle);
            _entryReferencia = FindViewById<TextInputEditText>(Resource.Id.editar_direccion_entry_referencia);
            _entryNombre = FindViewById<TextInputEditText>(Resource.Id.editar_direccion_entry_nombre);

            _progressBarHolder = FindViewById<FrameLayout>(Resource.Id.progressBarHolder);

            _buttonGuardar = FindViewById<Button>(Resource.Id.button_guardar);
            _buttonGuardar.Click += ButtonGuardar_Click;

            _layoutColonia = FindViewById<TextInputLayout>(Resource.Id.editar_direccion_layout_colonia);
            _entryColonia = FindViewById<TextInputAutoCompleteTextView>(Resource.Id.editar_direccion_entry_colonia);
        }

        protected override void OnResume()
        {
            base.OnResume();

            DireccionesViewModel.Instance.OnAgregarDireccionFinished += Instance_OnAgregarDireccionFinished;
            DireccionesViewModel.Instance.OnEditarDireccionFinished += Instance_OnEditarDireccionFinished;
            DireccionesViewModel.Instance.OnObtenerColoniasFinished += Instance_OnObtenerColoniasFinished;

            if (_coloniasLoaded) return;
            if (_direccionLoaded)
            {
                StartAnimatingLogin();

                DireccionesViewModel.Instance.ObtenerColonias(new Point
                {
                    Longitude = _longitud,
                    Latitude = _latitud
                }, _direction.PostalCode, CarritoViewModel.Instance.ExistenItemsEnCarrito
                    ? CarritoViewModel.Instance.PedidoActual.Restaurante.Id
                    : 0);
            }
            else
            {
                StartAnimatingLogin();
                ParseDireccionFromUbicacion(new Point { Latitude = _latitud, Longitude = _longitud, });
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            DireccionesViewModel.Instance.OnAgregarDireccionFinished -= Instance_OnAgregarDireccionFinished;
            DireccionesViewModel.Instance.OnEditarDireccionFinished -= Instance_OnEditarDireccionFinished;
            DireccionesViewModel.Instance.OnObtenerColoniasFinished -= Instance_OnObtenerColoniasFinished;
            StopAnimatingLogin(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _resultReceiver.ClearReceiver();
        }
        #endregion

        private void GrabIntentParameters()
        {
            var id = Intent.GetIntExtra(ExtraIdEdicion, -1);
            if (id == -1)
            {
                _latitud = Intent.GetDoubleExtra(ExtraLatitud, 99999);
                _direccionLoaded = false;
                if (Math.Abs(_latitud - 99999) < 1) throw new ArgumentNullException(nameof(ExtraLatitud));
                _longitud = Intent.GetDoubleExtra(ExtraLongitud, 99999);
                if (Math.Abs(_longitud - 99999) < 1) throw new ArgumentNullException(nameof(ExtraLongitud));
            }
            else
            {
                _direction = DireccionesViewModel.Instance.Direcciones.First(c => c.Id == id);
                _direccionLoaded = true;
                _latitud = _direction.Latitud;
                _longitud = _direction.Longitud;
            }
        }

        private void SetUpColoniasAutoComplete()
        {

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _colonias);
            adapter.SetDropDownViewResource(Resource.Layout.support_simple_spinner_dropdown_item);
            _entryColonia.Adapter = adapter;
            _entryColonia.Threshold = 1;
            _entryColonia.FocusChange += EntryColonia_FocusChange;
            _entryColonia.Enabled = _colonias.Count > 0;
        }

        private void ParseDireccionFromUbicacion(Point location)
        {
            var intent = new Intent(this, typeof(AddressResolverIntentService));
#if DEBUG
            Console.WriteLine($"||||||||||||||||||| ParseDireccionFromUbicacion Starting service intent");
#endif
            intent.PutExtra(AddressResolverIntentService.AddressReceiverExtra, _resultReceiver);
            intent.PutExtra(AddressResolverIntentService.LatitudeDataExtra, location.Latitude);
            intent.PutExtra(AddressResolverIntentService.LongitudeDataExtra, location.Longitude);
            StartService(intent);
        }

        public void OnReceiveResult(int resultCode, Bundle resultData)
        {
            if (_direccionLoaded) return;
            _direccionLoaded = true;
            if ((Result)resultCode == Result.Ok)
            {
                _direction = new Direction
                {
                    CountryCode = resultData.GetString(AddressResolverIntentService.ResultCountryCodeExtra),
                    CountryName = resultData.GetString(AddressResolverIntentService.ResultCountryNameExtra),
                    AdminArea = resultData.GetString(AddressResolverIntentService.ResultAdminAreaExtra),
                    SubAdminArea = resultData.GetString(AddressResolverIntentService.ResultSubAdminAreaExtra),
                    Locality = resultData.GetString(AddressResolverIntentService.ResultLocalityExtra),
                    SubLocality = resultData.GetString(AddressResolverIntentService.ResultSublocalityExtra),
                    Thoroughfare = resultData.GetString(AddressResolverIntentService.ResultThoroughfareExtra),
                    SubThoroughfare = resultData.GetString(AddressResolverIntentService.ResultSubThoroughfareExtra),
                    PostalCode = resultData.GetString(AddressResolverIntentService.ResultPostalCodeExtra),
                    OtherAddressLines = resultData.GetString(AddressResolverIntentService.ResultAddrExtra),
                    FeatureName = resultData.GetString(AddressResolverIntentService.ResultFeatureExtra)
                };

#if DEBUG
                Console.WriteLine($"||||||||||||||||||| OnReceiveResult Location Updated: {Newtonsoft.Json.JsonConvert.SerializeObject(_direction)}");
#endif
            }
            else
            {
                _direction = new Direction();
                Console.WriteLine(resultData.GetString(AddressResolverIntentService.ResultErrorExtra));
            }

            DireccionesViewModel.Instance.ObtenerColonias(new Point
            {
                Longitude = _longitud,
                Latitude = _latitud
            }, _direction.PostalCode ?? string.Empty, CarritoViewModel.Instance.ExistenItemsEnCarrito
                ? CarritoViewModel.Instance.PedidoActual.Restaurante.Id
                : 0);
        }

        private void UpdateUi()
        {
            if (_direction == null) return;

            StopAnimatingLogin();


            var labelDireccion = "Dirección: ";

            if (!string.IsNullOrEmpty(_direction.PostalCode))
            {
                labelDireccion += _direction.PostalCode;
            }
            if (!string.IsNullOrEmpty(_direction.Locality))
            {
                labelDireccion += $" {_direction.Locality}";
            }
            if (!string.IsNullOrEmpty(_direction.AdminArea))
            {
                labelDireccion += $", {_direction.AdminArea}";
            }

            _entryCalle.Text = _direction.Thoroughfare;
            _entryNumero.Text = _direction.SubThoroughfare;
            _entryReferencia.Text = _direction.OtherAddressLines;
            _entryNombre.Text = _direction.Nombre;
            _labelDireccion.Text = labelDireccion;

            _layoutColonia.Error = _colonias.Count == 0 ? $"Lo sentimos, por el momento no se cuenta con cobertura en el área seleccionada" : string.Empty;
            _coloniasLoaded = true;
        }

        private void Instance_OnObtenerColoniasFinished(object sender, ObtenerColoniasEventArgs e)
        {
            if (e.Success)
            {
                _colonias.Clear();

                if (!string.IsNullOrEmpty(_direction.PostalCode))
                {
                    // Si hay codigo postal, usa la colonia que se obtuvo via geocode
                    var coloniaGeocode = _direction.SubLocality;
                    if (!string.IsNullOrEmpty(coloniaGeocode)
                        && e.Colonias.All(c => c != _direction.SubLocality))
                    {
                        _colonias.Add(coloniaGeocode);
                    }
                    _entryColonia.Text = coloniaGeocode;
                }
                _colonias.AddRange(e.Colonias);
                SetUpColoniasAutoComplete();
            }
            else
            {
                SendMessage(e.Message);
            }
            _coloniasLoaded = true;
            UpdateUi();
        }

        private async void ButtonGuardar_Click(object sender, System.EventArgs e)
        {
            if (!ValidarInputs()) return;

            StartAnimatingLogin();
            if (string.IsNullOrEmpty(_direction.PostalCode))
            {
                _direction.SubLocality = _entryColonia.Text;
            }
            _direction.Nombre = _entryNombre.Text;
            _direction.Thoroughfare = _entryCalle.Text;
            _direction.SubThoroughfare = _entryNumero.Text;
            _direction.OtherAddressLines = _entryReferencia.Text;
            _direction.Latitud = _latitud;
            _direction.Longitud = _longitud;

            if (_direction.Id > 0)
            {
                await DireccionesViewModel.Instance.EditarDireccion(_direction,
                    CarritoViewModel.Instance.ExistenItemsEnCarrito
                    ? CarritoViewModel.Instance.PedidoActual.Restaurante.Id
                    : 0);
            }
            else
            {
                await DireccionesViewModel.Instance.AgregarDireccion(_direction,
                    CarritoViewModel.Instance.ExistenItemsEnCarrito
                    ? CarritoViewModel.Instance.PedidoActual.Restaurante.Id
                    : 0);
            }

        }

        private bool ValidarInputs()
        {
            var canContinue = true;
            View focusView = null;

            if (string.IsNullOrEmpty(_entryNombre.Text))
            {
                _layoutNombre.Error = GetString(Resource.String.error_campos_vacios);
                canContinue = false;
                focusView = _entryNombre;
            }
            else
            {
                _layoutNombre.Error = string.Empty;

            }

            if (string.IsNullOrEmpty(_entryColonia.Text))
            {
                _layoutColonia.Error = GetString(Resource.String.error_campos_vacios);
                canContinue = false;
                focusView = focusView ?? _entryColonia;
            }
            else
            {
                if (_colonias.Contains(_entryColonia.Text))
                {
                    _layoutColonia.Error = string.Empty;
                }
                else
                {
                    _layoutColonia.Error = $"La colonia que ingresaste es inválida";
                    canContinue = false;
                    focusView = focusView ?? _entryColonia;
                }

            }

            if (string.IsNullOrEmpty(_entryCalle.Text))
            {
                _layoutCalle.Error = GetString(Resource.String.error_campos_vacios);
                canContinue = false;
                focusView = focusView ?? _entryCalle;
            }
            else
            {
                _layoutCalle.Error = string.Empty;

            }

            if (string.IsNullOrEmpty(_entryNumero.Text))
            {
                _layoutNumero.Error = GetString(Resource.String.error_campos_vacios);
                canContinue = false;
                focusView = focusView ?? _entryNumero;
            }
            else
            {
                _layoutNumero.Error = string.Empty;

            }

            if (string.IsNullOrEmpty(_entryReferencia.Text))
            {
                _layoutReferencia.Error = GetString(Resource.String.error_campos_vacios);
                canContinue = false;
                focusView = focusView ?? _entryReferencia;
            }
            else
            {
                _layoutReferencia.Error = string.Empty;

            }

            focusView?.RequestFocus();


            return canContinue;
        }


        private void EntryColonia_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus) _layoutColonia.Error = _colonias.Contains(_entryColonia.Text) ? string.Empty : $"La colonia que ingresaste es inválida";
        }

        private void Instance_OnAgregarDireccionFinished(object sender, BaseUpdateArgs e)
        {

            if (e.Success)
            {
                var intent = new Intent();
                intent.PutExtra(ExtraIdEdicion, e.EditedId);
                SetResult(Result.Ok, intent);
                Finish();
                StopAnimatingLogin();
            }
            else
            {
                SendConfirmation(e.Message, "", "Volver", "", (ok) =>
                {
                    StopAnimatingLogin();
                });
            }
        }


        private void Instance_OnEditarDireccionFinished(object sender, BaseUpdateArgs e)
        {
            if (e.Success)
            {
                var intent = new Intent();
                intent.PutExtra(ExtraIdEdicion, e.EditedId);
                SetResult(Result.Ok, intent);
                Finish();
                StopAnimatingLogin();
            }
            else
            {
                SendConfirmation(e.Message, "", "Volver", "", (ok) =>
                {
                    StopAnimatingLogin();
                });
            }
        }

        #region LOADING
        private void StartAnimatingLogin()
        {
            var inAnimation = new Android.Views.Animations.AlphaAnimation(0f, 1f)
            {
                Duration = 100
            };
            _progressBarHolder.Animation = inAnimation;
            _progressBarHolder.Visibility = ViewStates.Visible;
        }

        private void StopAnimatingLogin(bool animate = true)
        {
            if (animate)
            {
                var outAnimation = new Android.Views.Animations.AlphaAnimation(1f, 0f)
                {
                    Duration = 100
                };
                _progressBarHolder.Animation = outAnimation;
            }

            _progressBarHolder.Visibility = ViewStates.Gone;
        }
        #endregion
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return true;
            }
        }
    }
}