using System;
using System.Collections.Generic;

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
      Rubicon.Utilities.ArgumentUtility.CheckNotNull ("source", source);

      foreach (TSource item in source)
        yield return selector (item);
    }

    public static TResult[] SelectToArray<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      if (source == null)
        return null;

      IList<TSource> sourceList = ToList (source);

      TResult[] result = new TResult[sourceList.Count];
      for (int i = 0; i < sourceList.Count; ++i)
        result[i] = selector (sourceList[i]);

      return result;
    }

    public static IList<T> ToList<T> (IEnumerable<T> source)
    {
      IList<T> list = source as IList<T>;
      if (list != null)
        return list;
      else
        return new List<T> (source);
    }

    public static T[] ToArray<T> (IEnumerable<T> source)
    {
      T[] array = source as T[];
      if (array != null)
        return array;
      else
        return new List<T> (source).ToArray();
    }
  }
}