using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests.TestDomain
{
[Serializable]
public abstract class Company : BindableDomainObject
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

  public Company (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected Company (DataContainer dataContainer) : base (dataContainer)
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

  public string PhoneNumber
  {
    get { return (string) DataContainer["PhoneNumber"]; }
    set { DataContainer["PhoneNumber"] = value; }
  }

  public Ceo Ceo
  {
    get { return (Ceo) GetRelatedObject ("Ceo"); }
    set { SetRelatedObject ("Ceo", value); }
  }

  public Address Address
  {
    get { return (Address) GetRelatedObject ("Address"); }
    set { SetRelatedObject ("Address", value); }
  }

}
}
