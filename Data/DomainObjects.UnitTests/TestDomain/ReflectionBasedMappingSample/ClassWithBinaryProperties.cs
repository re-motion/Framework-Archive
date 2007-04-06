using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithBinaryProperties: TestDomainBase
  {
    protected ClassWithBinaryProperties (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract byte[] NoAttribute { get; set; }

    [BinaryProperty (IsNullable = true)]
    [AutomaticProperty]
    public abstract byte[] NullableFromAttribute { get; set; }

    [BinaryProperty (IsNullable = false)]
    [AutomaticProperty]
    public abstract byte[] NotNullable { get; set; }

    [BinaryProperty (MaximumLength = 100)]
    [AutomaticProperty]
    public abstract byte[] MaximumLength { get; set; }

    [BinaryProperty (IsNullable = false, MaximumLength = 100)]
    [AutomaticProperty]
    public abstract byte[] NotNullableAndMaximumLength { get; set; }
  }
}