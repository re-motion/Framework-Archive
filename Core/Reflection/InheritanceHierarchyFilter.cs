using System;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.Reflection
{
  /// <summary>
  /// The <see cref="InheritanceHierarchyFilter"/> can be used to get all leaf classes within a deifned set of types passed into the 
  /// constructor.
  /// </summary>
  public class InheritanceHierarchyFilter
  {
    private readonly Type[] _types;

    public InheritanceHierarchyFilter (Type[] types)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("types", types);
      _types = types;
    }

    public Type[] GetLeafTypes ()
    {
      Set<Type> baseTypes = new Set<Type>();
      foreach (Type type in _types)
        baseTypes.Add (type.BaseType);

      return Array.FindAll (_types, delegate (Type type) { return !baseTypes.Contains (type); });
    }
  }
}