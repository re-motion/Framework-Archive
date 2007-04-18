using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute")]
  [DBTable (Name = "ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable")]
  [TestDomain]
  [NotAbstract]
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