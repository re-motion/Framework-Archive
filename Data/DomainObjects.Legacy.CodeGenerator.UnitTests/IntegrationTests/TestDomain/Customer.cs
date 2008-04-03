using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain
{
  public class Customer : Company
  {
    // types

    public enum CustomerType
    {
      DummyEntry = 0
    }

    // static members and constants

    public static new Customer GetObject (ObjectID id)
    {
      return (Customer) DomainObject.GetObject (id);
    }

    public static new Customer GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Customer) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public Customer ()
    {
    }

    public Customer (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Customer (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
      // For any code that should run when a DomainObject is loaded, OnLoaded () should be overridden.
    }

    // methods and properties

    public Customer.CustomerType Type
    {
      get { return (Customer.CustomerType) DataContainer["Type"]; }
      set { DataContainer["Type"] = value; }
    }

    public string PropertyWithIdenticalNameInDifferentInheritanceBranches
    {
      get { return (string) DataContainer["PropertyWithIdenticalNameInDifferentInheritanceBranches"]; }
      set { DataContainer["PropertyWithIdenticalNameInDifferentInheritanceBranches"] = value; }
    }

    public OrderCollection Orders
    {
      get { return (OrderCollection) GetRelatedObjects ("Orders"); }
      set { } // marks property Orders as modifiable
    }

    public Official PrimaryOfficial
    {
      get { return (Official) GetRelatedObject ("PrimaryOfficial"); }
      set { SetRelatedObject ("PrimaryOfficial", value); }
    }

  }
}
