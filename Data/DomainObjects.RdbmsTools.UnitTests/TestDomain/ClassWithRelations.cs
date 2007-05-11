using System;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [FirstStorageGroupAttribute]
  [Instantiable]
  public abstract class ClassWithRelations : DomainObject
  {
    public static ClassWithRelations NewObject()
    {
      return NewObject<ClassWithRelations>().With();
    }

    protected ClassWithRelations()
    {
    }

    protected ClassWithRelations (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract DerivedClass DerivedClass { get; set; }
  }
}