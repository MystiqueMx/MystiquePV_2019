// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.DataAnnotation.RequiredIfIntEqualsAttribute
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;


namespace MystiqueMC.Helpers.DataAnnotation
{
  public class RequiredIfIntEqualsAttribute : RequiredAttribute
  {
    private readonly int _target;
    private const string UnknownProperty = "The property {0} could not be found.";

    protected string OtherProperty { get; }

    public RequiredIfIntEqualsAttribute(int target, string otherProperty)
    {
      this.OtherProperty = !string.IsNullOrWhiteSpace(otherProperty) ? otherProperty : throw new ArgumentNullException(string.Format("{0} cannot but null or empty.", (object) nameof (otherProperty)));
      this._target = target;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      PropertyInfo property = validationContext.ObjectType.GetProperty(this.OtherProperty);
      if (property == (PropertyInfo) null)
        return new ValidationResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The property {0} could not be found.", (object) this.OtherProperty));
      int result;
      if (!int.TryParse(property.GetValue(validationContext.ObjectInstance).ToString(), out result))
        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
      return result == this._target && !this.IsValid(value) ? new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName)) : ValidationResult.Success;
    }
  }
}
