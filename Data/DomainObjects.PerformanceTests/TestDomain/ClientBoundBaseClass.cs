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
      return DomainObject.GetObject<ClientBoundBaseClass> (id);
    }

    public static new ClientBoundBaseClass GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.GetObject<ClientBoundBaseClass> (id);
      }
    }

    // member fields

    // construction and disposing

    protected ClientBoundBaseClass ()
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClientBoundBaseClasses")]
    [Mandatory]
    public abstract Client Client { get; set;}
  }
}
