using System;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  public abstract class DerivedClass : ConcreteClass
  {
    public new static DerivedClass NewObject()
    {
      return NewObject<DerivedClass>().With();
    }

    protected DerivedClass()
    {
    }

    protected DerivedClass (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string PropertyInDerivedClass { get; set; }
  }
}