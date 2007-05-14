using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public class AccessControlObject : BaseSecurityManagerObject
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AccessControlObject (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected AccessControlObject (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties
  }
}
