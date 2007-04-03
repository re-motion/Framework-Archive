using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Utilities
{
  /// <summary>
  /// Provides helper functions for <see cref="IEnumerable{T}"/> objects.
  /// </summary>
  /// <remarks>
  /// Most of these methods will become obsolete with C# 3.0/LINQ.
  /// </remarks>
  public static class EnumerableUtility
  {
    public static IEnumerable<TResult> Select<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      foreach (TSource item in source)
        yield return selector (item);
    }

    public static TResult[] SelectToArray<TSource, TResult> (IList<TSource> source, Func<TSource, TResult> selector)
    {
      if (source == null)
        return null;

      TResult[] result = new TResult[source.Count];
      for (int i = 0; i < source.Count; ++i)
        result[i] = selector (source[i]);

      return result;
    }
  }
}
