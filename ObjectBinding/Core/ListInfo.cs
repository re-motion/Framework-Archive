using System;
using System.Collections;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{
  public class ListInfo : IListInfo
  {
    private readonly Type _itemType;

    public ListInfo (Type itemType)
    {
      ArgumentUtility.CheckNotNull ("itemType", itemType);
      _itemType = itemType;
    }

    public Type ItemType
    {
      get { return _itemType; }
    }

    public bool RequiresWriteBack
    {
      get { return true; }
    }

    public IList CreateList (int count)
    {
      return Array.CreateInstance (_itemType, count);
    }

    public void InsertItem (object item, int index)
    {
      throw new NotImplementedException();
    }

    public void RemoveItem (object item)
    {
      throw new NotImplementedException();
    }
  }
}