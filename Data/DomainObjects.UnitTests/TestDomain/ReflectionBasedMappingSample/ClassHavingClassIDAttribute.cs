using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttribute")]
  [DBTable]
  public abstract class ClassHavingClassIDAttribute : ReflectionBasedMappingTestDomainBase
  {
    protected ClassHavingClassIDAttribute (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}