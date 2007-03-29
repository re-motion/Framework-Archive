using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithPropertiesHavingStorageClassAttribute: TestDomainBase
  {
    protected ClassWithPropertiesHavingStorageClassAttribute (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    public abstract int NoAttribute { get; set; }

    [AutomaticProperty]
    [StorageClass (StorageClass.Persistent)]
    public abstract int Persistent { get; set; }

    //[AutomaticProperty]
    //[StorageClassTransaction]
    //public abstract object Transaction { get; set; }

    [StorageClassNone]
    public object None 
    { get { return null; }
      set { }
    }
  }
}