using MystiqueNative.Helpers;
using MystiqueNative.Models.Ensaladas;
using MystiqueNative.Models.HazTuWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MystiqueNative.ViewModels
{
    public class EnsaladaPasoCuatroViewModel : BaseViewModel
    {
        #region SINGLETON
        public static EnsaladaPasoCuatroViewModel Instance => _instance ?? (_instance = new EnsaladaPasoCuatroViewModel());
        private static EnsaladaPasoCuatroViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        #region EVENTS
        public event EventHandler<BaseEventArgs> OnExtraAdded;
        #endregion
        #region FIELDS

        public Ensalada Ensalada => EnsaladasViewModel.Instance.Ensalada;
        public List<IngredienteEnsalada> ListaAderezos => EnsaladasViewModel.Instance.Configuracion.IngredientesAderezos;

        public string EtiquetaAderezos { get; set; }

        //ENSALADA ACTUAL
        public int CantidadIngredientesAderezos => Ensalada.CantidadIngredientesAderezos.Sum(c => c.Value);

        //CONFIGURACION ACTUAL
        private const int MaximoExtras = 5;

        private IEnumerable<ConfiguracionEnsalada> ConfiguracionesEnsaladas =>
            EnsaladasViewModel.Instance._configuracionesEnsaladas;
        public ConfiguracionEnsalada CantidadesEnsaladaActual =>
            EnsaladasViewModel.Instance._configuracionesEnsaladas.FirstOrDefault(c => c.Id == (int)Ensalada.Presentacion);
        public int MaximoAderezos => CantidadesEnsaladaActual.CantidadAderezos + MaximoExtras;

        //VALIDACIONES PASOS

        public bool HaCompletadoCuartoPaso => CantidadIngredientesAderezos > 0;

        #endregion
        #region API

        public string AgregarAderezo(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesAderezos.Values.Sum();
            var result = EtiquetaAderezos;

            if (countIngredientesSeleccionados >= CantidadesEnsaladaActual.CantidadAderezos)
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
            if (finalCount > CantidadesEnsaladaActual.CantidadAderezos)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadAderezos;
                var extraPrice = extraCount * ListaAderezos.First().Precio;
                result =
                    $"{CantidadesEnsaladaActual.CantidadAderezos}/{CantidadesEnsaladaActual.CantidadAderezos} | Extra : {extraCount} - {extraPrice}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadAderezos}";
            }

            EtiquetaAderezos = result;
            return EtiquetaAderezos;
        }
        public string RemoverAderezo(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesAderezos.Values.Sum();
            var result = EtiquetaAderezos;

            if (Ensalada.CantidadIngredientesAderezos.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > CantidadesEnsaladaActual.CantidadAderezos)
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
            if (finalCount > CantidadesEnsaladaActual.CantidadAderezos)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadAderezos;
                var extraPrice = extraCount * ListaAderezos.First().Precio;

                result =
                    $"{CantidadesEnsaladaActual.CantidadAderezos}/{CantidadesEnsaladaActual.CantidadAderezos} | Extra : {extraCount} - {extraPrice}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadAderezos}";
            }

            EtiquetaAderezos = result;
            return EtiquetaAderezos;

        }
        public void SeleccionarAderezo(AderezoEnsalada aderezo)
        {
            Ensalada.Aderezo = aderezo;
        }

        /* Exclusivo de Haz Wrap */
        public void SeleccionarTortilla(TortillaWrap tortilla)
        {
            Ensalada.Tortilla = tortilla;
        }

        public bool TerminarEnsalada() => EnsaladasViewModel.Instance.TerminarEnsalada();
        public void ReiniciarFlujo() => EnsaladasViewModel.Instance.ReiniciarFlujo();
        public void ReiniciarPaso() => EnsaladasViewModel.Instance.ReiniciarPasoCuatro();

        #endregion
    }
}
