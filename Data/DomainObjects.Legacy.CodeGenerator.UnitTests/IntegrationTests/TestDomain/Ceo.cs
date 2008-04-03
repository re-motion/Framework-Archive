using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain
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

    public Ceo (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected Ceo (DataContainer dataContainer)
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

    public Company Company
    {
      get { return (Company) GetRelatedObject ("Company"); }
      set { SetRelatedObject ("Company", value); }
    }

  }
}
