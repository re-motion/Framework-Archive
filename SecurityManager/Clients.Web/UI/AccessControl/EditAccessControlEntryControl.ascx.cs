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
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Rubicon.SecurityManager.Domain.AccessControl;

namespace Rubicon.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditAccessControlEntryControl : BaseControl
  {
    // types

    // static members and constants

    // member fields
    private List<EditPermissionControl> _editPermissionControls = new List<EditPermissionControl> ();

    // construction and disposing

    // methods and properties
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected AccessControlEntry CurrentAccessControlEntry
    {
      get { return (AccessControlEntry) CurrentObject.BusinessObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      PermissionsLabel.Text = AccessControlResources.PermissionsLabel;
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);
      if (interim)
        ActualPriority.LoadValue (false);

      LoadPermissions (interim);
    }

    private void LoadPermissions (bool interim)
    {
      CreateEditPermissionControls (CurrentAccessControlEntry.Permissions);
      foreach (EditPermissionControl control in _editPermissionControls)
        control.LoadValues (interim);
    }

    private void CreateEditPermissionControls (DomainObjectCollection permissions)
    {
      PermissionsPlaceHolder.Controls.Clear ();
      _editPermissionControls.Clear ();

      HtmlTable table = new HtmlTable ();
      PermissionsPlaceHolder.Controls.Add (table);

      for (int i = 0; i < permissions.Count; i++)
      {
        Permission permission = (Permission) permissions[i];

        EditPermissionControl control = (EditPermissionControl) LoadControl ("EditPermissionControl.ascx");
        control.ID = "P_" + i.ToString ();
        control.BusinessObject = permission;

        HtmlTableRow row = new HtmlTableRow ();
        table.Rows.Add (row);
        HtmlTableCell cell = new HtmlTableCell ();
        row.Cells.Add (cell);
        cell.Controls.Add (control);

        _editPermissionControls.Add (control);
      }
    }

    public override void SaveValues (bool interim)
    {
      base.SaveValues (interim);

      if (CurrentAccessControlEntry.SpecificPosition != null)
        CurrentAccessControlEntry.User = UserSelection.SpecificPosition;
      else
        CurrentAccessControlEntry.User = UserSelection.All;

      SavePermissions (interim);
    }

    private void SavePermissions (bool interim)
    {
      foreach (EditPermissionControl control in _editPermissionControls)
        control.SaveValues (interim);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();
      isValid &= ValidatePermissions ();

      return isValid;
    }

    private bool ValidatePermissions ()
    {
      bool isValid = true;
      foreach (EditPermissionControl control in _editPermissionControls)
        isValid &= control.Validate ();

      return isValid;
    }
  }
}