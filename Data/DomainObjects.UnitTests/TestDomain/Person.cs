using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class Person : TestDomainBase
  {
    public static new Person GetObject (ObjectID id)
    {
      return (Person) DomainObject.GetObject (id);
    }

    public static Person Create ()
    {
      return DomainObject.Create<Person>();
    }

    protected Person (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("ContactPerson")]
    public abstract Partner AssociatedPartnerCompany { get; set; }
  }
}
