// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.ResetPasswordViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System.ComponentModel.DataAnnotations;

namespace MystiqueMC.Models
{
  public class ResetPasswordViewModel
  {
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} caracteres de longitud.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
  }
}
