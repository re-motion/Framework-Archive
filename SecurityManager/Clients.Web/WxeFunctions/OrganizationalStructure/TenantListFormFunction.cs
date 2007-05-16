using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Security.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [WxeDemandTargetStaticMethodPermission (Tenant.Methods.Search)]
  public class TenantListFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public TenantListFormFunction ()
    {
    }

    // TODO: Make protected once a way is found to solve the "WxeDemandTargetStaticMethodPermission being typed on fixed class" problem
    public TenantListFormFunction (params object[] args)
      : base (args)
    {
    }

    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (TenantListForm), "UI/OrganizationalStructure/TenantListForm.aspx");
  }
}
