using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace Rubicon.Collections
{

/// <summary>
/// Case-sensitive name/object dictionary.
/// </summary>
[Serializable]
public class NameObjectCollection: NameObjectCollectionBase
{
  public NameObjectCollection()
    : base (null, null)
  {
  }

  public NameObjectCollection (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }

  public object this[string name]
  {
    get { return BaseGet (name); }
    set { BaseSet (name, value); }
  }

  public void Clear()
  {
    BaseClear();
  }

  public void Remove (string name)
  {
    BaseRemove (name);
  }
}

}
