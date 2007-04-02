using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  public class OrderCollection : ObjectList<Order>
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // standard constructor for collections
    public OrderCollection ()
    {
    }

    // standard constructor for collections
    public OrderCollection (OrderCollection collection, bool isCollectionReadOnly)
      : base (collection, isCollectionReadOnly)
    {
    }

    // methods and properties

    public new void SetIsReadOnly (bool isReadOnly)
    {
      base.SetIsReadOnly (isReadOnly);
    }
  }
}
