using System;
using System.Collections;
using Rubicon.ObjectBinding;

namespace Rubicon.Utilities
{

/// <summary>
/// Provides utility methods for processing IList instances. 
/// </summary>
public sealed class ListUtility
{
  private ListUtility()
  {
  }

  public static void CopyTo (IList source, IList destination)
  {
    int len = Math.Min (source.Count, destination.Count);
    for (int i = 0; i < len; ++i)
      destination[i] = source[i];
  }

  public static IList CreateList (IBusinessObjectReferenceProperty property, IList template, int size)
  {
    if (property != null)
      return property.CreateList (size);
    else if (template is Array)
      return Array.CreateInstance (template.GetType().GetElementType(), size);
    else 
      throw new NotSupportedException ("Cannot create instance if argument 'property' is null and 'template' is not an array.");
  }

  /// <summary>
  ///    Removes a range of values from a list and returns the resulting list. The original list may be modified.
  /// </summary>
  public static IList Remove (IBusinessObjectReferenceProperty property, IList list, IList objects, bool mustCreateCopy)
  {
    if (   list.IsFixedSize 
        || (mustCreateCopy && ! (list is ICloneable)))
    {
      ArrayList arrayList = new ArrayList (list);
      foreach (object obj in objects)
        arrayList.Remove (obj);

      IList newList = CreateList (property, list, arrayList.Count);
      CopyTo (arrayList, newList);
      return newList;
    }
    else
    {
      if (mustCreateCopy)
        list = (IList) ((ICloneable)list).Clone();

      foreach (object obj in objects)
        list.Remove (obj);
      return list;
    }
  }

  /// <summary>
  ///    Removes a range of values from a list and returns the resulting list. The original list may be modified.
  /// </summary>
  public static IList Remove (IBusinessObjectReferenceProperty property, IList list, object obj, bool mustCreateCopy)
  {
    if (   list.IsFixedSize 
        || (mustCreateCopy && ! (list is ICloneable)))
    {  
      int idx = list.IndexOf (obj);
      if (idx < 0)
        return list;
      
      IList newList = CreateList (property, list, list.Count - 1);

      for (int i = 0; i < idx; ++i)
        newList[i] = list[i];
      for (int i = idx; i < list.Count - 1; ++i)
        newList[i] = list[i + 1];
      return newList;
    }
    else
    {
      if (mustCreateCopy)
        list = (IList) ((ICloneable)list).Clone();

      list.Remove (obj);
      return list;
    }
  }
}

}
