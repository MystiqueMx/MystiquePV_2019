// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Sucursal.EstatusDatosFiscales
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\proyectomystique\publish\bin\MystiqueMC.dll

using System.ComponentModel;


namespace MystiqueMC.Models.Sucursal
{
  public enum EstatusDatosFiscales
  {
    [Description("No registrados")] Vacio,
    [Description("Datos cargados anteriormente")] Catalogo,
    [Description("Nuevos datos fiscales")] Nuevos,
  }
}
