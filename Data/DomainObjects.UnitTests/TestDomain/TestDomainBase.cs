using System;

#pragma warning disable 0618

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  public abstract class TestDomainBase : DomainObject
  {
    public new static TestDomainBase GetObject (ObjectID id)
    {
      return DomainObject.GetObject<TestDomainBase> (id);
    }

    public new static TestDomainBase GetObject (ObjectID id, bool includeDeleted)
    {
      return DomainObject.GetObject<TestDomainBase> (id, includeDeleted);
    }

    protected TestDomainBase()
    {
    }

    [StorageClassNone]
    public new DataContainer DataContainer
    {
      get { return base.DataContainer; }
    }

    public new DomainObject GetRelatedObject (string propertyName)
    {
      return base.GetRelatedObject (propertyName);
    }

    public new DomainObjectCollection GetRelatedObjects (string propertyName)
    {
      return base.GetRelatedObjects (propertyName);
    }

    public new DomainObject GetOriginalRelatedObject (string propertyName)
    {
      return base.GetOriginalRelatedObject (propertyName);
    }

    public new DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
    {
      return base.GetOriginalRelatedObjects (propertyName);
    }

    public new void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
    {
      base.SetRelatedObject (propertyName, newRelatedObject);
    }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}
#pragma warning restore 0618