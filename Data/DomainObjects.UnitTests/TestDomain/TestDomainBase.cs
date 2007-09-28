using System;
using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Development.UnitTesting;

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
    public DataContainer InternalDataContainer
    {
      get { return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (this, "GetDataContainer"); }
    }

    public DataContainer GetInternalDataContainerForTransaction(ClientTransaction transaction)
    {
      return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (this, "GetDataContainerForTransaction", transaction);
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

    [StorageClassNone]
    public new PropertyIndexer Properties
		{
			get { return base.Properties; }
		}

    public new IEnumerable<DomainObject> GetAllRelatedObjects ()
    {
      return base.GetAllRelatedObjects ();
    }

    public new Set<DomainObject> GetFlattenedRelatedObjectGraph ()
    {
      return base.GetFlattenedRelatedObjectGraph();
    }
  }
}
#pragma warning restore 0618