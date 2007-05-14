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

    protected ClientBoundBaseClass ()
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClientBoundBaseClasses")]
    [Mandatory]
    public abstract Client Client { get; set;}
  }
}
