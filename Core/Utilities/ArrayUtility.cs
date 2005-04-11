using System;
using System.Collections;
using Rubicon.Utilities;

namespace Rubicon.Utilities
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

  public static Array Combine (Type elementType, params Array[] arrays)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("arrays", arrays);
    if (arrays[0] == null)
      throw new ArgumentNullException ("arrays[0]");

    if (elementType == null)
      elementType = arrays[0].GetType().GetElementType();

    int newLength = 0;
    for (int i = 0; i < arrays.Length; ++i)
    {
      if (arrays[i].Rank != 1)
        throw new ArgumentException ("Combine does not support multi-dimensional arrays.", "array[" + i.ToString() + "]");
      if (! elementType.IsAssignableFrom (arrays[i].GetType().GetElementType()))
        elementType = typeof (object);
      newLength += arrays[i].Length;
    }

    Array result = Array.CreateInstance (elementType, newLength);
    int start = 0;
    for (int i = 0; i < arrays.Length; ++i)
    {
      arrays[i].CopyTo (result, start);
      start += arrays[i].Length;
    }
    return result;
  }

  public static Array Combine (params Array[] arrays)
  {
    return Combine (null, arrays);
  }

  public static Array Convert (Array array, Type elementType)
  {
    int rank = array.Rank;
    int[] lengths = new int[rank];
    int[] lowerBounds = new int[rank];
    for (int dimension = 0; dimension < rank; ++dimension)
    {
      lengths[dimension] = array.GetLength(dimension);
      lowerBounds[dimension] = array.GetLowerBound (dimension);
    }
    
    Array result = Array.CreateInstance (elementType, lengths, lowerBounds);
    array.CopyTo (result, 0);
    return result;
  }
  
  public static Array Convert (ICollection collection, Type elementType)
  {
    Array result = Array.CreateInstance (elementType, collection.Count);
    collection.CopyTo (result, 0);
    return result;
  }

  public static Array Insert (Array original, int index, object value)
  {
    if (original.Rank != 1)
      throw new ArgumentException ("Array rank must be 1.", "original");

    Array result = Array.CreateInstance (original.GetType().GetElementType(), original.Length + 1);

    for (int i = 0; i < index; ++i)
      result.SetValue (original.GetValue(i), i);

    result.SetValue (value, index);

    for (int i = index; i < original.Length; ++i)
      result.SetValue (original.GetValue(i), i + 1);

    return result;
  }

	private ArrayUtility()
	{
	}
}

}
