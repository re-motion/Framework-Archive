using System;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Reflection;
using System.Resources;

using Rubicon.Utilities;

namespace Rubicon.Globalization
{

/// <summary>
///   A wrapper for the .net Framework <c>ResourceManager</c> implementation.
/// </summary>
/// <remarks>
///   <para>
///     Limited to accessing string resources.
///     Limited on resources for the current UI culture and it's less specific cultures.
///   </para><para>
///     If multiple Resource Managers are added which belonging to derived types, 
///     make sure to sort the resource managers in the order of inheritance before wrapping them.
///   </para>
/// </remarks>
public class ResourceManagerWrapper: IResourceManager, IList
{
  // member fields

  /// <summary> Wrapped resource managers </summary>
  private ResourceManager [] _resourceManagers;

  /// <summary> BaseNames of the wrapped resource managers as a comma seperated list. </summary>
  private string _baseNameList = "";

  // construction and disposing

  /// <summary>
  ///   Simple Constructor
  /// </summary>
  /// <include file='doc\include\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/Constructor/param[@name="resourceManager"]' />
  public ResourceManagerWrapper (ResourceManager resourceManager)
    : this (new ResourceManager[] { resourceManager } )
  {
  }

  /// <summary>
  ///   Constructor for wrapping multipe resource managers
  /// </summary>
  /// <include file='doc\include\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/Constructor/param[@name="resourceManagers"]' />
  public ResourceManagerWrapper (ResourceManager [] resourceManagers)
  {
    ArgumentUtility.CheckNotNullOrEmpty("resourceManagers", resourceManagers);

    _resourceManagers = resourceManagers;

    //  Do null reference checking
    //  and build comma seperated list of BaseNames
    string[] resourceManagerNames = new string[resourceManagers.Length];
    for (int index = 0; index < resourceManagers.Length; index++)
    {
      //Debug.Assert (resourceManagers[index] != null, "resourceManagers[index] != null");
      if (resourceManagers[index] == null)
        throw new ArgumentNullException ("resourceManagers[" + index + "]");

      resourceManagerNames[index] = resourceManagers[index].BaseName;
    }

    _baseNameList = StringUtility.ConcatWithSeperator (resourceManagerNames, ", ");
  }

  // methods and properties

  /// <summary>
  ///   The wrapped <c>ResourceManager</c> instances. Is Read Only.
  /// </summary>
  /// <remarks>
  ///   Always contains at least one ResourceManager and no null references.
  /// </remarks>
  public ResourceManager this [int index]
  {
    get 
    {
      if (index < 0 || index >= _resourceManagers.Length) throw new ArgumentOutOfRangeException ("index");

      return _resourceManagers[index];
    }
    set
    {
      throw new NotSupportedException ("The list of ResourceMangers is read only");
    }
  }

  /// <summary>
  ///   Gets the root names of the resource files that the <c>IResourceManager</c>
  ///   searches for resources. Multiple roots are seperated by a comma.
  /// </summary>
  public string BaseNameList
  {
    get { return _baseNameList; }
  }

  /// <summary>
  ///   Returns all string resources inside the wrapped resource managers.
  /// </summary>
  /// <returns>
  ///   A collection of string pairs, the key being the resource's ID, the vale being the string.
  /// </returns>
  public NameValueCollection GetAllStrings()
  {
    return GetAllStrings ("");
  }

  /// <summary>
  ///   Searches for all string resources inside the resource manager whose name is prefixed 
  ///   with a matching tag.
  /// </summary>
  /// <include file='doc\include\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/GetAllStrings/remarks' />
  /// <param name="prefix">
  ///   The prefix all returned string resources must have.
  /// </param>
  /// <returns>
  ///   A collection of string pairs, the key being the resource's ID, the vale being the string.
  /// </returns>
  public NameValueCollection GetAllStrings (string prefix)
  {
    if (prefix == null || prefix == String.Empty)
      prefix = "";

    NameValueCollection result = new NameValueCollection();

    //  Loop through all entries in the resource managers
    //  Copy the resources into a collection

    CultureInfo[] cultureHierarchy = GetCultureHierarchy (ResourceManagerWrapper.GetUICulture());

    // Loop from most neutral to current UICulture
    foreach (CultureInfo culture in cultureHierarchy)
    {
      for (int index = 0; index < _resourceManagers.Length; index++)
      {
        ResourceSet resourceSet = _resourceManagers[index].GetResourceSet (
          culture, true, true);

        foreach (DictionaryEntry entry in resourceSet)
        {
          string key = (string)entry.Key;

          if (key.StartsWith (prefix))
            result[key] = (string)entry.Value;
        }

      }
    }

    return result;
  }

