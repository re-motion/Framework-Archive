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
  [WebMultiLingualResources (typeof (EditGroupTypePositionControlResources))]
  public partial class EditGroupTypePositionControl : BaseControl
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

    protected new EditGroupTypePositionFormFunction CurrentFunction
    {
      get { return (EditGroupTypePositionFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get 
      {
        if (!GroupTypeField.IsReadOnly)
          return GroupTypeField;
        else if (!PositionField.IsReadOnly)
          return PositionField;
        else
          return null;
      }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (CurrentFunction.Position != null)
        PositionField.ReadOnly = NaBoolean.True;
      else
        FillPositionField ();

      if (CurrentFunction.GroupType != null)
        GroupTypeField.ReadOnly = NaBoolean.True;
      else
        FillGroupTypeField ();
    }

    private void FillGroupTypeField ()
    {
      GroupTypeField.SetBusinessObjectList (GroupType.FindAll ());
    }

    private void FillPositionField ()
    {
      PositionField.SetBusinessObjectList (Position.FindAll ());
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }
  }
}