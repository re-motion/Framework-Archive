using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Specialized;

namespace Rubicon.Collections
{

[Serializable]
public class NameObjectCollection: NameObjectCollectionBase
{
  public NameObjectCollection()
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
}

}
