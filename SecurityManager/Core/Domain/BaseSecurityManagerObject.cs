using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.SecurityManager.Domain
{
  public abstract class BaseSecurityManagerObject : BindableDomainObject
  {
    protected BaseSecurityManagerObject ()
    {
    }
  }
}
