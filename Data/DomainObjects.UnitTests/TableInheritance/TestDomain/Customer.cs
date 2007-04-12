using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public enum CustomerType
  {
    Standard = 0,
    Premium = 1
  }

  [ClassID ("TI_Customer")]
  [NotAbstract]
  public abstract class Customer: Person
  {
    public new static Customer GetObject (ObjectID id)
    {
      return (Customer) DomainObject.GetObject (id);
    }

    public static Customer Create ()
    {
      return DomainObject.Create<Customer> ();
    }

    public Customer (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    public abstract CustomerType CustomerType { get; set; }

    public abstract DateTime CustomerSince { get; set; }

    [DBBidirectionalRelation ("Customers")]
    public abstract Region Region { get; set; }

    [DBBidirectionalRelation ("Customer")]
    public virtual ObjectList<Order> Orders { get { return (ObjectList<Order>) GetRelatedObjects(); } }
  }
}