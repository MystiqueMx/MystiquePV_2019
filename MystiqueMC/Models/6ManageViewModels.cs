// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.VerifyPhoneNumberViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System.ComponentModel.DataAnnotations;


namespace MystiqueMC.Models
{
  public class VerifyPhoneNumberViewModel
  {
    [Required]
    [Display(Name = "Code")]
    public string Code { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
  }
}
