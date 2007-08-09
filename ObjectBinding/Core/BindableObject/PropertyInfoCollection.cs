using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class PropertyInfoCollection : KeyedCollection<string, PropertyInfo>
  {
    public PropertyInfoCollection ()
    {
    }

    protected override string GetKeyForItem (PropertyInfo item)
    {
      return item.Name;
    }

    public PropertyInfo[] ToArray ()
    {
      return ArrayUtility.Convert (Items);
    }
  }
}