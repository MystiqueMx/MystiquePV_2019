// Decompiled with JetBrains decompiler
// Type: MystiqueMC.SmsService
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using Microsoft.AspNet.Identity;
using System.Threading.Tasks;


namespace MystiqueMC
{
  public class SmsService : IIdentityMessageService
  {
    public Task SendAsync(IdentityMessage message) => (Task) Task.FromResult<int>(0);
  }
}