  /// <summary>
  ///   Gets the value of the specified String resource.
  /// </summary>
  /// <include file='doc\include\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/GetAllStrings/remarks' />
  /// <param name="id">
  ///   The ID of the resource to get. Must not be <see langname="null"/>.
  /// </param>
  /// <returns>
  ///   The value of the resource. If a match is not possible, <see langname="null"/> is returned.
  /// </returns>
  public string GetString (string id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    string result = null;

    //  Loop through the resource managers and look for a resource with a matching ID
    //  Return the found resource

    for (int index = 0; index < _resourceManagers.Length; index++)
    {
      //  Implicit fallback to more neutral cultures if no match found
      result = _resourceManagers[index].GetString (id, GetUICulture());
    }

    return result;
  }

  /// <summary>
  ///   Returns the threads current UI culture.
  /// </summary>
  /// <returns>The UI culture of the thread.</returns>
  private static CultureInfo GetUICulture ()
  {
    return Thread.CurrentThread.CurrentUICulture;
  }

  /// <summary>
  ///   Returns the culture hierarch, starting with the most specialized culture
  /// </summary>
  /// <param name="mostSpecialized">
  ///   The starting point for walking the culture tree upwards. Must not be <see langame="null"/>.
  /// </param>
  /// <returns>
  ///   The cultures, starting with the most specialized, ending with the invariant culture.
  /// </returns>
  private static CultureInfo[] GetCultureHierarchy (CultureInfo mostSpecialized)
  {
    ArrayList hierarchyTopDown = new ArrayList();
    
    CultureInfo currentLevel = mostSpecialized;

    do {
      hierarchyTopDown.Add (currentLevel);
      currentLevel = currentLevel.Parent;
    } while (currentLevel != CultureInfo.InvariantCulture);

    CultureInfo[] hierarchyBottomUp = new CultureInfo[hierarchyTopDown.Count];

    hierarchyTopDown.Reverse();

    return (CultureInfo[])hierarchyTopDown.ToArray (typeof (CultureInfo));
  }

  #region IList Members

  public bool IsReadOnly
  {
    get { return true; }
  }

  object System.Collections.IList.this[int index]
  {
    get { return this[index]; }
    set { throw new NotSupportedException ("The list of ResourceMangers is read only"); }
  }

  public void RemoveAt (int index)
  {
    throw new NotSupportedException ("The list of ResourceMangers is read only");
  }

  public void Insert (int index, object value)
  {
    throw new NotSupportedException ("The list of ResourceMangers is read only");
  }

  public void Remove (object value)
  {
    throw new NotSupportedException ("The list of ResourceMangers is read only");
  }

  public bool Contains (object value)
  {
    return (IndexOf (value) == -1) ? false : true;
  }

  public void Clear ()
  {
    throw new NotSupportedException ("The list of ResourceMangers is read only");
  }

  public int IndexOf (object value)
  {
    int index = -1;

    for (; index < _resourceManagers.Length; index++)
    {
      if (_resourceManagers[index].Equals(value))
        break;
    }

    //  Item found before end of list?
    if (index < _resourceManagers.Length)
      return index;
    else
      return -1;
  }

  public int Add (object value)
  {
    throw new NotSupportedException ("The list of ResourceMangers is read only");
    return 0;
  }

  public bool IsFixedSize
  {
    get { return true; }
  }

  #endregion

  #region ICollection Members

  public bool IsSynchronized
  {
    get { return _resourceManagers.IsSynchronized; }
  }

  public int Count
  {
    get { return _resourceManagers.Length; }
  }

  public void CopyTo (Array array, int index)
  {
    _resourceManagers.CopyTo (array, index);
  }

  public object SyncRoot
  {
    get { return _resourceManagers.SyncRoot; }
  }

  #endregion

  #region IEnumerable Members

  public IEnumerator GetEnumerator ()
  {
    return _resourceManagers.GetEnumerator();
  }

  #endregion
}

}
