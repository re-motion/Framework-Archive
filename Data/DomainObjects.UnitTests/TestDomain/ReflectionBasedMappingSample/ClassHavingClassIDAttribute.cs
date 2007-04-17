using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttribute")]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassHavingClassIDAttribute : DomainObject
  {
    protected ClassHavingClassIDAttribute (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}