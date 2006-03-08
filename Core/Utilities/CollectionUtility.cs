using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Utilities
{

/// <summary>
///   Utility classes for <see cref="ICollection"/>
/// </summary>
public class CollectionUtility
{
  /// <summary>
  ///   Merges two collections. If a key occurs in both collections, the value of the second collections is taken.
  /// </summary>
  public static NameValueCollection Merge (NameValueCollection first, NameValueCollection second)
  {
    if (first == null && second == null)
      return null;
    else if (first == null)
      return CollectionUtility.Clone (second);
    if (second == null)
      return CollectionUtility.Clone (first);

    NameValueCollection result = CollectionUtility.Clone (first);

    for (int i = 0; i < second.Count; i++)
      first.Set (second.GetKey(i), second.Get(i));

    return result;
  }

  /// <summary>
  ///   Adds the second dictionary to the first. If a key occurs in both dictionaries, the value of the second
  ///   dictionaries is taken.
  /// </summary>
  /// <param name="first"> Must not be <see langword="null"/>. </param>
  public static void Append (NameValueCollection first, NameValueCollection second)
  {
    ArgumentUtility.CheckNotNull ("first", first);
    
    if (second != null)
    {
      for (int i = 0; i < second.Count; i++)
        first.Set (second.GetKey(i), second.Get(i));
    }
  }

  public static NameValueCollection Clone (NameValueCollection collection)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);
    
    NameValueCollection result = new NameValueCollection();
      for (int i = 0; i < collection.Count; i++)
        result.Add (collection.GetKey(i), collection.Get(i));

    return result;
  }

  private CollectionUtility()
	{
	}
}

}
