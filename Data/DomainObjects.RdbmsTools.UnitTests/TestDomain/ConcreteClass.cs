using System;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [FirstStorageGroupAttribute]
  [Instantiable]
  public abstract class ConcreteClass : DomainObject
  {
    public static ConcreteClass NewObject()
    {
      return NewObject<ConcreteClass>().With();
    }

    protected ConcreteClass()
    {
    }

    protected ConcreteClass (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string PropertyInConcreteClass { get; set; }
  }
}