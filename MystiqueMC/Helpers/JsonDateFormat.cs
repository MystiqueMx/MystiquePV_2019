// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.NewtonsoftJsonExtensions
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using Newtonsoft.Json;
using System.Web.Mvc;


namespace MystiqueMC.Helpers
{
  public static class NewtonsoftJsonExtensions
  {
    public static ActionResult ToJsonResult(this object obj)
    {
      return (ActionResult) new ContentResult()
      {
        Content = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Ignore
        }),
        ContentType = "application/json"
      };
    }
  }
}
