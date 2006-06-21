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
using Rubicon.NullableValueTypes;

namespace Rubicon.SecurityManager.Client.Web.OrganizationalStructure.UI
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

    public override Control FocusControl
    {
      get { return GroupField; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      CurrentObject.BusinessObject = CurrentFunction.Role;
      CurrentObject.LoadValues (IsPostBack);

      if (CurrentFunction.Role.User != null)
        UserField.ReadOnly = NaBoolean.True;
      else
        FillUserField ();

      if (CurrentFunction.Role.Group != null)
        GroupField.ReadOnly = NaBoolean.True;
      else
        FillGroupField ();

      if (CurrentFunction.Role.Position != null)
        PositionField.ReadOnly = NaBoolean.True;
      else
        FillPositionField ();
    }

    private void FillUserField ()
    {
      UserField.SetBusinessObjectList (User.FindByClientID (CurrentFunction.ClientID));
    }

    private void FillPositionField ()
    {
      PositionField.SetBusinessObjectList (Position.FindByClientID (CurrentFunction.ClientID));
    }

    private void FillGroupField ()
    {
      GroupField.SetBusinessObjectList (Group.FindByClientID (CurrentFunction.ClientID));
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }
  }
}