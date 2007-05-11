using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  //No StorageGroup
  [Instantiable]
  public abstract class ClassWithoutStorageGroupWithMixedProperties : DomainObject
  {
    protected ClassWithoutStorageGroupWithMixedProperties ()
    {
    }

    protected ClassWithoutStorageGroupWithMixedProperties (DataContainer dataContainer)
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
            "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.String");
      }
      set { SetPropertyValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.String", value); }
    }

    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToOne { get; set; }

    public abstract ObjectList<ClassWithOneSideRelationProperties> UnidirectionalOneToMany { get; }

    private string PrivateString
    {
      get
      {
        return GetPropertyValue<string> (
            "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.PrivateString");
      }
      set
      {
        SetPropertyValue (
            "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString", value);
      }
    }
  }
}
