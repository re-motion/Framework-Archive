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
using Rubicon.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.WxeFunctions
{
  public class SearchGroupTypeFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SearchGroupTypeFormFunction ()
    {
    }

    protected SearchGroupTypeFormFunction (params object[] args)
      : base (args)
    {
    }

    public SearchGroupTypeFormFunction (ObjectID clientID)
      : base (clientID)
    {
    }
    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (SearchGroupTypeForm), "OrganizationalStructure/UI/SearchGroupTypeForm.aspx");
  }
}
