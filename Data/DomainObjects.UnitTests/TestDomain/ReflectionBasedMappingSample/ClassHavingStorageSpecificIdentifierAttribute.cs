using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable (Name = "ClassHavingStorageSpecificIdentifierAttributeTable")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassHavingStorageSpecificIdentifierAttribute : DomainObject
  {
    protected ClassHavingStorageSpecificIdentifierAttribute (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    public abstract int NoAttribute { get; set; }

    [DBColumn ("CustomName")]
    public abstract int StorageSpecificName { get; set; }
  }
}