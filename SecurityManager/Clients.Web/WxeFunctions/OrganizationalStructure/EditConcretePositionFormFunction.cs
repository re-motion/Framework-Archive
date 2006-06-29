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
using Rubicon.SecurityManager.Configuration;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
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

    protected EditConcretePositionFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditConcretePositionFormFunction (ObjectID clientID, ObjectID organizationalStructureObjectID, Position position, GroupType groupType)
      : base (clientID, organizationalStructureObjectID)
    {
      GroupType = groupType;
      Position = position;
    }

    // methods and properties
    public GroupType GroupType
    {
      get { return (GroupType) Variables["GroupType"]; }
      set { Variables["GroupType"] = value; }
    }

    public Position Position
    {
      get { return (Position) Variables["Position"]; }
      set { Variables["Position"] = value; }
    }

    public ConcretePosition ConcretePosition
    {
      get { return (ConcretePosition) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        ConcretePosition = new ConcretePosition (CurrentTransaction);
        ConcretePosition.GroupType = GroupType;
        ConcretePosition.Position = Position;
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditConcretePositionForm), "UI/OrganizationalStructure/EditConcretePositionForm.aspx");
  }
}
