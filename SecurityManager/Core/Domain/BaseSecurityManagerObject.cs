using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.SecurityManager.Domain
{
  public abstract class BaseSecurityManagerObject : BindableDomainObject
  {
    public static new BaseSecurityManagerObject GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return DomainObject.GetObject<BaseSecurityManagerObject> (id);
      }
    }

    protected BaseSecurityManagerObject ()
    {
    }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}
