using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects;

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