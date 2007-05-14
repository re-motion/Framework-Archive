using System;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditRoleControlResources))]
  public partial class EditRoleControl : BaseControl
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

    protected new EditRoleFormFunction CurrentFunction
    {
      get { return (EditRoleFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return UserField; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      InitializeUserField (IsPostBack);
      InitializeGroupField (IsPostBack);
      InitializePositionField (IsPostBack);
    }

    private void InitializeUserField (bool interim)
    {
      if (CurrentFunction.User == null)
      {
        if (!interim)
          FillUserField ();
      }
      else
      {
        UserField.ReadOnly = NaBoolean.True;
      }
    }

    private void InitializeGroupField (bool interim)
    {
      if (CurrentFunction.Group == null)
      {
        if (!interim)
          FillGroupField ();
      }
      else
      {
        GroupField.ReadOnly = NaBoolean.True;
      }
    }

    private void InitializePositionField (bool interim)
    {
      bool isGroupSelected = GroupField.Value != null;
      PositionField.Enabled = isGroupSelected;
      if (!interim)
        FillPositionField ();
    }

    private void FillUserField ()
    {
      Group group = CurrentFunction.Group;
      if (group != null)
        UserField.SetBusinessObjectList (User.FindByClientID (group.Client.ID, CurrentFunction.Role.ClientTransaction));
    }

    private void FillGroupField ()
    {
      User user = CurrentFunction.User;
      if (user != null)
        GroupField.SetBusinessObjectList (CurrentFunction.Role.GetPossibleGroups (user.Client.ID));
    }

    private void FillPositionField ()
    {
      if (GroupField.Value == null)
        PositionField.ClearBusinessObjectList ();
      else
        PositionField.SetBusinessObjectList (CurrentFunction.Role.GetPossiblePositions ((Group) GroupField.Value));
    }

    protected void GroupField_SelectionChanged (object sender, EventArgs e)
    {
      InitializePositionField (false);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }
  }
}