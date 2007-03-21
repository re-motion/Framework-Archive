using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithStringProperties: TestDomainBase
  {
    protected ClassWithStringProperties (ObjectID objectID)
    {
    }

    [AutomaticProperty]
    public abstract string NoAttribute { get; set; }

    [String (IsNullable = true)]
    [AutomaticProperty]
    public abstract string NullableFromAttribute { get; set; }

    [String (IsNullable = false)]
    [AutomaticProperty]
    public abstract string NotNullable { get; set; }

    [String (MaximumLength = 100)]
    [AutomaticProperty]
    public abstract string MaximumLength { get; set; }

    [String (IsNullable = false, MaximumLength = 100)]
    [AutomaticProperty]
    public abstract string NotNullableAndMaximumLength { get; set; }
  }
}