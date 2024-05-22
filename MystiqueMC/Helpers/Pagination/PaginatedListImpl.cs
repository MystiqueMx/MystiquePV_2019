// Decompiled with JetBrains decompiler
// Type: MystiqueMC.Helpers.Pagination.PaginatedListImpl`1
// Assembly: MystiqueMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24F62E2F-C73B-47A1-AC91-0F22AE9440BB
// Assembly location: C:\Users\moise\OneDrive\Documents\mystique_web\bin\MystiqueMC.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace MystiqueMC.Helpers.Pagination
{
  public class PaginatedListImpl<T> : List<T>, IPaginatedList
  {
    private const int DEFAULT_PAGE_SIZE = 10;

    public PaginatedListImpl(List<T> items, int count, int pageIndex, int pageSize = 0)
    {
      if (pageSize == 0)
        pageSize = 10;
      this.PageIndex = pageIndex;
      this.TotalPages = (int) Math.Ceiling((double) count / (double) pageSize);
      this.AddRange((IEnumerable<T>) items);
    }

    public static PaginatedListImpl<T> Create(IQueryable<T> source, int pageIndex, int pageSize = 0)
    {
      if (pageSize == 0)
        pageSize = 10;
      int count = source.Count<T>();
      return new PaginatedListImpl<T>(source.Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).ToList<T>(), count, pageIndex, pageSize);
    }

    public bool HasNextPage() => this.PageIndex < this.TotalPages;

    public bool HasPreviousPage() => this.PageIndex > 1;

    int IPaginatedList.PageIndex() => this.PageIndex;

    int IPaginatedList.TotalPages() => this.TotalPages;

    public int PageIndex { get; private set; }

    public int TotalPages { get; private set; }
  }
}
