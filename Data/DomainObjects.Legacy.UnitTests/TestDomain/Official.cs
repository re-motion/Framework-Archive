using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Official : TestDomainBase
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

    protected Official (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return DataContainer.GetString ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public DomainObjectCollection Orders
    {
      get { return GetRelatedObjects ("Orders"); }
    }
  }
}
