using MystiqueNative.Helpers;
using MystiqueNative.Models.Ensaladas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MystiqueNative.ViewModels
{
    public class EnsaladaPasoDosViewModel : BaseViewModel
    {
        #region SINGLETON
        public static EnsaladaPasoDosViewModel Instance => _instance ?? (_instance = new EnsaladaPasoDosViewModel());
        private static EnsaladaPasoDosViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        #region EVENTS
        public event EventHandler<BaseEventArgs> OnExtraAdded;
        #endregion

        #region FIELDS

        public Ensalada Ensalada => EnsaladasViewModel.Instance.Ensalada;
        public List<IngredienteEnsalada> ListaBarraFria => EnsaladasViewModel.Instance.Configuracion.IngredientesBarraFria;

        public string EtiquetaBarraFria { get; set; }

        //ENSALADA ACTUAL
        public int CantidadIngredientesBarra => Ensalada.CantidadIngredientesBarra.Sum(c => c.Value);
        //CONFIGURACION ACTUAL
        private const int MaximoExtras = 5;

        public ConfiguracionEnsalada CantidadesEnsaladaActual =>
            EnsaladasViewModel.Instance._configuracionesEnsaladas.FirstOrDefault(c => c.Id == (int)Ensalada.Presentacion);
        public int MaximoBarraFria => CantidadesEnsaladaActual.CantidadBarraFria + MaximoExtras;

        //VALIDACIONES PASOS

        public bool HaCompleadoSegundoPaso => CantidadIngredientesBarra >= CantidadesEnsaladaActual?.CantidadBarraFria;

        #endregion
        #region API


        public string AgregarBarraFria(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesBarra.Values.Sum();
            var result = EtiquetaBarraFria;

            if (countIngredientesSeleccionados >= CantidadesEnsaladaActual.CantidadBarraFria)
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
                        Message = $"Solo puedes agregar {MaximoBarraFria} ingredientes de barra fría por ensalada"

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
            if (finalCount > CantidadesEnsaladaActual.CantidadBarraFria)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadBarraFria;
                var extraPrice = extraCount * ListaBarraFria.First().Precio;
                result =
                    $"{CantidadesEnsaladaActual.CantidadBarraFria}/{CantidadesEnsaladaActual.CantidadBarraFria} | Extra : {extraCount} - {extraPrice:C}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadBarraFria}";
            }

            EtiquetaBarraFria = result;
            return EtiquetaBarraFria;
        }
        public string RemoverBarraFria(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesBarra.Values.Sum();
            var result = EtiquetaBarraFria;

            if (Ensalada.CantidadIngredientesBarra.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > CantidadesEnsaladaActual.CantidadBarraFria)
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
            if (finalCount > CantidadesEnsaladaActual.CantidadBarraFria)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadBarraFria;
                var extraPrice = extraCount * ListaBarraFria.First().Precio;

                result =
                    $"{CantidadesEnsaladaActual.CantidadBarraFria}/{CantidadesEnsaladaActual.CantidadBarraFria} | Extra : {extraCount} - {extraPrice:C}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadBarraFria}";
            }

            EtiquetaBarraFria = result;
            return EtiquetaBarraFria;

        }
        public void ReiniciarPaso() => EnsaladasViewModel.Instance.ReiniciarPasoDos();

        #endregion
    }
}
