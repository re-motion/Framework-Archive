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
using Rubicon.SecurityManager.Configuration;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.WxeFunctions
{
  public class EditPositionFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditPositionFormFunction ()
    {
    }

    protected EditPositionFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditPositionFormFunction (ObjectID clientID, ObjectID organizationalStructureObjectID)
      : base (clientID, organizationalStructureObjectID)
    {
    }

    // methods and properties
    public Position Position
    {
      get { return (Position) OrganizationalStructureObject; }
      set { OrganizationalStructureObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (OrganizationalStructureObject == null)
      {
        Position = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreatePosition (CurrentTransaction);
        Position.Client = Rubicon.SecurityManager.Domain.OrganizationalStructure.Client.GetObject (ClientID, CurrentTransaction);
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditPositionForm), "OrganizationalStructure/UI/EditPositionForm.aspx");
  }
}
