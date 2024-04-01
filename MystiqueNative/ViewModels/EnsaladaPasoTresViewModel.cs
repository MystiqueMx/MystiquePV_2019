using MystiqueNative.Helpers;
using MystiqueNative.Models.Ensaladas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MystiqueNative.ViewModels
{
    public class EnsaladaPasoTresViewModel : BaseViewModel
    {
        #region SINGLETON
        public static EnsaladaPasoTresViewModel Instance => _instance ?? (_instance = new EnsaladaPasoTresViewModel());
        private static EnsaladaPasoTresViewModel _instance;
        public static void Destroy() => _instance = null;
        #endregion
        #region EVENTS
        public event EventHandler<BaseEventArgs> OnExtraAdded;
        #endregion
        #region FIELDS

        public Ensalada Ensalada => EnsaladasViewModel.Instance.Ensalada;

        public List<IngredienteEnsalada> ListaCortesias => EnsaladasViewModel.Instance.Configuracion.IngredientesCortesias;
        public List<IngredienteEnsalada> ListaComplementos => EnsaladasViewModel.Instance.Configuracion.IngredientesComplementos;

        public List<int> PrimerGrupoComplementos => new List<int> { 65, 66 };
        public List<int> SegundoGrupoComplementos => new List<int> { 53, 54 };
        public string EtiquetaComplementos { get; set; }
        public string EtiquetaCortesias { get; set; }

        //ENSALADA ACTUAL

        public int CantidadIngredientesComplementos => Ensalada.CantidadIngredientesComplementos.Sum(c => c.Value);
        public int CantidadIngredientesCortesias => Ensalada.CantidadIngredientesCortesias.Sum(c => c.Value);

        //CONFIGURACION ACTUAL
        private const int MaximoExtras = 5;

        public ConfiguracionEnsalada CantidadesEnsaladaActual =>
            EnsaladasViewModel.Instance._configuracionesEnsaladas.FirstOrDefault(c => c.Id == (int)Ensalada.Presentacion);
        public int MaximoComplementos => CantidadesEnsaladaActual.CantidadComplementos;
        public int MaximoCantidadCortesias => CantidadesEnsaladaActual.CantidadCortesias;

        //VALIDACIONES PASOS

        public bool HaCompleadoTercerPaso => true;

        #endregion
        #region API
        public string AgregarComplementos(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesComplementos.Values.Sum();
            var result = EtiquetaComplementos;

            if (PrimerGrupoComplementos.Contains(ingredienteEnsalada.Id))
            {
                if (Ensalada.CantidadIngredientesComplementos.Keys.Any(c => PrimerGrupoComplementos.Contains(c)))
                {
                    //OnExtraAdded?.Invoke(this, new BaseEventArgs
                    //{
                    //    Success = true,
                    //    Message = $"Debes elegir entre "
                    //});
                    return result;
                }
            }
            else if (SegundoGrupoComplementos.Contains(ingredienteEnsalada.Id))
            {
                if (Ensalada.CantidadIngredientesComplementos.Keys.Any(c => SegundoGrupoComplementos.Contains(c)))
                {
                    return result;
                }
            }

            if (countIngredientesSeleccionados >= CantidadesEnsaladaActual.CantidadComplementos)
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
                        //Ensalada.CantidadIngredientesComplementos[ingredienteEnsalada.Id]++;
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
            if (finalCount > CantidadesEnsaladaActual.CantidadComplementos)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadComplementos;
                result =
                    $"{CantidadesEnsaladaActual.CantidadComplementos}/{CantidadesEnsaladaActual.CantidadComplementos} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadComplementos}";
            }

            EtiquetaComplementos = result;
            return EtiquetaComplementos;
        }
        public string RemoverComplementos(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesComplementos.Values.Sum();
            var result = EtiquetaComplementos;

            if (Ensalada.CantidadIngredientesComplementos.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > CantidadesEnsaladaActual.CantidadComplementos)
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
            if (finalCount > CantidadesEnsaladaActual.CantidadComplementos)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadComplementos;
                result =
                    $"{CantidadesEnsaladaActual.CantidadComplementos}/{CantidadesEnsaladaActual.CantidadComplementos} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadComplementos}";
            }

            EtiquetaComplementos = result;
            return EtiquetaComplementos;

        }
        public string AgregarCortesias(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesCortesias.Values.Sum();
            var result = EtiquetaCortesias;

            if (countIngredientesSeleccionados >= CantidadesEnsaladaActual.CantidadCortesias)
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
            if (finalCount > CantidadesEnsaladaActual.CantidadCortesias)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadCortesias;
                result =
                    $"{CantidadesEnsaladaActual.CantidadCortesias}/{CantidadesEnsaladaActual.CantidadCortesias} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadCortesias}";
            }

            EtiquetaCortesias = result;
            return EtiquetaCortesias;
        }
        public string RemoverCortesias(IngredienteEnsalada ingredienteEnsalada)
        {
            var countIngredientesSeleccionados = Ensalada.CantidadIngredientesCortesias.Values.Sum();
            var result = EtiquetaCortesias;

            if (Ensalada.CantidadIngredientesCortesias.ContainsKey(ingredienteEnsalada.Id))
            {
                if (countIngredientesSeleccionados > CantidadesEnsaladaActual.CantidadCortesias)
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
            if (finalCount > CantidadesEnsaladaActual.CantidadCortesias)
            {
                var extraCount = finalCount - CantidadesEnsaladaActual.CantidadCortesias;
                result =
                    $"{CantidadesEnsaladaActual.CantidadCortesias}/{CantidadesEnsaladaActual.CantidadCortesias} | Extra : {extraCount}";
            }
            else
            {
                result =
                    $"{finalCount}/{CantidadesEnsaladaActual.CantidadCortesias}";
            }

            EtiquetaCortesias = result;
            return EtiquetaCortesias;

        }
        public void SeleccionarComplemento(ComplementosEnsalada seleccion)
        {
            Ensalada.Complemento = seleccion;
        }
        public void ReiniciarPaso() => EnsaladasViewModel.Instance.ReiniciarPasoTres();

        #endregion
    }
}
