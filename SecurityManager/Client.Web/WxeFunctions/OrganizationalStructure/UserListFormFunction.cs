using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.SecurityManager.Client.Web.Classes.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.SecurityManager.Client.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Client.Web.WxeFunctions.OrganizationalStructure
{
  public class UserListFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public UserListFormFunction ()
    {
    }

    protected UserListFormFunction (params object[] args)
      : base (args)
    {
    }

    public UserListFormFunction (ObjectID clientID)
      : base (clientID)
    {
    }
    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (UserListForm), "UI/OrganizationalStructure/UserListForm.aspx");
  }
}
