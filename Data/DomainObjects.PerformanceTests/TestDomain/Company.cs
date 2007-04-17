using System;

using Rubicon.Data.DomainObjects;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
  public class Company : ClientBoundBaseClass
  {
    // types

    // static members and constants

    public static new Company GetObject (ObjectID id)
    {
      return (Company) DomainObject.GetObject (id);
    }

    public static new Company GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Company) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public Company ()
    {
    }

    public Company (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Company (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer.GetValue ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }
  }
}
