using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain
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

    public Company (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Company (DataContainer dataContainer)
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
