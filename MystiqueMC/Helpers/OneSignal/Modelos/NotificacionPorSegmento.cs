// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.OneSignal.Modelos.NotificacionPorSegmento
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll


namespace MystiqueMC.Helpers.OneSignal.Modelos
{
  public class NotificacionPorSegmento : NotificacionBase
  {
    public string[] included_segments { get; set; }

    public object contents { get; set; }

    public object headings { get; set; }
  }
}
