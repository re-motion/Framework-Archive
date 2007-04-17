using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [NotAbstract]
  public abstract class DerivedClassWithMixedProperties : ClassWithMixedProperties
  {
    protected DerivedClassWithMixedProperties (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    public override int Int32
    {
      get { return 0; }
      set { }
    }

    public abstract string OtherString { get; set; }

    [DBColumn ("NewString")]
    public new abstract string String { get; set; }

    [DBColumn ("DerivedPrivateString")]
    private string PrivateString
    {
      get { return GetPropertyValue<string> ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties.PrivateString"); }
      set { SetPropertyValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties.PrivateString", value); }
    }
  }
}