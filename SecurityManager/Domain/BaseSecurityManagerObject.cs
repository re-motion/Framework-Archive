using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.SecurityManager.Domain
{
  public class BaseSecurityManagerObject : BindableDomainObject
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public BaseSecurityManagerObject ()
    {
    }

    public BaseSecurityManagerObject (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected BaseSecurityManagerObject (DataContainer dataContainer) : base (dataContainer)
    {
    // This infrastructure constructor is necessary for the DomainObjects framework.
    // Do not remove the constructor or place any code here.
    }

    // methods and properties
  }
}
