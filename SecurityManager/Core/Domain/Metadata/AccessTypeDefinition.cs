using System;
using System.ComponentModel;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  public abstract class AccessTypeDefinition : EnumValueDefinition
  {
    public static AccessTypeDefinition NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return NewObject<AccessTypeDefinition> ().With ();
      }
    }

    public static AccessTypeDefinition NewObject (ClientTransaction clientTransaction, Guid metadataItemID, string name, int value)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return NewObject<AccessTypeDefinition> ().With (metadataItemID, name, value);
      }
    }

    protected AccessTypeDefinition ()
    {
    }

    protected AccessTypeDefinition (Guid metadataItemID, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      MetadataItemID = metadataItemID;
      Name = name;
      Value = value;
    }

    [DBBidirectionalRelation ("AccessType")]
    public abstract ObjectList<AccessTypeReference> References { get; }

    [DBBidirectionalRelation ("AccessType")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected abstract ObjectList<Permission> Permissions { get; }
  }
}
