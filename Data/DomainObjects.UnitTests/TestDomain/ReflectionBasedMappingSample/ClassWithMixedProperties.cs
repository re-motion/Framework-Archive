using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithMixedProperties: ClassWithMixedPropertiesNotInMapping
  {
    protected ClassWithMixedProperties (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [StorageClassNone]
    public object Unmanaged
    {
      get { return null; }
      set { }
    }

    [AutomaticProperty]
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

    [AutomaticProperty]
    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToOne { get; set; }

    [AutomaticProperty]
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