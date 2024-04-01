using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace MystiqueNative.Droid.Utils.Tarjetas
{
    /**
     * Formats the watched EditText to a credit card number
     */
    public class FourDigitCardTextWatcher : Java.Lang.Object, ITextWatcher
    {
        public const char Separator = ' ';
        public void AfterTextChanged(IEditable s)
        {
            // Remove spacing char
            if (s.Length() > 0 && (s.Length() % 5) == 0)
            {
                var c = s.CharAt(s.Length() - 1);
                if (Separator == c)
                {
                    s.Delete(s.Length() - 1, s.Length());
                }
            }
            // Insert char where needed.
            if (s.Length() > 0 && s.Length() % 5 == 0)
            {

                var lastCharAt = s.CharAt(s.Length() - 1);
                // Only if its a digit where there should be a space we insert a space
                if (Character.IsDigit(lastCharAt) && TextUtils.Split(s.ToString(), Separator.ToString()).Length <= 3)
                {
                    s.Insert(s.Length() - 1, Separator.ToString());
                }
            }
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
        }
    }
}