using System;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects
{
  public class DomainObjectMixin<TDomainObject> : DomainObjectMixin<TDomainObject, IDomainObjectBaseCallRequirements>
    where TDomainObject : DomainObject
  {
  }

  public class DomainObjectMixin<TDomainObject, TBaseCallRequirements>
      : Mixin<TDomainObject, TBaseCallRequirements>, IDomainObjectMixin
      where TDomainObject : DomainObject
      where TBaseCallRequirements : class, IDomainObjectBaseCallRequirements
  {
    protected ObjectID ID
    {
      get { return This.ID; }
    }

    protected Type GetPublicDomainObjectType ()
    {
      return This.GetPublicDomainObjectType ();
    }

    protected StateType State
    {
      get { return This.State; }
    }

    protected bool IsDiscarded
    {
      get { return This.IsDiscarded; }
    }

    protected PropertyIndexer Properties
    {
      get { return This.Properties; }
    }

    void IDomainObjectMixin.OnDomainObjectCreated ()
    {
      OnDomainObjectCreated ();
    }

    protected virtual void OnDomainObjectCreated ()
    {
    }

    void IDomainObjectMixin.OnDomainObjectLoaded (LoadMode loadMode)
    {
      OnDomainObjectLoaded (loadMode);
    }

    protected virtual void OnDomainObjectLoaded (LoadMode loadMode)
    {
    }
  }

  public interface IDomainObjectBaseCallRequirements
  {
    PropertyIndexer Properties { get; }
  }
}