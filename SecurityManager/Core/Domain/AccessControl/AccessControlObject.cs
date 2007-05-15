using System;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public abstract class AccessControlObject : BaseSecurityManagerObject
  {
    protected AccessControlObject ()
    {
    }
  }
}