using System;

using Rubicon.Data.DomainObjects;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
  public abstract class ClientBoundBaseClass : DomainObject
  {
    // types

    // static members and constants

    public static new ClientBoundBaseClass GetObject (ObjectID id)
    {
      return (ClientBoundBaseClass) DomainObject.GetObject (id);
    }

    public static new ClientBoundBaseClass GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (ClientBoundBaseClass) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public ClientBoundBaseClass ()
    {
    }

    public ClientBoundBaseClass (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected ClientBoundBaseClass (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
      set { SetRelatedObject ("Client", value); }
    }

  }
}
