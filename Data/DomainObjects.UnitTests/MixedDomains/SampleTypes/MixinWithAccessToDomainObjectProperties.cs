using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  public class MixinWithAccessToDomainObjectProperties<TDomainObject> : DomainObjectMixin<TDomainObject>
      where TDomainObject : DomainObject
  {
    public bool OnDomainObjectCreatedCalled = false;
    public bool OnDomainObjectLoadedCalled = false;
    public LoadMode OnDomainObjectLoadedLoadMode;

    public new ObjectID ID
    {
      get { return base.ID; }
    }

    public new Type GetPublicDomainObjectType ()
    {
      return base.GetPublicDomainObjectType ();
    }

    public new StateType State
    {
      get { return base.State; }
    }

    public new bool IsDiscarded
    {
      get { return base.IsDiscarded; }
    }

    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

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