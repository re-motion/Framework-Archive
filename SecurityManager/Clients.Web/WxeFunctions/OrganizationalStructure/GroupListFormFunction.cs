using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [WxeDemandTargetStaticMethodPermission (Group.Methods.Search)]
  public class GroupListFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public GroupListFormFunction ()
    {
    }

    // TODO: Make protected once a way is found to solve the "WxeDemandTargetStaticMethodPermission being typed on fixed class" problem
    public GroupListFormFunction (params object[] args)
      : base (args)
    {
    }

    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (GroupListForm), "UI/OrganizationalStructure/GroupListForm.aspx");
  }
}
