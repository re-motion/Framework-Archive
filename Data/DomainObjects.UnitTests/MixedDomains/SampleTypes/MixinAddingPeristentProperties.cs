using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  public class MixinAddingPeristentProperties : DomainObjectMixin<DomainObject>, IMixinAddingPeristentProperties
  {
    private int _nonPersistentProperty = 0;

    public int PersistentProperty
    {
      get { return Properties[typeof (MixinAddingPeristentProperties), "PersistentProperty"].GetValue<int>(); }
      set { Properties[typeof (MixinAddingPeristentProperties), "PersistentProperty"].SetValue (value); }
    }

    [StorageClass (StorageClass.Persistent)]
    public int ExtraPersistentProperty
    {
      get { return Properties[typeof (MixinAddingPeristentProperties), "ExtraPersistentProperty"].GetValue<int> (); }
      set { Properties[typeof (MixinAddingPeristentProperties), "ExtraPersistentProperty"].SetValue (value); }
    }

    [StorageClassNone]
    public int NonPersistentProperty
    {
      get { return _nonPersistentProperty; }
      set { _nonPersistentProperty = value; }
    }
  }
}