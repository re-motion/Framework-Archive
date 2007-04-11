using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithStringProperties : ReflectionBasedMappingTestDomainBase
  {
    protected ClassWithStringProperties (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract string NoAttribute { get; set; }

    [StringProperty (IsNullable = true)]
    [AutomaticProperty]
    public abstract string NullableFromAttribute { get; set; }

    [StringProperty (IsNullable = false)]
    [AutomaticProperty]
    public abstract string NotNullable { get; set; }

    [StringProperty (MaximumLength = 100)]
    [AutomaticProperty]
    public abstract string MaximumLength { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [AutomaticProperty]
    public abstract string NotNullableAndMaximumLength { get; set; }
  }
}