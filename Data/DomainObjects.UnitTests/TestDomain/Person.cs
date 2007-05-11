using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Person : TestDomainBase
  {
    public static Person NewObject ()
    {
      return NewObject<Person>().With();
    }

    protected Person()
    {
    }

    protected Person (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("ContactPerson")]
    public abstract Partner AssociatedPartnerCompany { get; set; }
  }
}
