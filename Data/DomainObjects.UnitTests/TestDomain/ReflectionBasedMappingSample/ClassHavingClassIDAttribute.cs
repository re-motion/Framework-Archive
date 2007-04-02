using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttribute")]
  [DBTable]
  public abstract class ClassHavingClassIDAttribute: TestDomainBase
  {
    protected ClassHavingClassIDAttribute (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}