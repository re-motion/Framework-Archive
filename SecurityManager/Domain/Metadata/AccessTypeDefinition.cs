using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class AccessTypeDefinition : EnumValueDefinition
  {
    // types

    // static members and constants

    public static new AccessTypeDefinition GetObject (ObjectID id)
    {
      return (AccessTypeDefinition) DomainObject.GetObject (id);
    }

    public static new AccessTypeDefinition GetObject (ObjectID id, bool includeDeleted)
    {
      return (AccessTypeDefinition) DomainObject.GetObject (id, includeDeleted);
    }

    public static new AccessTypeDefinition GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (AccessTypeDefinition) DomainObject.GetObject (id, clientTransaction);
    }

    public static new AccessTypeDefinition GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (AccessTypeDefinition) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public AccessTypeDefinition (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected AccessTypeDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public Rubicon.Data.DomainObjects.DomainObjectCollection References
    {
      get { return (Rubicon.Data.DomainObjects.DomainObjectCollection) GetRelatedObjects ("References"); }
      set { } // marks property References as modifiable
    }
  }
}
