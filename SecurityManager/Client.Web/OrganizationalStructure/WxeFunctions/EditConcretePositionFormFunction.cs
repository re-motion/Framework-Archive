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
  public class EditConcretePositionFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditConcretePositionFormFunction ()
    {
    }

    public EditConcretePositionFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditConcretePositionFormFunction (ObjectID clientID, ObjectID organizationalStructureObjectID)
      : base (clientID, organizationalStructureObjectID)
    {
    }

    // methods and properties
    public ConcretePosition ConcretePosition
    {
      get { return (ConcretePosition) OrganizationalStructureObject; }
      set { OrganizationalStructureObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (OrganizationalStructureObject == null)
        ConcretePosition = new ConcretePosition (CurrentTransaction);
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditConcretePositionForm), "OrganizationalStructure/UI/EditConcretePositionForm.aspx");
  }
}
