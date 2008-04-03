using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain
{
  public class Official : BindableDomainObject
  {
    // types

    // static members and constants

    public static new Official GetObject (ObjectID id)
    {
      return (Official) DomainObject.GetObject (id);
    }

    public static new Official GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Official) DomainObject.GetObject (id, clientTransaction);
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
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
      // For any code that should run when a DomainObject is loaded, OnLoaded () should be overridden.
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public OrderPriority ResponsibleForOrderPriority
    {
      get { return (OrderPriority) DataContainer["ResponsibleForOrderPriority"]; }
      set { DataContainer["ResponsibleForOrderPriority"] = value; }
    }

    public Customer.CustomerType ResponsibleForCustomerType
    {
      get { return (Customer.CustomerType) DataContainer["ResponsibleForCustomerType"]; }
      set { DataContainer["ResponsibleForCustomerType"] = value; }
    }

    public OrderCollection Orders
    {
      get { return (OrderCollection) GetRelatedObjects ("Orders"); }
      set { } // marks property Orders as modifiable
    }

  }
}
