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
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.NullableValueTypes;
using Rubicon.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.Classes;

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

      if (CurrentFunction.User != null)
        UserField.ReadOnly = NaBoolean.True;
      else
        FillUserField ();

      if (CurrentFunction.Group != null)
        GroupField.ReadOnly = NaBoolean.True;
      else
        FillGroupField ();

      if (CurrentFunction.Position != null)
        PositionField.ReadOnly = NaBoolean.True;
      else
        FillPositionField ();
    }

    private void FillUserField ()
    {
      UserField.SetBusinessObjectList (User.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction));
    }

    private void FillPositionField ()
    {
      PositionField.SetBusinessObjectList (Position.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction));
    }

    private void FillGroupField ()
    {
      GroupField.SetBusinessObjectList (Group.FindByClientID (CurrentFunction.ClientID, CurrentFunction.CurrentTransaction));
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }
  }
}