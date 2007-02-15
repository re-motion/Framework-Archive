using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  public class GroupListFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public GroupListFormFunction ()
    {
    }

    // TODO: Make protected once delegation works
    public GroupListFormFunction (params object[] args)
      : base (args)
    {
    }

    public GroupListFormFunction (ObjectID clientID)
      : base (clientID)
    {
    }

    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (GroupListForm), "UI/OrganizationalStructure/GroupListForm.aspx");
  }
}
