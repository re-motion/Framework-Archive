using System;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [NotAbstract]
  public abstract class Customer : Company
  {
    public enum CustomerType
    {
      Standard = 0,
      Premium = 1,
      Gold = 2
    }

    public new static Customer NewObject ()
    {
      return DomainObject.NewObject<Customer> ().With ();
    }

    protected Customer ()
    {
    }

    protected Customer (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public abstract NaDateTime CustomerSince { get; set; }

    [DBColumn ("CustomerType")]
    public abstract CustomerType Type { get; set; }

    [DBBidirectionalRelation ("Customer", SortExpression = "OrderNo asc")]
    public virtual OrderCollection Orders { get { return (OrderCollection) GetRelatedObjects(); } }
  }
}
