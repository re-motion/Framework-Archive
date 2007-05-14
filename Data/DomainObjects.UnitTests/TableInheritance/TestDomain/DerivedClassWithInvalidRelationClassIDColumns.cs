using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_DerivedClassWithInvalidRelationClassIDColumns")]
  [Instantiable]
  public abstract class DerivedClassWithInvalidRelationClassIDColumns : BaseClassWithInvalidRelationClassIDColumns
  {
    protected DerivedClassWithInvalidRelationClassIDColumns ()
    {
    }
  }
}
