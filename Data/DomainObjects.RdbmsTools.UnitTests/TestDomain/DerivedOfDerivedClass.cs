using System;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  public abstract class DerivedOfDerivedClass : DerivedClass
  {
    public static DerivedOfDerivedClass NewObject()
    {
      return NewObject<DerivedOfDerivedClass>().With();
    }

    protected DerivedOfDerivedClass()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string PropertyInDerivedOfDerivedClass { get; set; }

    [DBColumn ("ClassWithRelationsInDerivedOfDerivedClassID")]
    public abstract ClassWithRelations ClassWithRelationsToDerivedOfDerivedClass { get; set; }
  }
}