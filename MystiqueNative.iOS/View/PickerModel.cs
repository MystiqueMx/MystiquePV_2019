using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace MystiqueNative.iOS.View
{
    public class PickerModel: UIPickerViewModel
    {
        List<string> Values;
        public EventHandler ValueChanged;
        public string SelectedValue;
        public nint ItemSelectedValue;

        public PickerModel(List<string> Values)
        {
            this.Values = Values;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return Values.Count;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return Values[(int)row];
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            var values = Values[(int)row];
            ItemSelectedValue = component;
            SelectedValue = values;
            ValueChanged?.Invoke(null, null);

        }

    }
}