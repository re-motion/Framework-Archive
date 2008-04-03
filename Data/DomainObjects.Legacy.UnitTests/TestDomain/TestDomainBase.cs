using System;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class TestDomainBase : DomainObject
  {
    // types

    // static members and constants

    public static TestDomainBase GetObject (ObjectID id, bool includeDeleted)
    {
      return (TestDomainBase) RepositoryAccessor.GetObject (id, includeDeleted);
    }

    // member fields

    // construction and disposing

    protected TestDomainBase ()
    {
    }

    protected TestDomainBase (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public new DataContainerIndirection DataContainer
    {
      get { return base.DataContainer; }
    }

		public DataContainer InternalDataContainer
		{
			get { return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (this, typeof (DomainObject), "GetDataContainer"); }
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
      base.Delete ();
    }
  }
}
