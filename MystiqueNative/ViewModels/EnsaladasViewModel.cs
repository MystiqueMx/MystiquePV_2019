using Humanizer;
using MystiqueNative.Helpers;
using MystiqueNative.Models.Carrito;
using MystiqueNative.Models.Ensaladas;
using MystiqueNative.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystiqueNative.ViewModels
{
    public class EnsaladasViewModel : BaseViewModel
    {
        #region SINGLETON
        public static EnsaladasViewModel Instance => _instance ?? (_instance = new EnsaladasViewModel());
        private static EnsaladasViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        #region EVENTS
        public event EventHandler<BaseEventArgs> OnObtenerConfiguracionEnsaladasFinished;
        public event EventHandler<BaseEventArgs> OnExtraAdded;
        #endregion

        #region CTOR

        public EnsaladasViewModel()
        {
            _configuracionesEnsaladas = new List<ConfiguracionEnsalada>();
        }

        #endregion
        #region FIELDS

        public bool IsLoaded { get; private set; }
        public ConfiguracionEnsaladas Configuracion { get; private set; }
        public Ensalada Ensalada { get; private set; }

        public string EtiquetaProteina { get; set; }
        public string EtiquetaBarraFria { get; set; }
        public string EtiquetaAderezos { get; set; }
        public string EtiquetaComplementos { get; set; }
        public string EtiquetaExtras { get; set; }
        public string EtiquetaCortesias { get; set; }

        internal List<ConfiguracionEnsalada> _configuracionesEnsaladas;
        internal List<IngredienteEnsalada> _ingredientes;
        private List<IngredienteEnsalada> ProteinaCamaron => _ingredientes.Where(c => new[] { 7 }.Contains(c.Id)).ToList();

        private List<IngredienteEnsalada> ProteinaPollo => _ingredientes.Where(c => new[] { 1, 2, 3, 4, 5, 6, 8, 71, 72, 73, 75, 76, 77, 78, 87 }.Contains(c.Id)).ToList();

        private List<IngredienteEnsalada> ProteinaMariscos => _ingredientes.Where(c => new[] { 6, 8 }.Contains(c.Id)).ToList();

        //ENSALADA ACTUAL
        public int CantidadIngredientesProteina => Ensalada.CantidadIngredientesProteina.Sum(c => c.Value);
        public int CantidadIngredientesBarra => Ensalada.CantidadIngredientesBarra.Sum(c => c.Value);
        public int CantidadIngredientesAderezos => Ensalada.CantidadIngredientesAderezos.Sum(c => c.Value);

        public int CantidadIngredientesComplementos => Ensalada.CantidadIngredientesComplementos.Sum(c => c.Value);
        public int CantidadIngredientesCortesias => Ensalada.CantidadIngredientesCortesias.Sum(c => c.Value);
        public int CantidadIngredientesExtra => Ensalada.CantidadIngredientesExtra.Sum(c => c.Value);

        //CONFIGURACION ACTUAL
        private const int MaximoExtras = 5;

        public ConfiguracionEnsalada CantidadesEnsaladaActual =>
            _configuracionesEnsaladas.FirstOrDefault(c => c.Id == (int)Ensalada.Presentacion);
        public int MaximoProteinas => CantidadesEnsaladaActual.CantidadProteina + MaximoExtras;
        public int MaximoAderezos => CantidadesEnsaladaActual.CantidadAderezos + MaximoExtras;
        public int MaximoBarraFria => CantidadesEnsaladaActual.CantidadBarraFria + MaximoExtras;
        public int MaximoComplementos => CantidadesEnsaladaActual.CantidadComplementos;
        public int MaximoCantidadExtras => MaximoExtras;
        public int MaximoCantidadCortesias => CantidadesEnsaladaActual.CantidadCortesias;

        //VALIDACIONES PASOS

        public bool HaCompleadoPrimerPaso => CantidadIngredientesProteina >= CantidadesEnsaladaActual?.CantidadProteina;
        public bool HaCompleadoSegundoPaso => CantidadIngredientesBarra >= CantidadesEnsaladaActual?.CantidadBarraFria;
        public bool HaCompleadoTercerPaso => true;
        public bool HaCompletadoCuartoPaso => CantidadIngredientesAderezos > 0;

        #endregion
        #region API

        public async Task ObtenerConfiguracion()
        {
            IsBusy = true;
            var response = await QdcApi.Restaurantes.LlamarObtenerConfiguracionEnsaladas();
            if (response.Estatus.IsSuccessful)
            {

                Configuracion = new ConfiguracionEnsaladas()
                {
                    Precios = new PreciosEnsaladas()
                    {
                        PrecioEnsaladaCamaronChica = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.CamaronChica)
                            ?.PrecioConFormatoMoneda,
                        PrecioEnsaladaCamaronMediana = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.CamaronMediana)
                            ?.PrecioConFormatoMoneda,
                        PrecioEnsaladaCamaronGrande = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.CamaronGrande)
                            ?.PrecioConFormatoMoneda,
                        PrecioEnsaladaPolloChica = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.PolloChica)
                            ?.PrecioConFormatoMoneda,
                        PrecioEnsaladaPolloMediana = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.PolloMediana)
                            ?.PrecioConFormatoMoneda,
                        PrecioEnsaladaPolloGrande = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.PolloGrande)
                            ?.PrecioConFormatoMoneda,
                        PrecioEnsaladaMariscosChica = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.MariscosChica)
                            ?.PrecioConFormatoMoneda,
                        PrecioEnsaladaMariscosMediana = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.MariscosMediana)
                            ?.PrecioConFormatoMoneda,
                        PrecioEnsaladaMariscosGrande = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.MariscosGrande)
                            ?.PrecioConFormatoMoneda,

                        PrecioWrapChica = response.Resultados.ListaConfiguracionesEnsaladas
                            .FirstOrDefault(c => c.Id == (int)PresentacionEnsalada.WrapChica)
                            ?.PrecioConFormatoMoneda,
                    },
                    IngredientesAderezos = response.Resultados.ListaIngredientesEnsaladas
                            .Where(c => c.TipoIngrediente == (int)CategoriaIngredienteEnsalada.Aderezos)
                            .ToList(),
                    IngredientesBarraFria = response.Resultados.ListaIngredientesEnsaladas
                        .Where(c => c.TipoIngrediente == (int)CategoriaIngredienteEnsalada.BarraFria)
                        .OrderBy(o => o.Descripcion)
                        .ToList(),
                    IngredientesComplementos = response.Resultados.ListaIngredientesEnsaladas
                        .Where(c => c.TipoIngrediente == (int)CategoriaIngredienteEnsalada.Complementos)
                        .ToList(),
                    IngredientesCortesias = response.Resultados.ListaIngredientesEnsaladas
                        .Where(c => c.TipoIngrediente == (int)CategoriaIngredienteEnsalada.Cortesias)
                        .ToList(),
                    IngredientesExtras = response.Resultados.ListaIngredientesEnsaladas
                        .Where(c => c.TipoIngrediente == (int)CategoriaIngredienteEnsalada.Extras)
                        .ToList(),
                };
                _configuracionesEnsaladas = response.Resultados.ListaConfiguracionesEnsaladas;
                _ingredientes = response.Resultados.ListaIngredientesEnsaladas;
                IsLoaded = true;
                OnObtenerConfiguracionEnsaladasFinished?.Invoke(this, new BaseEventArgs()
                {
                    Success = true,
                });
            }
            else
            {
                OnObtenerConfiguracionEnsaladasFinished?.Invoke(this, new BaseEventArgs()
                {
                    Success = false,
                    Message = response.Estatus.Message
                });
            }

            IsBusy = false;
        }

        public void SeleccionarPresentacion(PresentacionEnsalada presentacion)
        {
            var configuracion = _configuracionesEnsaladas.First(c => c.Id == (int)presentacion);
            switch (presentacion)
            {
                case PresentacionEnsalada.PolloChica:
                case PresentacionEnsalada.PolloMediana:
                case PresentacionEnsalada.PolloGrande:
                case PresentacionEnsalada.WrapChica:
                    Configuracion.IngredientesProteina = ProteinaPollo;
                    break;
                case PresentacionEnsalada.MariscosChica:
                case PresentacionEnsalada.MariscosMediana:
                case PresentacionEnsalada.MariscosGrande:
                    Configuracion.IngredientesProteina = ProteinaMariscos;
                    break;
                case PresentacionEnsalada.CamaronChica:
                case PresentacionEnsalada.CamaronMediana:
                case PresentacionEnsalada.CamaronGrande:
                    Configuracion.IngredientesProteina = ProteinaCamaron;
                    break;
                case PresentacionEnsalada.NoDefinida:
                default:
                    throw new NotSupportedException(presentacion.ToString());
            }
            Ensalada = new Ensalada()
            {
                IdEnsalada = _configuracionesEnsaladas.First(c => c.Id == (int)presentacion).IdPlatillo,
                Presentacion = presentacion,
                CantidadIngredientesAderezos = new Dictionary<int, int>(),
                CantidadIngredientesCortesias = new Dictionary<int, int>(),
                CantidadIngredientesProteina = new Dictionary<int, int>(),
                CantidadIngredientesBarra = new Dictionary<int, int>(),
                CantidadIngredientesComplementos = new Dictionary<int, int>(),
                CantidadIngredientesExtra = new Dictionary<int, int>(),
                Precio = configuracion.Precio,
            };
            EtiquetaAderezos = $"0/{configuracion.CantidadAderezos}";
            EtiquetaBarraFria = $"0/{configuracion.CantidadBarraFria}";
            EtiquetaComplementos = $"0/{configuracion.CantidadComplementos}";
            EtiquetaCortesias = $"0/{configuracion.CantidadCortesias}";
            EtiquetaProteina = $"0/{configuracion.CantidadProteina}";
            EtiquetaExtras = $"0";

            ReiniciarPasoUno();
            ReiniciarPasoDos();
            ReiniciarPasoTres();
            ReiniciarPasoCuatro();
        }

        internal void ReiniciarPasoUno()
        {
            EnsaladaPasoUnoViewModel.Instance.EtiquetaProteina = $"0/{CantidadesEnsaladaActual.CantidadProteina}";
            EnsaladaPasoUnoViewModel.Instance.EtiquetaExtras = $"0";

            Ensalada.CantidadIngredientesExtra = new Dictionary<int, int>();
            Ensalada.CantidadIngredientesProteina = new Dictionary<int, int>();
        }

        internal void ReiniciarPasoDos()
        {
            EnsaladaPasoDosViewModel.Instance.EtiquetaBarraFria = $"0/{CantidadesEnsaladaActual.CantidadBarraFria}";

            Ensalada.CantidadIngredientesBarra = new Dictionary<int, int>();
        }

        internal void ReiniciarPasoTres()
        {
            EnsaladaPasoTresViewModel.Instance.EtiquetaCortesias = $"0/{CantidadesEnsaladaActual.CantidadCortesias}";
            EnsaladaPasoTresViewModel.Instance.EtiquetaComplementos = $"0/{CantidadesEnsaladaActual.CantidadComplementos}";

            Ensalada.CantidadIngredientesCortesias = new Dictionary<int, int>();
            Ensalada.CantidadIngredientesComplementos = new Dictionary<int, int>();
        }

        internal void ReiniciarPasoCuatro()
        {
            EnsaladaPasoCuatroViewModel.Instance.EtiquetaAderezos = $"0/{CantidadesEnsaladaActual.CantidadAderezos}";

            Ensalada.CantidadIngredientesAderezos = new Dictionary<int, int>();
        }

        [Obsolete]
        public string AgregarProteina(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesProteina.Values.Sum();
            var result = EtiquetaProteina;

            if (countIngredientesSeleccionados >= CantidadesEnsaladaActual.CantidadProteina)
            {

                if (countIngredientesSeleccionados < MaximoProteinas)
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Las proteinas extras tienen un costo adicional de {ingredienteEnsalada.Precio:C} MXN"
                    });
                    //Ensalada.Precio = Ensalada.Precio + ingredienteEnsalada.Precio;

                    if (Ensalada.CantidadIngredientesProteina.ContainsKey(ingredienteEnsalada.Id))
                    {
                        Ensalada.CantidadIngredientesProteina[ingredienteEnsalada.Id]++;
                    }
                    else
                    {
                        Ensalada.CantidadIngredientesProteina.Add(ingredienteEnsalada.Id, 1);
                    }
                }
                else
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Solo puedes agregar {MaximoCantidadExtras} proteinas extras por ensalada"

                    });
                }
            }
            else
            {
                if (Ensalada.CantidadIngredientesProteina.ContainsKey(ingredienteEnsalada.Id))
                {
                    Ensalada.CantidadIngredientesProteina[ingredienteEnsalada.Id]++;
                }
                else
                {
                    Ensalada.CantidadIngredientesProteina.Add(ingredienteEnsalada.Id, 1);
                }
            }


            var finalCount = Ensalada.CantidadIngredientesProteina.Values.Sum();
            if (finalCount > CantidadesEnsaladaActual.CantidadProteina)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadProteina;
                result =
                    $"{CantidadesEnsaladaActual.CantidadProteina}/{CantidadesEnsaladaActual.CantidadProteina} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadProteina}";
            }

            EtiquetaProteina = result;
            return EtiquetaProteina;
        }
        [Obsolete]
        public string RemoverProteina(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesProteina.Values.Sum();
            var result = EtiquetaProteina;

            if (Ensalada.CantidadIngredientesProteina.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > configuracionPresentacion.CantidadProteina)
                {
                    //Ensalada.Precio = Ensalada.Precio - ingredienteEnsalada.Precio;
                }
                Ensalada.CantidadIngredientesProteina[ingredienteEnsalada.Id]--;

                if (Ensalada.CantidadIngredientesProteina[ingredienteEnsalada.Id] <= 0)
                {
                    Ensalada.CantidadIngredientesProteina.Remove(ingredienteEnsalada.Id);
                }
            }
            else
            {
                return result;
            }

            var finalCount = Ensalada.CantidadIngredientesProteina.Values.Sum();
            if (finalCount > configuracionPresentacion.CantidadProteina)
            {
                var extraCount = finalCount - configuracionPresentacion.CantidadProteina;
                result =
                    $"{configuracionPresentacion.CantidadProteina}/{configuracionPresentacion.CantidadProteina} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{configuracionPresentacion.CantidadProteina}";
            }

            EtiquetaProteina = result;
            return EtiquetaProteina;

        }
        [Obsolete]
        public string AgregarAderezo(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesAderezos.Values.Sum();
            var result = EtiquetaAderezos;

            if (countIngredientesSeleccionados >= configuracionPresentacion.CantidadAderezos)
            {
                if (countIngredientesSeleccionados < MaximoAderezos)
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Los aderezos extras tienen un costo adicional de {ingredienteEnsalada.Precio:C} MXN"
                    });
                    //Ensalada.Precio = Ensalada.Precio + ingredienteEnsalada.Precio;


                    if (Ensalada.CantidadIngredientesAderezos.ContainsKey(ingredienteEnsalada.Id))
                    {
                        Ensalada.CantidadIngredientesAderezos[ingredienteEnsalada.Id]++;
                    }
                    else
                    {
                        Ensalada.CantidadIngredientesAderezos.Add(ingredienteEnsalada.Id, 1);
                    }
                }
                else
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Solo puedes agregar {MaximoAderezos} aderezos por ensalada"
                    });
                }
            }
            else
            {
                if (Ensalada.CantidadIngredientesAderezos.ContainsKey(ingredienteEnsalada.Id))
                {
                    Ensalada.CantidadIngredientesAderezos[ingredienteEnsalada.Id]++;
                }
                else
                {
                    Ensalada.CantidadIngredientesAderezos.Add(ingredienteEnsalada.Id, 1);
                }
            }

            var finalCount = Ensalada.CantidadIngredientesAderezos.Values.Sum();
            if (finalCount > configuracionPresentacion.CantidadAderezos)
            {
                var extraCount = finalCount - configuracionPresentacion.CantidadAderezos;
                result =
                    $"{configuracionPresentacion.CantidadAderezos}/{configuracionPresentacion.CantidadAderezos} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{configuracionPresentacion.CantidadAderezos}";
            }

            EtiquetaAderezos = result;
            return EtiquetaAderezos;
        }
        [Obsolete]
        public string RemoverAderezo(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesAderezos.Values.Sum();
            var result = EtiquetaAderezos;

            if (Ensalada.CantidadIngredientesAderezos.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > configuracionPresentacion.CantidadAderezos)
                {
                    //Ensalada.Precio = Ensalada.Precio - ingredienteEnsalada.Precio;
                }
                Ensalada.CantidadIngredientesAderezos[ingredienteEnsalada.Id]--;
                if (Ensalada.CantidadIngredientesAderezos[ingredienteEnsalada.Id] <= 0)
                {
                    Ensalada.CantidadIngredientesAderezos.Remove(ingredienteEnsalada.Id);
                }
            }
            else
            {
                return result;
            }

            var finalCount = Ensalada.CantidadIngredientesAderezos.Values.Sum();
            if (finalCount > configuracionPresentacion.CantidadAderezos)
            {
                var extraCount = finalCount - configuracionPresentacion.CantidadAderezos;
                result =
                    $"{configuracionPresentacion.CantidadAderezos}/{configuracionPresentacion.CantidadAderezos} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{configuracionPresentacion.CantidadAderezos}";
            }

            EtiquetaAderezos = result;
            return EtiquetaAderezos;

        }
        [Obsolete]
        public string AgregarBarraFria(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesBarra.Values.Sum();
            var result = EtiquetaBarraFria;

            if (countIngredientesSeleccionados >= configuracionPresentacion.CantidadBarraFria)
            {
                if (countIngredientesSeleccionados < MaximoBarraFria)
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Los ingredientes de barra fria extras tienen un costo adicional de {ingredienteEnsalada.Precio:C}"
                    });
                    //Ensalada.Precio = Ensalada.Precio + ingredienteEnsalada.Precio;


                    if (Ensalada.CantidadIngredientesBarra.ContainsKey(ingredienteEnsalada.Id))
                    {
                        Ensalada.CantidadIngredientesBarra[ingredienteEnsalada.Id]++;
                    }
                    else
                    {
                        Ensalada.CantidadIngredientesBarra.Add(ingredienteEnsalada.Id, 1);
                    }
                }
                else
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Solo puedes agregar {MaximoCantidadExtras} ingredientes de barra fría por ensalada"

                    });
                }

            }
            else
            {
                if (Ensalada.CantidadIngredientesBarra.ContainsKey(ingredienteEnsalada.Id))
                {
                    Ensalada.CantidadIngredientesBarra[ingredienteEnsalada.Id]++;
                }
                else
                {
                    Ensalada.CantidadIngredientesBarra.Add(ingredienteEnsalada.Id, 1);
                }
            }

            var finalCount = Ensalada.CantidadIngredientesBarra.Values.Sum();
            if (finalCount > configuracionPresentacion.CantidadBarraFria)
            {
                var extraCount = finalCount - configuracionPresentacion.CantidadBarraFria;
                result =
                    $"{configuracionPresentacion.CantidadBarraFria}/{configuracionPresentacion.CantidadBarraFria} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{configuracionPresentacion.CantidadBarraFria}";
            }

            EtiquetaBarraFria = result;
            return EtiquetaBarraFria;
        }
        [Obsolete]
        public string RemoverBarraFria(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesBarra.Values.Sum();
            var result = EtiquetaBarraFria;

            if (Ensalada.CantidadIngredientesBarra.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > configuracionPresentacion.CantidadBarraFria)
                {
                    //Ensalada.Precio = Ensalada.Precio - ingredienteEnsalada.Precio;
                }
                Ensalada.CantidadIngredientesBarra[ingredienteEnsalada.Id]--;
                if (Ensalada.CantidadIngredientesBarra[ingredienteEnsalada.Id] <= 0)
                {
                    Ensalada.CantidadIngredientesBarra.Remove(ingredienteEnsalada.Id);
                }
            }
            else
            {
                return result;
            }

            var finalCount = Ensalada.CantidadIngredientesBarra.Values.Sum();
            if (finalCount > configuracionPresentacion.CantidadBarraFria)
            {
                var extraCount = finalCount - configuracionPresentacion.CantidadBarraFria;
                result =
                    $"{configuracionPresentacion.CantidadBarraFria}/{configuracionPresentacion.CantidadBarraFria} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{configuracionPresentacion.CantidadBarraFria}";
            }

            EtiquetaBarraFria = result;
            return EtiquetaBarraFria;

        }
        [Obsolete]
        public string AgregarComplementos(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesComplementos.Values.Sum();
            var result = EtiquetaComplementos;

            if (countIngredientesSeleccionados >= configuracionPresentacion.CantidadComplementos)
            {
                if (countIngredientesSeleccionados < MaximoComplementos)
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Los complementos extras tienen un costo adicional de {ingredienteEnsalada.Precio:C}"
                    });
                    //Ensalada.Precio = Ensalada.Precio + ingredienteEnsalada.Precio;

                    if (Ensalada.CantidadIngredientesComplementos.ContainsKey(ingredienteEnsalada.Id))
                    {
                        Ensalada.CantidadIngredientesComplementos[ingredienteEnsalada.Id]++;
                    }
                    else
                    {
                        Ensalada.CantidadIngredientesComplementos.Add(ingredienteEnsalada.Id, 1);
                    }
                }
                else
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Solo puedes agregar {CantidadesEnsaladaActual.CantidadComplementos} complementos por ensalada"

                    });
                }
            }
            else
            {
                if (Ensalada.CantidadIngredientesComplementos.ContainsKey(ingredienteEnsalada.Id))
                {
                    Ensalada.CantidadIngredientesComplementos[ingredienteEnsalada.Id]++;
                }
                else
                {
                    Ensalada.CantidadIngredientesComplementos.Add(ingredienteEnsalada.Id, 1);
                }
            }


            var finalCount = Ensalada.CantidadIngredientesComplementos.Values.Sum();
            if (finalCount > configuracionPresentacion.CantidadComplementos)
            {
                var extraCount = finalCount - configuracionPresentacion.CantidadComplementos;
                result =
                    $"{configuracionPresentacion.CantidadComplementos}/{configuracionPresentacion.CantidadComplementos} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{configuracionPresentacion.CantidadComplementos}";
            }

            EtiquetaComplementos = result;
            return EtiquetaComplementos;
        }
        [Obsolete]
        public string RemoverComplementos(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesComplementos.Values.Sum();
            var result = EtiquetaComplementos;

            if (Ensalada.CantidadIngredientesComplementos.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > configuracionPresentacion.CantidadComplementos)
                {
                    //Ensalada.Precio = Ensalada.Precio - ingredienteEnsalada.Precio;
                }
                Ensalada.CantidadIngredientesComplementos[ingredienteEnsalada.Id]--;
                if (Ensalada.CantidadIngredientesComplementos[ingredienteEnsalada.Id] <= 0)
                {
                    Ensalada.CantidadIngredientesComplementos.Remove(ingredienteEnsalada.Id);
                }
            }
            else
            {
                return result;
            }

            var finalCount = Ensalada.CantidadIngredientesComplementos.Values.Sum();
            if (finalCount > configuracionPresentacion.CantidadComplementos)
            {
                var extraCount = finalCount - configuracionPresentacion.CantidadComplementos;
                result =
                    $"{configuracionPresentacion.CantidadComplementos}/{configuracionPresentacion.CantidadComplementos} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{configuracionPresentacion.CantidadComplementos}";
            }

            EtiquetaComplementos = result;
            return EtiquetaComplementos;

        }
        [Obsolete]
        public string AgregarCortesias(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesCortesias.Values.Sum();
            var result = EtiquetaCortesias;

            if (countIngredientesSeleccionados >= configuracionPresentacion.CantidadCortesias)
            {
                if (countIngredientesSeleccionados < MaximoCantidadCortesias)
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Las cortesías extras tienen un costo adicional de {ingredienteEnsalada.Precio:C} MXN"
                    });
                    //Ensalada.Precio = Ensalada.Precio + ingredienteEnsalada.Precio;

                    if (Ensalada.CantidadIngredientesCortesias.ContainsKey(ingredienteEnsalada.Id))
                    {
                        //Ensalada.CantidadIngredientesCortesias[ingredienteEnsalada.Id] ++;
                    }
                    else
                    {
                        Ensalada.CantidadIngredientesCortesias.Add(ingredienteEnsalada.Id, 1);
                    }
                }
                else
                {
                    OnExtraAdded?.Invoke(this, new BaseEventArgs
                    {
                        Success = true,
                        Message = $"Solo puedes agregar {CantidadesEnsaladaActual.CantidadCortesias} cortesías por ensalada"

                    });
                }

            }
            else
            {
                if (Ensalada.CantidadIngredientesCortesias.ContainsKey(ingredienteEnsalada.Id))
                {
                    //Ensalada.CantidadIngredientesCortesias[ingredienteEnsalada.Id] ++;
                }
                else
                {
                    Ensalada.CantidadIngredientesCortesias.Add(ingredienteEnsalada.Id, 1);
                }
            }

            var finalCount = Ensalada.CantidadIngredientesCortesias.Values.Sum();
            if (finalCount > configuracionPresentacion.CantidadCortesias)
            {
                var extraCount = finalCount - configuracionPresentacion.CantidadCortesias;
                result =
                    $"{configuracionPresentacion.CantidadCortesias}/{configuracionPresentacion.CantidadCortesias} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{configuracionPresentacion.CantidadCortesias}";
            }

            EtiquetaCortesias = result;
            return EtiquetaCortesias;
        }
        [Obsolete]
        public string RemoverCortesias(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesCortesias.Values.Sum();
            var result = EtiquetaCortesias;

            if (Ensalada.CantidadIngredientesCortesias.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > configuracionPresentacion.CantidadCortesias)
                {
                    //Ensalada.Precio = Ensalada.Precio - ingredienteEnsalada.Precio;
                }
                Ensalada.CantidadIngredientesCortesias[ingredienteEnsalada.Id]--;
                if (Ensalada.CantidadIngredientesCortesias[ingredienteEnsalada.Id] <= 0)
                {
                    Ensalada.CantidadIngredientesCortesias.Remove(ingredienteEnsalada.Id);
                }
            }
            else
            {
                return result;
            }

            var finalCount = Ensalada.CantidadIngredientesCortesias.Values.Sum();
            if (finalCount > configuracionPresentacion.CantidadCortesias)
            {
                var extraCount = finalCount - configuracionPresentacion.CantidadCortesias;
                result =
                    $"{configuracionPresentacion.CantidadCortesias}/{configuracionPresentacion.CantidadCortesias} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{configuracionPresentacion.CantidadCortesias}";
            }

            EtiquetaComplementos = result;
            return EtiquetaComplementos;

        }
        [Obsolete]
        public string AgregarExtras(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesExtra.Values.Sum();

            if (countIngredientesSeleccionados < MaximoExtras)
            {
                OnExtraAdded?.Invoke(this, new BaseEventArgs
                {
                    Success = true,
                    Message = $"Las ingredientes extras tienen un costo adicional de {ingredienteEnsalada.Precio:C} MXN"
                });

                //Ensalada.Precio = Ensalada.Precio + ingredienteEnsalada.Precio;

                if (Ensalada.CantidadIngredientesExtra.ContainsKey(ingredienteEnsalada.Id))
                {
                    Ensalada.CantidadIngredientesExtra[ingredienteEnsalada.Id]++;
                }
                else
                {
                    Ensalada.CantidadIngredientesExtra.Add(ingredienteEnsalada.Id, 1);
                }
            }
            else
            {
                OnExtraAdded?.Invoke(this, new BaseEventArgs
                {
                    Success = true,
                    Message = $"Solo puedes agregar {MaximoCantidadExtras} por ensalada"
                });
            }



            var finalCount = Ensalada.CantidadIngredientesExtra.Values.Sum();

            EtiquetaExtras = $"{finalCount}"; ;
            return EtiquetaExtras;
        }
        [Obsolete]
        public string RemoverExtras(IngredienteEnsalada ingredienteEnsalada)
        {
            var configuracionPresentacion = _configuracionesEnsaladas
                .First(c => c.Id == (int)Ensalada.Presentacion);

            if (Ensalada.CantidadIngredientesExtra.ContainsKey(ingredienteEnsalada.Id))
            {
                //Ensalada.Precio = Ensalada.Precio - ingredienteEnsalada.Precio;
                Ensalada.CantidadIngredientesExtra[ingredienteEnsalada.Id]--;
                if (Ensalada.CantidadIngredientesExtra[ingredienteEnsalada.Id] <= 0)
                {
                    Ensalada.CantidadIngredientesExtra.Remove(ingredienteEnsalada.Id);
                }
            }
            else
            {
                return EtiquetaExtras;
            }

            var finalCount = Ensalada.CantidadIngredientesExtra.Values.Sum();
            EtiquetaExtras = $"{finalCount}";
            return EtiquetaExtras;

        }
        [Obsolete]
        public void SeleccionarAderezo(AderezoEnsalada aderezo)
        {
            Ensalada.Aderezo = aderezo;
        }
        [Obsolete]
        public void SeleccionarComplemento(ComplementosEnsalada seleccion)
        {
            Ensalada.Complemento = seleccion;
        }
        public bool TerminarEnsalada()
        {
            var ensalada = new EnsaladaCarrito
            {
                Id = Ensalada.IdEnsalada,
                Precio = Ensalada.Precio,
                Presentacion = Ensalada.Presentacion,
                Ingredientes = new Dictionary<int, int>(),
                Extras = new Dictionary<int, int>(),
                Nombre = Ensalada.Presentacion.Humanize(),
            };
            var cantidadProteina = CantidadesEnsaladaActual.CantidadProteina;
            foreach (var ingrediente in Ensalada.CantidadIngredientesProteina.Keys) //Calcula y separa las proteinas extras
            {
                var cantidad = Ensalada.CantidadIngredientesProteina[ingrediente];
                if (cantidadProteina >= cantidad)
                {
                    cantidadProteina = cantidadProteina - cantidad;
                    ensalada.Ingredientes.Add(ingrediente, cantidad);
                }
                else
                {
                    if (cantidadProteina > 0)
                    {
                        var extras = cantidad - cantidadProteina;
                        ensalada.Ingredientes.Add(ingrediente, cantidadProteina);
                        ensalada.Extras.Add(ingrediente, extras);
                        ensalada.Precio = ensalada.Precio + _ingredientes.First(c => c.Id == ingrediente).Precio * extras;

                        cantidadProteina = 0;
                    }
                    else
                    {
                        ensalada.Extras.Add(ingrediente, cantidad);
                        ensalada.Precio = ensalada.Precio + _ingredientes.First(c => c.Id == ingrediente).Precio * cantidad;
                    }
                }
            }
            var cantidadAderezos = CantidadesEnsaladaActual.CantidadAderezos;
            foreach (var ingrediente in Ensalada.CantidadIngredientesAderezos.Keys) //Calcula y separa los aderezos extras
            {
                var cantidad = Ensalada.CantidadIngredientesAderezos[ingrediente];
                if (cantidadAderezos >= cantidad)
                {
                    cantidadAderezos = cantidadAderezos - cantidad;
                    ensalada.Ingredientes.Add(ingrediente, cantidad);
                }
                else
                {
                    if (cantidadAderezos > 0)
                    {
                        var extras = cantidad - cantidadAderezos;
                        ensalada.Ingredientes.Add(ingrediente, cantidadAderezos);
                        ensalada.Extras.Add(ingrediente, extras);
                        cantidadAderezos = 0;
                        ensalada.Precio = ensalada.Precio + _ingredientes.First(c => c.Id == ingrediente).Precio * extras;

                    }
                    else
                    {
                        ensalada.Extras.Add(ingrediente, cantidad);
                        ensalada.Precio = ensalada.Precio + _ingredientes.First(c => c.Id == ingrediente).Precio * cantidad;

                    }
                }
            }
            var cantidadBarra = CantidadesEnsaladaActual.CantidadBarraFria;
            foreach (var ingrediente in Ensalada.CantidadIngredientesBarra.Keys) //Calcula y separa los items de barra fria extras
            {
                var cantidad = Ensalada.CantidadIngredientesBarra[ingrediente];
                if (cantidadBarra >= cantidad)
                {
                    cantidadBarra = cantidadBarra - cantidad;
                    ensalada.Ingredientes.Add(ingrediente, cantidad);
                }
                else
                {
                    if (cantidadBarra > 0)
                    {
                        var extras = cantidad - cantidadBarra;
                        ensalada.Ingredientes.Add(ingrediente, cantidadBarra);
                        ensalada.Extras.Add(ingrediente, extras);
                        cantidadBarra = 0;
                        ensalada.Precio = ensalada.Precio + _ingredientes.First(c => c.Id == ingrediente).Precio * extras;

                    }
                    else
                    {
                        ensalada.Extras.Add(ingrediente, cantidad);
                        ensalada.Precio = ensalada.Precio + _ingredientes.First(c => c.Id == ingrediente).Precio * cantidad;

                    }
                }
            }
            var cantidadComplementos = CantidadesEnsaladaActual.CantidadComplementos;
            foreach (var ingrediente in Ensalada.CantidadIngredientesComplementos.Keys) //Calcula y separa los complementos extras
            {
                var cantidad = Ensalada.CantidadIngredientesComplementos[ingrediente];
                if (cantidadComplementos >= cantidad)
                {
                    cantidadComplementos = cantidadComplementos - cantidad;
                    ensalada.Ingredientes.Add(ingrediente, cantidad);
                }
                else
                {
                    if (cantidadComplementos > 0)
                    {
                        var extras = cantidad - cantidadComplementos;
                        ensalada.Ingredientes.Add(ingrediente, cantidadComplementos);
                        ensalada.Extras.Add(ingrediente, extras);
                        cantidadComplementos = 0;
                        ensalada.Precio = ensalada.Precio + _ingredientes.First(c => c.Id == ingrediente).Precio * extras;

                    }
                    else
                    {
                        ensalada.Extras.Add(ingrediente, cantidad);
                        ensalada.Precio = ensalada.Precio + _ingredientes.First(c => c.Id == ingrediente).Precio * cantidad;

                    }
                }
            }
            foreach (var ingrediente in Ensalada.CantidadIngredientesExtra.Keys) //Calcula y separa los extras
            {
                var cantidad = Ensalada.CantidadIngredientesExtra[ingrediente];
                ensalada.Extras.Add(ingrediente, cantidad);
                ensalada.Precio = ensalada.Precio + _ingredientes.First(c => c.Id == ingrediente).Precio * cantidad;

            }
            foreach (var ingrediente in Ensalada.CantidadIngredientesCortesias.Keys) //Calcula y separa los cortesias
            {
                var cantidad = Ensalada.CantidadIngredientesCortesias[ingrediente];
                ensalada.Ingredientes.Add(ingrediente, cantidad);
            }

            ensalada.Contenido = ObtenerContenido(ensalada.Ingredientes, ensalada.Extras, Ensalada.Presentacion, Ensalada.Aderezo, Ensalada.Complemento, Ensalada.Tortilla);
            ReiniciarFlujo();
            return CarritoViewModel.Instance.AgregarEnsaladaACarrito(ensalada, true);
        }

        private string ObtenerContenido(Dictionary<int, int> ingredientes, Dictionary<int, int> extras, PresentacionEnsalada presentacion, AderezoEnsalada aderezo, ComplementosEnsalada complementos, Models.HazTuWrap.TortillaWrap tortilla)
        {
            var contenido = new List<string>
            {
                presentacion.Humanize(),
                aderezo.Humanize(),
                complementos.Humanize(),
                tortilla.Humanize()
            };
            foreach (var ingredienteCantidad in ingredientes)
            {
                var ingrediente = _ingredientes.First(c => c.Id == ingredienteCantidad.Key);
                contenido.Add(ingredienteCantidad.Value > 1
                    ? $"{ingredienteCantidad.Value}x {ingrediente.Descripcion}"
                    : $"{ingrediente.Descripcion}");
            }

            if (extras.Count <= 0) return string.Join(", ", contenido);

            foreach (var extrasCantidad in extras)
            {
                var ingrediente = _ingredientes.First(c => c.Id == extrasCantidad.Key);
                contenido.Add(extrasCantidad.Value > 1
                    ? $"Extra: {extrasCantidad.Value}x {ingrediente.Descripcion}"
                    : $"Extra: {ingrediente.Descripcion}");
            }
            return string.Join(", ", contenido);
        }

        public void ReiniciarFlujo()
        {
            Ensalada = null;
            EtiquetaProteina = string.Empty;
            EtiquetaAderezos = string.Empty;
            EtiquetaBarraFria = string.Empty;
            EtiquetaComplementos = string.Empty;
            EtiquetaCortesias = string.Empty;
            EtiquetaExtras = string.Empty;

            EnsaladaPasoUnoViewModel.Instance.EtiquetaExtras = string.Empty;
            EnsaladaPasoUnoViewModel.Instance.EtiquetaProteina = string.Empty;
            EnsaladaPasoDosViewModel.Instance.EtiquetaBarraFria = string.Empty;
            EnsaladaPasoTresViewModel.Instance.EtiquetaComplementos = string.Empty;
            EnsaladaPasoTresViewModel.Instance.EtiquetaCortesias = string.Empty;
            EnsaladaPasoCuatroViewModel.Instance.EtiquetaAderezos = string.Empty;

        }
        #endregion
    }
}
