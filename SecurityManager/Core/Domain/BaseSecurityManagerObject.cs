using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.SecurityManager.Domain
{
  [Serializable]
  public abstract class BaseSecurityManagerObject : BindableDomainObject
  {
    public static BaseSecurityManagerObject GetObject (ObjectID id)
    {
      return DomainObject.GetObject<BaseSecurityManagerObject> (id);
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
