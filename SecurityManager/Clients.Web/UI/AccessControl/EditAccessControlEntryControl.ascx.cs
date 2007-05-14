using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using Rubicon.Data.DomainObjects;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Web.UI.Globalization;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditAccessControlEntryControl : BaseControl
  {
    // types

    // static members and constants

    private static readonly object s_deleteEvent = new object ();

    // member fields

    private List<EditPermissionControl> _editPermissionControls = new List<EditPermissionControl> ();

    // construction and disposing

    // methods and properties

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public event EventHandler Delete
    {
      add { Events.AddHandler (s_deleteEvent, value); }
      remove { Events.RemoveHandler (s_deleteEvent, value); }
    }

    protected AccessControlEntry CurrentAccessControlEntry
    {
      get { return (AccessControlEntry) CurrentObject.BusinessObject; }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SpecificPositionAndGroupLinkingLabel.Text = AccessControlResources.SpecificPositionAndGroupLinkingLabelText;

      UpdateActualPriority ();
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadPermissions (interim);
      AdjustSpecificClientField ();
      AdjustPositionFields ();
    }

    public override void SaveValues (bool interim)
    {
      using (new SecurityFreeSection ())
      {
        base.SaveValues (interim);
      }

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

    protected void DeleteAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      EventHandler handler = (EventHandler) Events[s_deleteEvent];
      if (handler != null)
        handler (this, e);
    }

    protected void ClientField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificClientField ();
    }

    protected void SpecificPositionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustPositionFields ();
    }

    private void AdjustSpecificClientField ()
    {
      if ((ClientSelection) ClientField.Value == ClientSelection.SpecificClient)
      {
        SpecificClientField.Visible = true;
      }
      else
      {
        SpecificClientField.Visible = false;
        SpecificClientField.Value = null;
      }
    }

    private void AdjustPositionFields ()
    {
      if (SpecificPositionField.BusinessObjectID == null)
        CurrentAccessControlEntry.User = UserSelection.All;
      else
        CurrentAccessControlEntry.User = UserSelection.SpecificPosition;

      // TODO: Remove when Group can stand alone during ACE lookup.
      if (SpecificPositionField.BusinessObjectID == null)
      {
        SpecificPositionAndGroupLinkingLabel.Visible = false;
        GroupField.Visible = false;
        GroupField.Value = GroupSelection.All;
      }
      else
      {
        SpecificPositionAndGroupLinkingLabel.Visible = true;
        GroupField.Visible = true;
      }
    }

    private void UpdateActualPriority ()
    {
      foreach (IBusinessObjectBoundEditableWebControl control in CurrentObject.BoundControls)
      {
        if (control.IsDirty)
        {
          BocTextValue bocTextValue = control as BocTextValue;
          if (bocTextValue != null && !bocTextValue.IsValidValue)
            continue;

          using (new SecurityFreeSection ())
          {
            control.SaveValue (false);
          }
          control.IsDirty = true;
        }
      }

      ActualPriorityLabel.Text = string.Format (AccessControlResources.ActualPriorityLabelText, CurrentAccessControlEntry.ActualPriority);
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

      HtmlGenericControl ul = new HtmlGenericControl ("ul");
      ul.Attributes.Add ("class", "permissionsList");
      PermissionsPlaceHolder.Controls.Add (ul);

      for (int i = 0; i < permissions.Count; i++)
      {
        Permission permission = (Permission) permissions[i];

        EditPermissionControl control = (EditPermissionControl) LoadControl ("EditPermissionControl.ascx");
        control.ID = "P_" + i.ToString ();
        control.BusinessObject = permission;

        HtmlGenericControl li = new HtmlGenericControl ("li");
        li.Attributes.Add ("class", "permissionsList");
        ul.Controls.Add (li);
        li.Controls.Add (control);

        _editPermissionControls.Add (control);
      }
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
