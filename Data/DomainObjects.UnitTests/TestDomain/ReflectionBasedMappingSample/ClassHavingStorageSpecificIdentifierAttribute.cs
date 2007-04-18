using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable (Name = "ClassHavingStorageSpecificIdentifierAttributeTable")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassHavingStorageSpecificIdentifierAttribute : DomainObject
  {

    protected ClassHavingStorageSpecificIdentifierAttribute ()
    {
    }

    protected ClassHavingStorageSpecificIdentifierAttribute (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public abstract int NoAttribute { get; set; }

    [DBColumn ("CustomName")]
    public abstract int StorageSpecificName { get; set; }
  }
}