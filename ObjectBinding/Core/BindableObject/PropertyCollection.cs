using System;
using System.Collections.ObjectModel;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class PropertyCollection : KeyedCollection<string, PropertyBase>
  {
    public PropertyCollection ()
    {
    }

    protected override string GetKeyForItem (PropertyBase item)
    {
      return item.Identifier;
    }

    public PropertyBase[] ToArray ()
    {
      return ArrayUtility.Convert (Items);
    }
  }
}