using System;
using System.Collections;

namespace Rubicon.ObjectBinding
{
  //TODO: doc
  public interface IListInfo
  {
    Type ItemType { get; }
    bool RequiresWriteBack { get; }
    IList CreateList (int count);
    void InsertItem (object item, int index);
    void RemoveItem (object item);
  }
}