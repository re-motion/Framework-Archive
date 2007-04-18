using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Address")]
  [DBTable (Name = "TableInheritance_Address")]
  [NotAbstract]
  [TestDomain]
  public abstract class Address : DomainObject
  {
    public static Address NewObject ()
    {
      return DomainObject.NewObject<Address> ().With();
    }

    protected Address ()
    {
    }

    protected Address (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Street { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 10)]
    public abstract string Zip { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string City { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Country { get; set; }

    [DBBidirectionalRelation ("Address", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Person Person { get; set; }
  }
}