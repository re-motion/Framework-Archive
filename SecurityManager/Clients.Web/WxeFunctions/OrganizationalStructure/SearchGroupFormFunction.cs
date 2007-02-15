using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  public class SearchGroupFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SearchGroupFormFunction ()
    {
    }

    protected SearchGroupFormFunction (params object[] args)
      : base (args)
    {
    }

    public SearchGroupFormFunction (ObjectID clientID)
      : base (clientID)
    {
    }

    // methods and properties

    public Group SelectedGroup
    {
      get { return (Group) Variables["Group"]; }
      set { Variables["Group"] = value; }
    }

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (SearchGroupForm), "UI/OrganizationalStructure/SearchGroupForm.aspx");
  }
}
