using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  public class OrderCollection : ObjectList<Order>
  {
    public OrderCollection ()
    {
    }

    // standard constructor for collections
    public OrderCollection (OrderCollection collection, bool isCollectionReadOnly)
      : base (collection, isCollectionReadOnly)
    {
    }

    public new void SetIsReadOnly (bool isReadOnly)
    {
      base.SetIsReadOnly (isReadOnly);
    }
  }
}
