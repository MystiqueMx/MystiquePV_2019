using MystiqueNative.Helpers;
using MystiqueNative.Models.Ensaladas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MystiqueNative.ViewModels
{
    public class EnsaladaPasoUnoViewModel : BaseViewModel
    {
        #region SINGLETON
        public static EnsaladaPasoUnoViewModel Instance => _instance ?? (_instance = new EnsaladaPasoUnoViewModel());
        private static EnsaladaPasoUnoViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        #region EVENTS
        public event EventHandler<BaseEventArgs> OnExtraAdded;
        #endregion
        #region FIELDS

        public Ensalada Ensalada => EnsaladasViewModel.Instance.Ensalada;

        public List<IngredienteEnsalada> ListaProteinas => EnsaladasViewModel.Instance.Configuracion.IngredientesProteina;
        public List<IngredienteEnsalada> ListaExtras => EnsaladasViewModel.Instance.Configuracion.IngredientesExtras;

        public string EtiquetaProteina { get; set; }
        public string EtiquetaExtras { get; set; }

        public int CantidadIngredientesProteina => Ensalada.CantidadIngredientesProteina.Sum(c => c.Value);
        public int CantidadIngredientesExtra => Ensalada.CantidadIngredientesExtra.Sum(c => c.Value);

        //CONFIGURACION ACTUAL
        private const int MaximoExtras = 5;

        public ConfiguracionEnsalada CantidadesEnsaladaActual =>
            EnsaladasViewModel.Instance._configuracionesEnsaladas.FirstOrDefault(c => c.Id == (int)Ensalada.Presentacion);
        public int MaximoProteinas => CantidadesEnsaladaActual.CantidadProteina + MaximoExtras;
        public int MaximoCantidadExtras => MaximoExtras;

        //VALIDACIONES PASOS

        public bool HaCompleadoPrimerPaso => CantidadIngredientesProteina >= CantidadesEnsaladaActual?.CantidadProteina;

        #endregion
        #region API

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
                var extraPrice = extraCount * ListaProteinas.First().Precio;

                result =
                    $"{CantidadesEnsaladaActual.CantidadProteina}/{CantidadesEnsaladaActual.CantidadProteina} | Extra : {extraCount} - {extraPrice:C}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadProteina}";
            }

            EtiquetaProteina = result;
            return EtiquetaProteina;
        }
        public string RemoverProteina(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesProteina.Values.Sum();
            var result = EtiquetaProteina;

            if (Ensalada.CantidadIngredientesProteina.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > CantidadesEnsaladaActual.CantidadProteina)
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
            if (finalCount > CantidadesEnsaladaActual.CantidadProteina)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadProteina;
                var extraPrice = extraCount * ListaProteinas.First().Precio;
                result =
                    $"{CantidadesEnsaladaActual.CantidadProteina}/{CantidadesEnsaladaActual.CantidadProteina} | Extra : {extraCount} - {extraPrice:C}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadProteina}";
            }

            EtiquetaProteina = result;
            return EtiquetaProteina;

        }
        public string AgregarExtras(IngredienteEnsalada ingredienteEnsalada)
        {
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
            var extraPrice = ListaExtras.First().Precio * finalCount;
            EtiquetaExtras = $"{finalCount} - {extraPrice:C}";
            return EtiquetaExtras;
        }
        public string RemoverExtras(IngredienteEnsalada ingredienteEnsalada)
        {

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
            var extraPrice = ListaExtras.First().Precio * finalCount;
            EtiquetaExtras = $"{finalCount} - {extraPrice:C}";
            return EtiquetaExtras;

        }
        public void ReiniciarPaso() => EnsaladasViewModel.Instance.ReiniciarPasoUno();

        #endregion
    }
}
