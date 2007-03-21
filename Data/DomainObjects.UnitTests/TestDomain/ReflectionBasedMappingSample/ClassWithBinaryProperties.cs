using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithBinaryProperties: TestDomainBase
  {
    protected ClassWithBinaryProperties (ObjectID objectID)
    {
    }

    [AutomaticProperty]
    public abstract byte[] NoAttribute { get; set; }

    [Binary (IsNullable = true)]
    [AutomaticProperty]
    public abstract byte[] NullableFromAttribute { get; set; }

    [Binary (IsNullable = false)]
    [AutomaticProperty]
    public abstract byte[] NotNullable { get; set; }

    [Binary (MaximumLength = 100)]
    [AutomaticProperty]
    public abstract byte[] MaximumLength { get; set; }

    [Binary (IsNullable = false, MaximumLength = 100)]
    [AutomaticProperty]
    public abstract byte[] NotNullableAndMaximumLength { get; set; }
  }
}