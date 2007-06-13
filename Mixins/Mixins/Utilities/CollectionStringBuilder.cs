using System;
using System.Collections.Generic;
using Rubicon;
using Rubicon.Text;

namespace Mixins.Utilities
{
  public static class CollectionStringBuilder
  {
    public static string BuildCollectionString<T> (IEnumerable<T> collection, string separator, Func<T, string> itemFormatter)
    {
      SeparatedStringBuilder sb = new SeparatedStringBuilder (separator);
      foreach (T t in collection)
        sb.Append (itemFormatter (t));
      return sb.ToString();
    }
  }
}
