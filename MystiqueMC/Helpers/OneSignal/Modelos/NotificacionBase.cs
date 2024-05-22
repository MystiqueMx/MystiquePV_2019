// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.OneSignal.Modelos.NotificacionBase
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll


namespace MystiqueMC.Helpers.OneSignal.Modelos
{
  public class NotificacionBase
  {
    public string app_id { get; set; }

    public string android_channel_id { get; set; }

    public string ios_badgeType { get; set; }

    public int ios_badgeCount { get; set; }

    public int ttl { get; set; }
  }
}
