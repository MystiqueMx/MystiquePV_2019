// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Models.Gastos.AperturaViewModel
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;
using System.Collections.Generic;


namespace MystiqueMC.Models.Gastos
{
  public class AperturaViewModel
  {
    public int IdApertura { get; set; }

    public int AperturaIdSucursal { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistroApertura { get; set; }

    public DateTime FechaInicial { get; set; }

    public DateTime FechaFinal { get; set; }

    public Decimal TipoCambio { get; set; }

    public Decimal Fondo { get; set; }

    public string uuidApertura { get; set; }

    public int VentaId { get; set; }

    public string UsuarioRegistro { get; set; }

    public string UsuarioAutorizo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public List<GastosPvViewModel> GastosPv { get; set; }
  }
}
