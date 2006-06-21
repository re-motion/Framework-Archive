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
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.WxeFunctions
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

    public EditRoleFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditRoleFormFunction (ObjectID clientID, ObjectID organizationalStructureObjectID)
      : base (clientID, organizationalStructureObjectID)
    {
    }

    // methods and properties
    public Role Role
    {
      get { return (Role) OrganizationalStructureObject; }
      set { OrganizationalStructureObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (OrganizationalStructureObject == null)
        Role = new Role (CurrentTransaction);
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditRoleForm), "OrganizationalStructure/UI/EditRoleForm.aspx");

  }
}
