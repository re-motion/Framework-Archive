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
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.SecurityManager.Client.Web.UI.OrganizationalStructure;

namespace Rubicon.SecurityManager.Client.Web.WxeFunctions.OrganizationalStructure
{
  public class EditRoleFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditRoleFormFunction ()
    {
    }

    protected EditRoleFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditRoleFormFunction (ObjectID clientID, ObjectID organizationalStructureObjectID, User user, Group group, Position position)
      : base (clientID, organizationalStructureObjectID)
    {
      User = user;
      Group = group;
      Position = position;
    }

    // methods and properties
    public User User
    {
      get { return (User) Variables["User"]; }
      set { Variables["User"] = value; }
    }

    public Group Group
    {
      get { return (Group) Variables["Group"]; }
      set { Variables["Group"] = value; }
    }

    public Position Position
    {
      get { return (Position) Variables["Position"]; }
      set { Variables["Position"] = value; }
    }

    public Role Role
    {
      get { return (Role) OrganizationalStructureObject; }
      set { OrganizationalStructureObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (OrganizationalStructureObject == null)
      {
        Role = new Role (CurrentTransaction);
        Role.User = User;
        Role.Group = Group;
        Role.Position = Position;
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditRoleForm), "OrganizationalStructure/UI/EditRoleForm.aspx");

  }
}
