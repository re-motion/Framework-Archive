using System;

namespace Rubicon
{

/// <summary>
/// Provides utility functions that make common array operations easier.
/// </summary>
public sealed class ArrayUtility
{
  public static bool IsNullOrEmpty (System.Array array)
  {
    return (array == null) || (array.Length == 0);
  }

  public static bool IsNullOrEmpty (System.Collections.ICollection collection)
  {
    return (collection == null) || (collection.Count == 0);
  }

	private ArrayUtility()
	{
	}
}

}
