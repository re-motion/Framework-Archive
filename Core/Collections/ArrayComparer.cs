using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Collections
{
  public class ArrayComparer<T> : IEqualityComparer<T[]>
  {
    public bool Equals (T[] x, T[] y)
    {
      if (ReferenceEquals (x, y))
        return true;

      if (ReferenceEquals (x, null) || ReferenceEquals (y, null))
        return false;

      if (x.Rank != 1 || y.Rank != 1)
        throw new NotSupportedException ("ArrayComparer does not support multidimensional arrays.");

      if (x.Length != y.Length)
        return false;

      for (int i = 0; i < x.Length; ++i)
      {
        if (!Equals (x[i], y[i]))
          return false;
      }

      return true;
    }

    public int GetHashCode (T[] array)
    {
      if (ReferenceEquals (array, null))
        return 0;

      if (array.Rank != 1)
        throw new NotSupportedException ("ArrayComparer does not support multidimensional arrays.");

      int hc = 0;
      for (int i = 0; i < array.Length; ++i)
      {
        T item = array[i];
        if (item != null)
          hc ^= item.GetHashCode ();
      }
      return hc;
    }
  }
}
