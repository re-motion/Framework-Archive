using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute")]
  [DBTable (Name = "ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable")]
  public abstract class ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute : ReflectionBasedMappingTestDomainBase
  {
    protected ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}