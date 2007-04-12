using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [NotAbstract]
  [DBTable]
  [TestDomain]
  public abstract class OrderItemWithNewPropertyAccess : DomainObject
  {
    public static OrderItemWithNewPropertyAccess Create ()
    {
      return DomainObject.Create<OrderItemWithNewPropertyAccess> ();
    }

    protected OrderItemWithNewPropertyAccess (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    public int Position
    {
      get { return (int) DataContainer["Position"]; }
      set { DataContainer["Position"] = value; }
    }

    public string Product
    {
      get { return (string) DataContainer["Product"]; }
      set { DataContainer["Product"] = value; }
    }

    [AutomaticProperty]
    [DBBidirectionalRelation ("OrderItems")]
    public abstract OrderWithNewPropertyAccess Order { get; set; }

    [StorageClassNone]
    public virtual OrderWithNewPropertyAccess OriginalOrder
    {
      get { return (OrderWithNewPropertyAccess) GetOriginalRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItemWithNewPropertyAccess.Order"); }
    }
  }
}
