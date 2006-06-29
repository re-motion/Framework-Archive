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
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

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

    protected PositionListFormFunction (params object[] args)
      : base (args)
    {
    }

    public PositionListFormFunction (ObjectID clientID)
      : base (clientID)
    {
    }
    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (PositionListForm), "UI/OrganizationalStructure/PositionListForm.aspx");
  }
}
