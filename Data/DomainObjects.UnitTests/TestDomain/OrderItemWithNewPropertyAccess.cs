using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [TestDomain]
  public abstract class OrderItemWithNewPropertyAccess : DomainObject
  {
    public static OrderItemWithNewPropertyAccess NewObject ()
    {
      return NewObject<OrderItemWithNewPropertyAccess> ().With();
    }

    protected OrderItemWithNewPropertyAccess()
    {
    }

    protected OrderItemWithNewPropertyAccess (DataContainer dataContainer)
        : base (dataContainer)
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
