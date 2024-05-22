using System;
using System.Globalization;
using System.Text;

namespace ApiDoc.Helpers.Facturacion.Helpers
{
    /// <summary>
    /// Convierte números en su expresión numérica a su numeral cardinal
    /// <example>
    ///     var numalet = new Numalet{ LetraCapital = true, ConvertirDecimales = true, Decimales = 2 };
    ///     numalet.ToCustomCardinal(1.00);
    /// </example>
    /// <see cref="https://varionet.wordpress.com/2007/11/29/convertir-numeros-a-letras/"/>
    /// </summary>
    public sealed class Numalet
    {
        #region Miembros estáticos

        private const int Uni = 0, Dieci = 1, Decena = 2, Centena = 3;
        private static readonly string[,] Matriz = {
            {null," uno", " dos", " tres", " cuatro", " cinco", " seis", " siete", " ocho", " nueve"},
            {" diez"," once"," doce"," trece"," catorce"," quince"," dieciséis"," diecisiete"," dieciocho"," diecinueve"},
            {null,null,null," treinta"," cuarenta"," cincuenta"," sesenta"," setenta"," ochenta"," noventa"},
            {null,null,null,null,null," quinientos",null," setecientos",null," novecientos"}
        };

        private const char Sub = (char)26;
        //Cambiar acá si se quiere otro comportamiento en los métodos de clase
        public const string SeparadorDecimalSalidaDefault = "con";
        public const string MascaraSalidaDecimalDefault = "00'/100.-'";
        public const int DecimalesDefault = 2;
        public const bool LetraCapitalDefault = false;
        public const bool ConvertirDecimalesDefault = false;
        public const bool ApocoparUnoParteEnteraDefault = false;
        public const bool ApocoparUnoParteDecimalDefault = false;

        #endregion

        #region Propiedades

        private int _decimales = DecimalesDefault;
        private string _separadorDecimalSalida = SeparadorDecimalSalidaDefault;
        private int _posiciones = DecimalesDefault;
        private string _mascaraSalidaDecimal, _mascaraSalidaDecimalInterna = MascaraSalidaDecimalDefault;
        private bool _esMascaraNumerica = true;
        private bool _convertirDecimales = ConvertirDecimalesDefault;

        /// <summary>
        /// Indica la cantidad de decimales que se pasarán a entero para la conversión
        /// </summary>
        /// <remarks>Esta propiedad cambia al cambiar MascaraDecimal por un valor que empieze con '0'</remarks>
        public int Decimales
        {
            get => _decimales;
            set
            {
                if (value > 10) throw new ArgumentException(value.ToString() + " excede el número máximo de decimales admitidos, solo se admiten hasta 10.");
                _decimales = value;
            }
        }

