using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  public class Official : StorageProviderStubDomainBase
  {
    // types

    // static members and constants

    public static new Official GetObject (ObjectID id)
    {
      return (Official) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Official ()
    {
    }

    public Official (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public Official (ClientTransaction clientTransaction, ObjectID objectID)
      : base(clientTransaction, objectID)
    {
    }

    protected Official (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public string Name
    {
      get { return DataContainer.GetString ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    [DBBidirectionalRelation ("Official")]
    public ObjectList<Order> Orders
    {
      get { return (ObjectList<Order>) GetRelatedObjects ("Orders"); }
    }
  }
}
