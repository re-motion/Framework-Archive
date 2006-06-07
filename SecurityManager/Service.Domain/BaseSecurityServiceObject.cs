using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Security.Service.Domain
{
  public class BaseSecurityServiceObject : BindableDomainObject
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public BaseSecurityServiceObject ()
    {
    }

    public BaseSecurityServiceObject (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected BaseSecurityServiceObject (DataContainer dataContainer) : base (dataContainer)
    {
    // This infrastructure constructor is necessary for the DomainObjects framework.
    // Do not remove the constructor or place any code here.
    }

    // methods and properties
  }
}
