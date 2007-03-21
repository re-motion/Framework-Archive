using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithPropertiesForEachStorageClass: TestDomainBase
  {
    protected ClassWithPropertiesForEachStorageClass (ObjectID objectID)
    {
    }

    [AutomaticProperty]
    [StorageClass (StorageClass.Persistent)]
    public abstract int Persistent { get; set; }

    [AutomaticProperty]
    [StorageClass (StorageClass.Transaction)]
    public abstract object Transaction { get; set; }

    [StorageClassNone]
    public object None 
    { get { return null; }
      set { }
    }
  }
}