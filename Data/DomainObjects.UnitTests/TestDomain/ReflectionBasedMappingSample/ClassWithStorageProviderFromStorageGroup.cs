using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [StorageProviderStub]
  [NotAbstract]
  public abstract class ClassWithStorageProviderFromStorageGroup : DomainObject
  {
    protected ClassWithStorageProviderFromStorageGroup (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}