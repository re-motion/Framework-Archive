using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.SecurityManager.Clients.Web.Test.Domain
{
  public abstract class BaseObject : BindableDomainObject
  {
    // types

    // static members

    // member fields

    // construction and disposing
 
    protected BaseObject (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    // methods and properties

    protected BaseObject (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }
 }
}