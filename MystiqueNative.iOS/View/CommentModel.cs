using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;

namespace MystiqueNative.iOS.View
{
    public class CommentModel : UIPickerViewModel
    {
        public string[] names = new string[] {
            "Comentario",
            "Sugerencia",
            "Queja"
        };

        private UITextField personLabel;

        public CommentModel(UITextField personLabel)
        {
            this.personLabel = personLabel;
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
            personLabel.Text = names[pickerView.SelectedRowInComponent(0)];
        }


    }

}