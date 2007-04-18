using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [NotAbstract]
  public abstract class ClassWithDefaultStorageProvider : DomainObject
  {
    protected ClassWithDefaultStorageProvider ()
    {
    }

    protected ClassWithDefaultStorageProvider (DataContainer dataContainer)
      : base (dataContainer)
    {
    }
 }
}