using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class AccessTypeReference : MetadataObject
  {
    // types

    // static members and constants

    public static new AccessTypeReference GetObject (ObjectID id)
    {
      return (AccessTypeReference) DomainObject.GetObject (id);
    }

    public static new AccessTypeReference GetObject (ObjectID id, bool includeDeleted)
    {
      return (AccessTypeReference) DomainObject.GetObject (id, includeDeleted);
    }

    public static new AccessTypeReference GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (AccessTypeReference) DomainObject.GetObject (id, clientTransaction);
    }

    public static new AccessTypeReference GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (AccessTypeReference) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public AccessTypeReference (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected AccessTypeReference (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public SecurableClassDefinition Class
    {
      get { return (SecurableClassDefinition) GetRelatedObject ("Class"); }
      set { SetRelatedObject ("Class", value); }
    }

    public AccessTypeDefinition AccessType
    {
      get { return (AccessTypeDefinition) GetRelatedObject ("AccessType"); }
      set { SetRelatedObject ("AccessType", value); }
    }

  }
}
