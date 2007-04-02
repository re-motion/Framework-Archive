using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable (Name = "TableName")]
  public abstract class ClassHavingStorageSpecificIdentifierAttribute: TestDomainBase
  {
    protected ClassHavingStorageSpecificIdentifierAttribute (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract int NoAttribute { get; set; }

    [AutomaticProperty]
    [DBColumn ("CustomName")]
    public abstract int StorageSpecificName { get; set; }
  }
}