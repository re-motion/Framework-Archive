using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{
  public class PropertyCollection:KeyedCollection<string, PropertyBase>
  {
    public PropertyCollection (IList<PropertyBase> properties)
    {
      foreach (PropertyBase property in properties)
        Add (property);      
    }

    ///<summary>
    ///When implemented in a derived class, extracts the key from the specified element.
    ///</summary>
    ///
    ///<returns>
    ///The key for the specified element.
    ///</returns>
    ///
    ///<param name="item">The element from which to extract the key.</param>
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