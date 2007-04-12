using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_OrganizationalUnit")]
  [DBTable (Name = "TableInheritance_OrganizationalUnit")]
  [NotAbstract]
  public abstract class OrganizationalUnit: DomainBase
  {
    public static OrganizationalUnit Create()
    {
      return Create<OrganizationalUnit>();
    }

    public OrganizationalUnit (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }
  }
}