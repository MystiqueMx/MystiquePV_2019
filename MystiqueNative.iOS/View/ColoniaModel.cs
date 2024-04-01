using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace MystiqueNative.iOS.View
{
    class ColoniaModel : UIPickerViewModel
    {
        public string[] names = new string[] {
            "SELECCIONE...",
            "MEXICALI",
            "TIJUANA"
        };

        private UITextField ColoniaLabel;

        public ColoniaModel(UITextField ColoniaLabel)
        {
            this.ColoniaLabel = ColoniaLabel;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return names.Length;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return names[(int)row].ToString();
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            ColoniaLabel.Text = names[pickerView.SelectedRowInComponent(0)];
        }
    }
}