using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MystiqueNative.Helpers
{
    public static class ValidatorHelper
    {
        public static bool IsValidEmail(string input)
        {
            return Regex.IsMatch(input, "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
        }
        public static bool IsValidPassword(string input)
        {
            return input.Length >= 6;
        }
        public static bool IsValidBirthday(string input)
        {
            var isDate = DateTime.TryParseExact(input,
                           "dd/MM/yyyy",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out var bd);
            return isDate && DateTime.Compare(DateTime.Now.Date, bd) > 0;
        }
        public static bool IsValidDay(string input)
        {
            if (int.TryParse(input, out int inputAsInt))
            {
                return inputAsInt > 0 && inputAsInt <= 31;
            }
            else
            {
                return false;
            }
        }
        public static bool IsValidYear(string input)
        {
            if (int.TryParse(input, out int inputAsInt))
            {
                return inputAsInt > 1900 && inputAsInt <= DateTime.Now.Year;
            }
            else
            {
                return false;
            }
        }
        public static bool IsValidMonth(string input)
        {
            if (int.TryParse(input, out int inputAsInt))
            {
                return inputAsInt > 0 && inputAsInt <= 12;
            }
            else
            {
                return false;
            }
        }
        public static bool IsValidName(string input)
        {
            return Regex.IsMatch(input, "^[\\p{L}\\s'.-]+$");
        }
        public static bool IsValidPhone(string input)
        {
            return Regex.IsMatch(input, "\\(?([0-9]{3})\\)?([ .-]?)([0-9]{3})\\2([0-9]{4})") && (input.Count() == 10);
        }
        public static bool IsValidRfc(string input)
        {
            return Regex.IsMatch(input, "^([A-Za-zÑñ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])([A-Z]|[0-9]){2}([A]|[0-9]){1})?$");
        }
        public static bool IsValidPostalCode(string input)
        {
            return input.Length == 5 
                && int.TryParse(input,out var cp)
                && cp > 0;
        }

        public static bool IsValidCreditCard(string input)
        {
            return Regex.IsMatch(input, "^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\\d{3})\\d{11})$");
        }

        public static bool IsValidCreditCardExpiration(string input)
        {
            var regions = input.Split('/');
            if (regions.Length != 2) return false;
            if (int.TryParse($"20{regions[1]}", out var year))
            {
                if (year < DateTime.Now.Year) return false;
                if (int.TryParse(regions[0], out var month))
                {
                    if (year == DateTime.Now.Year)
                    {
                        if (DateTime.Now.Month == 12)
                        {
                            return month == 12;
                        }
                        else
                        {
                            return month > DateTime.Now.Month && month <= 12;
                        }
                    }
                    else
                    {
                        return month > 0 && month <= 12;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsAmexCreditCard(string input)
        {
            return Regex.IsMatch(input, "^3[47][0-9]{13}$");
        }
        public static bool IsMastercardCreditCard(string input)
        {
            return Regex.IsMatch(input, "^5[1-5][0-9]{14}$");
        }
        public static bool IsVisaCreditCard(string input)
        {
            return Regex.IsMatch(input, "^4[0-9]{12}(?:[0-9]{3})?$")
            || Regex.IsMatch(input, "^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14})$");
        }
    }
}
