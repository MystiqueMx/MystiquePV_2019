// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.ControlInventariosResponse
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll


namespace MystiqueMC.Models
{
  public class ControlInventariosResponse
  {
    public bool isSuccess { get; set; }

    public bool hasError { get; set; }

    public string message { get; set; }

    public bool isRedirectToAction { get; set; }

    public string actionName { get; set; }

    public string controllerName { get; set; }

    public object routeValue { get; set; }
  }
}
