using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute")]
  [DBTable ("ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute : DomainObject
  {
    protected ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute ()
    {
    }

    protected ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute (DataContainer dataContainer)
      : base (dataContainer)
    {
    }
  }
}