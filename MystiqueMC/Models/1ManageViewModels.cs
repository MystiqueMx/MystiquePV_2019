// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.ManageLoginsViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;


namespace MystiqueMC.Models
{
  public class ManageLoginsViewModel
  {
    public IList<UserLoginInfo> CurrentLogins { get; set; }

    public IList<AuthenticationDescription> OtherLogins { get; set; }
  }
}
