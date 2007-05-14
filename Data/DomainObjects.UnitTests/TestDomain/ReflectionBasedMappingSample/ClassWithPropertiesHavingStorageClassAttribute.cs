using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithPropertiesHavingStorageClassAttribute : DomainObject
  {
    protected ClassWithPropertiesHavingStorageClassAttribute ()
    {
    }

    public abstract int NoAttribute { get; set; }

    [StorageClass (StorageClass.Persistent)]
    public abstract int Persistent { get; set; }

    //[StorageClassTransaction]
    //public abstract object Transaction { get; set; }

    [StorageClassNone]
    public object None 
    { get { return null; }
      set { }
    }
  }
}