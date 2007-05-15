using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class StatePropertyReference : BaseSecurityManagerObject
  {
    public static StatePropertyReference NewObject (ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.NewObject<StatePropertyReference> ().With ();
      }
    }

    protected StatePropertyReference ()
    {
    }

    [DBBidirectionalRelation ("StatePropertyReferences")]
    [DBColumn ("SecurableClassID")]
    [Mandatory]
    public abstract SecurableClassDefinition Class { get; set; }

    [DBBidirectionalRelation ("References")]
    [Mandatory]
    public abstract StatePropertyDefinition StateProperty { get; set; }

  }
}
