using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class AccessTypeDefinition : EnumValueDefinition
  {
    // types

    // static members and constants

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

    public AccessTypeDefinition (ClientTransaction clientTransaction, Guid metadataItemID, string name, int value)
      : base (clientTransaction)
    {
      DataContainer["MetadataItemID"] = metadataItemID;
      DataContainer["Name"] = name;
      DataContainer["Value"] = value;
    }

    protected AccessTypeDefinition (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public DomainObjectCollection References
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("References"); }
      set { } // marks property References as modifiable
    }

    private DomainObjectCollection Permissions
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Permissions"); }
    }
  }
}
