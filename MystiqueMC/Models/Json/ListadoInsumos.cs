﻿// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Json.ListadoInsumos
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System.Collections.Generic;


namespace MystiqueMC.Models.Json
{
  public class ListadoInsumos : BaseJsonResult
  {
    public List<ItemListadoInsumos> resultado { get; set; }
  }
}