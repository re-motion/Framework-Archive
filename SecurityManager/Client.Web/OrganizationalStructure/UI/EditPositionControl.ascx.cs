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
  [WebMultiLingualResources (typeof (EditPositionControlResources))]
  public partial class EditPositionControl : BaseControl
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

    protected new EditPositionFormFunction CurrentFunction
    {
      get { return (EditPositionFormFunction) base.CurrentFunction; }
    }

    public override Control FocusControl
    {
      get { return NameField; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      CurrentObject.BusinessObject = CurrentFunction.Position;
      CurrentObject.LoadValues (IsPostBack);
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
          role.Position = CurrentFunction.Position;

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

    protected void ConcretePositionsField_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          ConcretePosition concretePosition = new ConcretePosition (CurrentFunction.CurrentTransaction);
          concretePosition.Position = CurrentFunction.Position;

          EditConcretePositionFormFunction editConcretePositionFormFunction = 
            new EditConcretePositionFormFunction (CurrentFunction.ClientID, concretePosition.ID);

          editConcretePositionFormFunction.TransactionMode = WxeTransactionMode.None;
          Page.ExecuteFunction (editConcretePositionFormFunction);
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (ConcretePosition concretePosition in ConcretePositionsField.GetSelectedBusinessObjects ())
        {
          ConcretePositionsField.RemoveRow (concretePosition);
          concretePosition.Delete ();
        }

        ConcretePositionsField.ClearSelectedRows ();
      }
    }
  }
}