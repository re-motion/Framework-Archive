using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_DerivedClassWithInvalidRelationClassIDColumns")]
  [NotAbstract]
  public abstract class DerivedClassWithInvalidRelationClassIDColumns : BaseClassWithInvalidRelationClassIDColumns
  {
    protected DerivedClassWithInvalidRelationClassIDColumns (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    protected DerivedClassWithInvalidRelationClassIDColumns (DataContainer dataContainer)
      : base (dataContainer)
    {
    }
  }
}
