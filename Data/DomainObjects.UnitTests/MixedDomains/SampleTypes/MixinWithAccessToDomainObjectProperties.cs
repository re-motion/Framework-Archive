using System;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [CLSCompliant (false)]
  [Extends (typeof (ClassWithAllDataTypes), MixinTypeArguments = new Type[] { typeof (ClassWithAllDataTypes) })]
  public class MixinWithAccessToDomainObjectProperties<TDomainObject> : DomainObjectMixin<TDomainObject>
      where TDomainObject : DomainObject
  {
    public bool OnDomainObjectCreatedCalled = false;
    public bool OnDomainObjectLoadedCalled = false;
    public LoadMode OnDomainObjectLoadedLoadMode;

    [StorageClassNone]
    public new ObjectID ID
    {
      get { return base.ID; }
    }

    public new Type GetPublicDomainObjectType ()
    {
      return base.GetPublicDomainObjectType ();
    }

    [StorageClassNone]
    public new StateType State
    {
      get { return base.State; }
    }

    [StorageClassNone]
    public new bool IsDiscarded
    {
      get { return base.IsDiscarded; }
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    [StorageClassNone]
    public new TDomainObject This
    {
      get { return base.This; }
    }

    protected override void OnDomainObjectCreated ()
    {
      OnDomainObjectCreatedCalled = true;
    }

    protected override void OnDomainObjectLoaded (LoadMode loadMode)
    {
      OnDomainObjectLoadedCalled = true;
      OnDomainObjectLoadedLoadMode = loadMode;
    }
  }
}