using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  public class PositionListFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public PositionListFormFunction ()
    {
    }

    // TODO: Make protected once delegation works
    public PositionListFormFunction (params object[] args)
      : base (args)
    {
    }

    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (PositionListForm), "UI/OrganizationalStructure/PositionListForm.aspx");
  }
}
