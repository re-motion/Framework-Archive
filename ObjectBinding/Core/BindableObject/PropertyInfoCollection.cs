using System;
using System.Collections.ObjectModel;
using System.Reflection;

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
  }
}