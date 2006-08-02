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
  public class EditGroupTypePositionFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditGroupTypePositionFormFunction ()
    {
    }

    protected EditGroupTypePositionFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditGroupTypePositionFormFunction (ObjectID clientID, ObjectID organizationalStructureObjectID, Position position, GroupType groupType)
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

    public GroupTypePosition GroupTypePosition
    {
      get { return (GroupTypePosition) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        GroupTypePosition = new GroupTypePosition (CurrentTransaction);
        GroupTypePosition.GroupType = GroupType;
        GroupTypePosition.Position = Position;
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditGroupTypePositionForm), "UI/OrganizationalStructure/EditGroupTypePositionForm.aspx");
  }
}
