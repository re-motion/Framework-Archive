using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
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

    public new static Customer GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Customer> (id);
    }

    public new static Customer GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return Customer.GetObject (id);
      }
    }

    protected Customer ()
    {
    }

    public abstract DateTime? CustomerSince { get; set; }

    [DBColumn ("CustomerType")]
    public abstract CustomerType Type { get; set; }

    [DBBidirectionalRelation ("Customer", SortExpression = "OrderNo asc")]
    public abstract OrderCollection Orders { get; }
  }
}
