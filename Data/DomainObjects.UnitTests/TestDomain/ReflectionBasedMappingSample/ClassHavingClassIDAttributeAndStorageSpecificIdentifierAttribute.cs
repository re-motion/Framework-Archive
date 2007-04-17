using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [ClassID ("ClassIDForClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute")]
  [DBTable (Name = "ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttributeTable")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute : DomainObject
  {
    protected ClassHavingClassIDAttributeAndStorageSpecificIdentifierAttribute (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }
  }
}