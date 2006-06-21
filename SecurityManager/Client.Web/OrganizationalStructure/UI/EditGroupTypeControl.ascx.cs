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
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
{
  [WebMultiLingualResources (typeof (EditGroupTypeControlResources))]
  public partial class EditGroupTypeControl : BaseControl
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

    protected new EditGroupTypeFormFunction CurrentFunction
    {
      get { return (EditGroupTypeFormFunction) base.CurrentFunction; }
    }

    public override Control FocusControl
    {
      get { return NameField; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      CurrentObject.BusinessObject = CurrentFunction.GroupType;
      CurrentObject.LoadValues (IsPostBack);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void ConcretePositionsField_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          ConcretePosition concretePosition = new ConcretePosition (CurrentFunction.CurrentTransaction);
          concretePosition.GroupType = CurrentFunction.GroupType;

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

    protected void GroupsField_MenuItemClick (object sender, Rubicon.Web.UI.Controls.WebMenuItemClickEventArgs e)
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
        GroupsField.RemoveRows (GroupsField.GetSelectedBusinessObjects ());
    }
  }
}