// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.NotificacionesViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using MystiqueMC.DAL;
using System.Collections.Generic;


namespace MystiqueMC.Models
{
  public class NotificacionesViewModel
  {
    public IEnumerable<catRangoEdad> RangosEdades { get; set; }

    public IEnumerable<sucursales> Sucursales { get; set; }

    public IEnumerable<catSexos> Sexos { get; set; }

    public IEnumerable<notificaciones> NotificacionesAnteriores { get; set; }

    public IEnumerable<notificaciones> NotificacionesActuales { get; set; }
  }
}
