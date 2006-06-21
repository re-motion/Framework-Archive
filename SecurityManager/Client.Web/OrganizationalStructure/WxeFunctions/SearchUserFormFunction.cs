using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.Classes;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.WxeFunctions
{
  public class SearchUserFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SearchUserFormFunction ()
    {
    }

    public SearchUserFormFunction (params object[] args)
      : base (args)
    {
    }

    public SearchUserFormFunction (ObjectID clientID)
      : base (clientID)
    {
    }
    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (SearchUserForm), "OrganizationalStructure/UI/SearchUserForm.aspx");
  }
}
