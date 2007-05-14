using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  public class GroupTypeListFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public GroupTypeListFormFunction ()
    {
    }

    // TODO: Make protected once delegation works
    public GroupTypeListFormFunction (params object[] args)
      : base (args)
    {
    }

    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (GroupTypeListForm), "UI/OrganizationalStructure/GroupTypeListForm.aspx");
  }
}
