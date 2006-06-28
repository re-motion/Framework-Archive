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

namespace Rubicon.SecurityManager.Client.Web.WxeFunctions.OrganizationalStructure
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
