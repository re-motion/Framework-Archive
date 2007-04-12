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

    public new static Customer GetObject (ObjectID id)
    {
      return (Customer) DomainObject.GetObject (id);
    }

    public new static Customer GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Customer) DomainObject.GetObject (id, clientTransaction);
    }

    public new static Customer Create ()
    {
      return DomainObject.Create<Customer> ();
    }

    protected Customer (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    public abstract NaDateTime CustomerSince { get; set; }

    [DBColumn ("CustomerType")]
    public abstract CustomerType Type { get; set; }

    [DBBidirectionalRelation ("Customer", SortExpression = "OrderNo asc")]
    public virtual OrderCollection Orders { get { return (OrderCollection) GetRelatedObjects(); } }
  }
}
