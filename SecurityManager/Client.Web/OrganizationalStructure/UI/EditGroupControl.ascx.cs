using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.Classes;
using Rubicon.SecurityManager.Client.Web.OrganizationalStructure.WxeFunctions;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Client.Web.Globalization.OrganizationalStructure.UI;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  [WebMultiLingualResources (typeof (EditGroupControlResources))]
  public partial class EditGroupControl : BaseControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditGroupFormFunction CurrentFunction
    {
      get { return (EditGroupFormFunction) base.CurrentFunction; }
    }

    public override Control FocusControl
    {
      get { return NameField; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      CurrentObject.BusinessObject = CurrentFunction.Group;
      CurrentObject.LoadValues (IsPostBack);

      FillParentField ();
      FillGroupTypeField ();
    }

    private void FillGroupTypeField ()
    {
      GroupTypeField.SetBusinessObjectList (GroupType.FindByClientID (CurrentFunction.ClientID));
    }

    private void FillParentField ()
    {
      List<Group> groups = new List<Group> ();

      foreach (Group group in Group.FindByClientID (CurrentFunction.ClientID))
      {
        if ((!CurrentFunction.Group.Children.Contains (group.ID)) && (group.ID != CurrentFunction.Group.ID))
          groups.Add (group);
      }

      ParentField.SetBusinessObjectList (groups);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void RolesField_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          Role role = new Role (CurrentFunction.CurrentTransaction);
          role.Group = CurrentFunction.Group;

          EditRoleFormFunction editRoleFormFunction = new EditRoleFormFunction (CurrentFunction.ClientID, role.ID);
          editRoleFormFunction.TransactionMode = WxeTransactionMode.None;
          Page.ExecuteFunction (editRoleFormFunction);
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (Role role in RolesField.GetSelectedBusinessObjects ())
        {
          RolesField.RemoveRow (role);
          role.Delete ();
        }

        RolesField.ClearSelectedRows ();
      }
    }

    protected void ChildrenField_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "AddItem")
      {
        if (!Page.IsReturningPostBack)
        {
          SearchGroupFormFunction searchGroupFormFunction = new SearchGroupFormFunction (CurrentFunction.ClientID);
          searchGroupFormFunction.TransactionMode = WxeTransactionMode.None;

          //Page.ExecuteFunction (searchGroupFormFunction);
        }
      }

      if (e.Item.ItemID == "RemoveItem")
        ChildrenField.RemoveRows (ChildrenField.GetSelectedBusinessObjects ());
    }
  }
}