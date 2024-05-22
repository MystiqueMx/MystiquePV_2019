// Decompiled with JetBrains decompiler
// Type: MystiqueMC.DateTimeExtensions
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;
using System.Globalization;
using System.Threading;


namespace MystiqueMC
{
  public static class DateTimeExtensions
  {
    public static DateTime FirstDayOfWeek(this DateTime dt)
    {
      CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
      int num = dt.DayOfWeek - currentCulture.DateTimeFormat.FirstDayOfWeek;
      if (num < 0)
        num += 7;
      return dt.AddDays((double) -num).Date;
    }

    public static DateTime LastDayOfWeek(this DateTime dt) => dt.FirstDayOfWeek().AddDays(6.0);

    public static DateTime FirstDayOfMonth(this DateTime dt) => new DateTime(dt.Year, dt.Month, 1);

    public static DateTime LastDayOfMonth(this DateTime dt)
    {
      DateTime dateTime = dt.FirstDayOfMonth();
      dateTime = dateTime.AddMonths(1);
      return dateTime.AddDays(-1.0);
    }

    public static DateTime FirstDayOfNextMonth(this DateTime dt)
    {
      return dt.FirstDayOfMonth().AddMonths(1);
    }
  }
}
