using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [Instantiable]
  public abstract class DerivedClassWithMixedProperties : ClassWithMixedProperties
  {
    protected DerivedClassWithMixedProperties()
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
      get {
        return Properties["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties.PrivateString"]
            .GetValue<string> ();
      }
      set
      {
        Properties["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties.PrivateString"]
            .SetValue (value);
      }
    }
  }
}