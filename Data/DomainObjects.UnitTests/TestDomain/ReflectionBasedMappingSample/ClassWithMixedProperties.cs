using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithMixedProperties: ClassWithMixedPropertiesNotInMapping
  {
    protected ClassWithMixedProperties ()
    {
    }

    protected ClassWithMixedProperties (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [StorageClassNone]
    public object Unmanaged
    {
      get { return null; }
      set { }
    }

    public abstract int Int32 { get; set; }

    public virtual string String
    {
      get
      {
        return GetPropertyValue<string> (
            "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.String");
      }
      set { SetPropertyValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.String", value); }
    }

    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToOne { get; set; }

    public abstract ObjectList<ClassWithOneSideRelationProperties> UnidirectionalOneToMany { get; }

    private string PrivateString
    {
      get
      {
        return GetPropertyValue<string> (
            "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString");
      }
      set
      {
        SetPropertyValue (
            "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString", value);
      }
    }
  }
}