        /// <summary>
        /// Objeto CultureInfo utilizado para convertir las cadenas de entrada en números
        /// </summary>
        public CultureInfo CultureInfo { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Indica la cadena a intercalar entre la parte entera y la decimal del número
        /// </summary>
        public string SeparadorDecimalSalida
        {
            get => _separadorDecimalSalida;
            set
            {
                _separadorDecimalSalida = value;
                //Si el separador decimal es compuesto, infiero que estoy cuantificando algo,
                //por lo que apocopo el "uno" convirtiéndolo en "un"
                ApocoparUnoParteEntera = value.Trim().IndexOf(" ", StringComparison.Ordinal) > 0;
            }
        }

        /// <summary>
        /// Indica el formato que se le dara a la parte decimal del número
        /// </summary>
        public string MascaraSalidaDecimal
        {
            get => !string.IsNullOrEmpty(_mascaraSalidaDecimal) ? _mascaraSalidaDecimal : "";
            set
            {
                //determino la cantidad de cifras a redondear a partir de la cantidad de '0' o '#' 
                //que haya al principio de la cadena, y también si es una máscara numérica
                var i = 0;
                while (i < value.Length
                       && (value[i] == '0')
                       | value[i] == '#')
                    i++;
                _posiciones = i;
                if (i > 0)
                {
                    _decimales = i;
                    _esMascaraNumerica = true;
                }
                else _esMascaraNumerica = false;
                _mascaraSalidaDecimal = value;
                if (_esMascaraNumerica)
                {
                    _mascaraSalidaDecimalInterna = value.Substring(0, _posiciones) + "'" + value.Substring(_posiciones)
                                                       .Replace("''", Sub.ToString()).Replace("'", string.Empty)
                                                       .Replace(Sub.ToString(), "'") + "'";
                }
                else
                    _mascaraSalidaDecimalInterna = value
                        .Replace("''", Sub.ToString())
                        .Replace("'", string.Empty)
                        .Replace(Sub.ToString(), "'");
            }
        }

        /// <summary>
        /// Indica si la primera letra del resultado debe estár en mayúscula
        /// </summary>
        public bool LetraCapital { get; set; } = LetraCapitalDefault;

        /// <summary>
        /// Indica si se deben convertir los decimales a su expresión nominal
        /// </summary>
        public bool ConvertirDecimales
        {
            get => _convertirDecimales;
            set
            {
                _convertirDecimales = value;
                ApocoparUnoParteDecimal = value;
                if (value)
                {// Si la máscara es la default, la borro
                    if (_mascaraSalidaDecimal == MascaraSalidaDecimalDefault)
                        MascaraSalidaDecimal = "";
                }
                else if (string.IsNullOrEmpty(_mascaraSalidaDecimal))
                    //Si no hay máscara dejo la default
                    MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            }
        }

        /// <summary>
        /// Indica si de debe cambiar "uno" por "un" en las unidades.
        /// </summary>
        public bool ApocoparUnoParteEntera { get; set; } = false;

        /// <summary>
        /// Determina si se debe apococopar el "uno" en la parte decimal
        /// </summary>
        /// <remarks>El valor de esta propiedad cambia al setear ConvertirDecimales</remarks>
        public bool ApocoparUnoParteDecimal { get; set; }

        #endregion

        #region Constructores

        public Numalet()
        {
            MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            SeparadorDecimalSalida = SeparadorDecimalSalidaDefault;
            LetraCapital = LetraCapitalDefault;
            ConvertirDecimales = _convertirDecimales;
        }

        public Numalet(bool convertirDecimales, string mascaraSalidaDecimal, string separadorDecimalSalida, bool letraCapital)
        {
            if (!string.IsNullOrEmpty(mascaraSalidaDecimal))
                MascaraSalidaDecimal = mascaraSalidaDecimal;
            if (!string.IsNullOrEmpty(separadorDecimalSalida))
                _separadorDecimalSalida = separadorDecimalSalida;
            LetraCapital = letraCapital;
            _convertirDecimales = convertirDecimales;
        }
        #endregion

        #region Conversores de instancia

        public string ToCustomCardinal(double numero)
        { return Convertir((decimal)numero, _decimales, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, LetraCapital, _convertirDecimales, ApocoparUnoParteEntera, ApocoparUnoParteDecimal); }

        public string ToCustomCardinal(string numero)
        {
            if (double.TryParse(numero, NumberStyles.Float, CultureInfo, out var dNumero))
                return ToCustomCardinal(dNumero);
            else throw new ArgumentException("'" + numero + "' no es un número válido.");
        }

        public string ToCustomCardinal(decimal numero)
        { return ToCardinal((numero)); }

        public string ToCustomCardinal(int numero)
        { return Convertir(numero, 0, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, LetraCapital, _convertirDecimales, ApocoparUnoParteEntera, false); }

        #endregion

        #region Conversores estáticos

        public static string ToCardinal(int numero)
        {
            return Convertir(numero, 0, null, null, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        public static string ToCardinal(double numero)
        {
            return ToCardinal((decimal)numero);
        }

        public static string ToCardinal(string numero, CultureInfo referenciaCultural)
        {
            if (double.TryParse(numero, NumberStyles.Float, referenciaCultural, out var dNumero))
                return ToCardinal(dNumero);
            else throw new ArgumentException("'" + numero + "' no es un número válido.");
        }

        public static string ToCardinal(string numero)
        {
            return ToCardinal(numero, CultureInfo.CurrentCulture);
        }

        public static string ToCardinal(decimal numero)
        {
            return Convertir(numero, DecimalesDefault, SeparadorDecimalSalidaDefault, MascaraSalidaDecimalDefault, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        #endregion

        private static string Convertir(decimal numero, int decimales, string separadorDecimalSalida, string mascaraSalidaDecimal, bool esMascaraNumerica, bool letraCapital, bool convertirDecimales, bool apocoparUnoParteEntera, bool apocoparUnoParteDecimal)
        {
            var resultado = new StringBuilder();

            var num = (long)Math.Abs(numero);

            if (num >= 1000000000000 || num < 0)
                throw new ArgumentException("El número '" + numero.ToString(CultureInfo.InvariantCulture) +
                                            "' excedió los límites del conversor: [0;1.000.000.000.000)");
            if (num == 0)
            {
                resultado.Append(" cero");
            }
            else
            {
                var iTerna = 0;
                while (num > 0)
                {
                    iTerna++;
                    var cadTerna = string.Empty;
                    var terna = (int)(num % 1000);

                    var centenaTerna = terna / 100;
                    var decenaTerna = terna % 100;
                    var unidadTerna = terna % 10;

                    if ((decenaTerna > 0) && (decenaTerna < 10))
                    {
                        cadTerna = Matriz[Uni, unidadTerna] + cadTerna;
                    }
                    else if ((decenaTerna >= 10) && (decenaTerna < 20))
                    {
                        cadTerna = cadTerna + Matriz[Dieci, unidadTerna];
                    }
                    else if (decenaTerna == 20)
                    {
                        cadTerna = cadTerna + " veinte";
                    }
                    else if ((decenaTerna > 20) && (decenaTerna < 30))
                    {
                        cadTerna = " veinti" + Matriz[Uni, unidadTerna].Substring(1);
                    }
                    else if ((decenaTerna >= 30) && (decenaTerna < 100))
                    {
                        if (unidadTerna != 0)
                        {
                            cadTerna = Matriz[Decena, decenaTerna / 10] + " y" + Matriz[Uni, unidadTerna] + cadTerna;
                        }
                        else
                        {
                            cadTerna += Matriz[Decena, decenaTerna / 10];
                        }
                    }

                    switch (centenaTerna)
                    {
                        case 1:
                            if (decenaTerna > 0) cadTerna = " ciento" + cadTerna;
                            else cadTerna = " cien" + cadTerna;
                            break;
                        case 5:
                        case 7:
                        case 9:
                            cadTerna = Matriz[Centena, terna / 100] + cadTerna;
                            break;
                        default:
                            if (terna / 100 > 1) cadTerna = Matriz[Uni, terna / 100] + "cientos" + cadTerna;
                            break;
                    }
                    //Reemplazo el 'uno' por 'un' si no es en las únidades o si se solicító apocopar
                    if ((iTerna > 1 | apocoparUnoParteEntera) && decenaTerna == 21)
                    {
                        cadTerna = cadTerna.Replace("veintiuno", "veintiún");
                    }
                    else if ((iTerna > 1 | apocoparUnoParteEntera) && unidadTerna == 1 && decenaTerna != 11)
                    {
                        cadTerna = cadTerna.Substring(0, cadTerna.Length - 1);
                    }
                    //Acentúo 'veintidós', 'veintitrés' y 'veintiséis'
                    else if (decenaTerna == 22)
                    {
                        cadTerna = cadTerna.Replace("veintidos", "veintidós");
                    }
                    else if (decenaTerna == 23)
                    {
                        cadTerna = cadTerna.Replace("veintitres", "veintitrés");
                    }
                    else if (decenaTerna == 26)
                    {
                        cadTerna = cadTerna.Replace("veintiseis", "veintiséis");
                    }

                    //Completo miles y millones
                    if (iTerna == 3)
                    {
                        if (numero < 2000000)
                        {
                            cadTerna += " millón";
                        }
                        else
                        {
                            cadTerna += " millones";
                        }
                    }
                    else if (iTerna == 2 || iTerna == 4)
                    {
                        if (terna > 0)
                        {
                            cadTerna += " mil";
                        }
                    }

                    resultado.Insert(0, cadTerna);
                    num = (int)(num / 1000);
                } //while
            }

            //Se agregan los decimales si corresponde
            if (decimales > 0)
            {
                resultado.Append(" " + separadorDecimalSalida + " ");
                var enteroDecimal = (int)Math.Round((double)(numero - (long)numero) * Math.Pow(10, decimales), 0);
                if (convertirDecimales)
                {
                    resultado.Append(Convertir(enteroDecimal,
                                         0,
                                         null,
                                         null,
                                         esMascaraNumerica,
                                         false,
                                         false,
                                         (apocoparUnoParteDecimal && !esMascaraNumerica /*&& !esMascaraDecimalDefault*/),
                                         false) +
                                     " "
                                     + (esMascaraNumerica ? "" : mascaraSalidaDecimal));
                }
                else
                if (esMascaraNumerica)
                {
                    resultado.Append(enteroDecimal.ToString(mascaraSalidaDecimal));
                }
                else
                {
                    resultado.Append(enteroDecimal.ToString() + " " + mascaraSalidaDecimal);
                }
            }
            //Se pone la primer letra en mayúscula si corresponde y se retorna el resultado
            return letraCapital
                ? resultado[1].ToString().ToUpper() + resultado.ToString(2, resultado.Length - 2)
                : resultado.ToString().Substring(1);
        }
    }
}