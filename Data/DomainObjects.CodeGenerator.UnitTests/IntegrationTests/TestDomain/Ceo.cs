using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests.TestDomain
{
public class Ceo : BindableDomainObject
{
  // types

  // static members and constants

  public static new Ceo GetObject (ObjectID id)
  {
    return (Ceo) DomainObject.GetObject (id);
  }

  public static new Ceo GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (Ceo) DomainObject.GetObject (id, clientTransaction);
  }

  // member fields

  // construction and disposing

  public Ceo ()
  {
  }

  public Ceo (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected Ceo (DataContainer dataContainer) : base (dataContainer)
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

  public Company Company
  {
    get { return (Company) GetRelatedObject ("Company"); }
    set { SetRelatedObject ("Company", value); }
  }

}
}
