using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MystiqueNative.Droid.Helpers;

namespace MystiqueNative.Droid.Fragments
{
    public class BirthdayPickerDialogFragment : AppCompatDialogFragment
    {
        private readonly DateTime _presetDate;
        private DatePicker _datePicker;
        public event EventHandler<BirthdayDialogEventArgs> DialogClosed;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialog_fragment_birthday_picker, container, false);

            view.FindViewById<Button>(Resource.Id.dialog_datepicker_close).Click += (s, ev) =>
            {
                //Cancel
                DialogClosed?.Invoke(this, null);
                Dismiss();
            };

            view.FindViewById<Button>(Resource.Id.dialog_datepicker_choose).Click+=(s,ev)=> 
            {
                //Save
                DialogClosed?.Invoke(this, new BirthdayDialogEventArgs { Day = _datePicker.DayOfMonth, Month = _datePicker.Month + 1, Year = _datePicker.Year });
                Dismiss();
            };

            _datePicker = view.FindViewById<DatePicker>(Resource.Id.fragments_datepicker);

            SetDatepickerBounds(_datePicker, _presetDate);

            return view;
        }

        private static void SetDatepickerBounds(DatePicker datePicker, DateTime presetDate)
        {
            datePicker.UpdateDate(presetDate.Year, presetDate.Month, presetDate.Day);
            datePicker.MaxDate =
                (long) DateTime.Now.Date.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            datePicker.MinDate =
                (long) presetDate.AddYears(-100).Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public BirthdayPickerDialogFragment(DateTime presetDate)
        {
            _presetDate = presetDate.ToUniversalTime();
        }
    }
    public class BirthdayDialogEventArgs
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}