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

    public new static Customer NewObject ()
    {
      return DomainObject.NewObject<Customer> ().With ();
    }

    protected Customer()
    {
    }

    protected Customer (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract CustomerType CustomerType { get; set; }

    public abstract DateTime CustomerSince { get; set; }

    [DBBidirectionalRelation ("Customers")]
    public abstract Region Region { get; set; }

    [DBBidirectionalRelation ("Customer")]
    public abstract ObjectList<Order> Orders { get; }
  }
}