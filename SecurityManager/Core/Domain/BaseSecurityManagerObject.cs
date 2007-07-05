using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.SecurityManager.Domain
{
  public abstract class BaseSecurityManagerObject : BindableDomainObject
  {
    private ClientTransaction _clientTransaction;

    public static new BaseSecurityManagerObject GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new ClientTransactionScope (clientTransaction))
      {
        return DomainObject.GetObject<BaseSecurityManagerObject> (id);
      }
    }

    protected BaseSecurityManagerObject ()
    {
      _clientTransaction = ClientTransactionScope.CurrentTransaction;
    }

    protected override void OnLoaded ()
    {
      base.OnLoaded ();
      _clientTransaction = ClientTransactionScope.CurrentTransaction;
    }

    public new void Delete ()
    {
      base.Delete();
    }

    [StorageClassNone]
    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }
  }
}
