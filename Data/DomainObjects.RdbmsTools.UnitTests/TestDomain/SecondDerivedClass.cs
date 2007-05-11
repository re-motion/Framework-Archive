using System;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  public abstract class SecondDerivedClass : ConcreteClass
  {
    public static SecondDerivedClass NewObject()
    {
      return NewObject<SecondDerivedClass>().With();
    }

    protected SecondDerivedClass()
    {
    }

    protected SecondDerivedClass (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string PropertyInSecondDerivedClass { get; set; }

    [DBColumn ("ClassWithRelationsInSecondDerivedClassID")]
    public abstract ClassWithRelations ClassWithRelationsToSecondDerivedClass { get; set; }
  }
}