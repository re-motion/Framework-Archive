using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [StorageProviderStub]
  [NotAbstract]
  public abstract class ClassWithStorageProviderFromStorageGroup : DomainObject
  {
    protected ClassWithStorageProviderFromStorageGroup ()
    {
    }

    protected ClassWithStorageProviderFromStorageGroup (DataContainer dataContainer)
      : base (dataContainer)
    {
    }
  }
}