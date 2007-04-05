using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [NotAbstract]
  public abstract class ClassWithDefaultStorageProvider : DomainObject
  {
    protected ClassWithDefaultStorageProvider (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
 }
}