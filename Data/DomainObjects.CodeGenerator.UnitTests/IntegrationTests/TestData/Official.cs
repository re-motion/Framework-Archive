using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests.TestDomain
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

  public Official (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected Official (DataContainer dataContainer) : base (dataContainer)
  {
    // This infrastructure constructor is necessary for the DomainObjects framework.
    // Do not remove the constructor or place any code here.
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
