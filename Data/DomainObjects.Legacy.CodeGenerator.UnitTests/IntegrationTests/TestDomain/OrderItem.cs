using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain
{
  public class OrderItem : BindableDomainObject
  {
    // types

    // static members and constants

    public static new OrderItem GetObject (ObjectID id)
    {
      return (OrderItem) DomainObject.GetObject (id);
    }

    public static new OrderItem GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (OrderItem) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public OrderItem ()
    {
    }

    public OrderItem (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected OrderItem (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
      // For any code that should run when a DomainObject is loaded, OnLoaded () should be overridden.
    }

    // methods and properties

    public int Position
    {
      get { return (int) DataContainer["Position"]; }
      set { DataContainer["Position"] = value; }
    }

    public string Product
    {
      get { return (string) DataContainer["Product"]; }
      set { DataContainer["Product"] = value; }
    }

    public Order Order
    {
      get { return (Order) GetRelatedObject ("Order"); }
      set { SetRelatedObject ("Order", value); }
    }

  }
}
