using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithStringProperties : DomainObject
  {
    protected ClassWithStringProperties ()
    {
    }

    protected ClassWithStringProperties (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public abstract string NoAttribute { get; set; }

    [StringProperty (IsNullable = true)]
    public abstract string NullableFromAttribute { get; set; }

    [StringProperty (IsNullable = false)]
    public abstract string NotNullable { get; set; }

    [StringProperty (MaximumLength = 100)]
    public abstract string MaximumLength { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string NotNullableAndMaximumLength { get; set; }
  }
}