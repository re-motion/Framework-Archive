using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class AccessTypeReference : BaseSecurityManagerObject
  {
    public static AccessTypeReference NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<AccessTypeReference> ().With ();
      }
    }

    protected AccessTypeReference ()
    {
    }

    public abstract int Index { get; set; }

    [DBBidirectionalRelation ("AccessTypeReferences")]
    [DBColumn ("SecurableClassID")]
    [Mandatory]
    public abstract SecurableClassDefinition Class { get; set; }

    [DBBidirectionalRelation ("References")]
    [Mandatory]
    public abstract AccessTypeDefinition AccessType { get; set; }
  }
}
