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
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditAccessControlListControl : BaseControl
  {
    // types

    // static members and constants

    // member fields
    private List<EditStateCombinationControl> _editStateCombinationControls = new List<EditStateCombinationControl> ();
    private List<EditAccessControlEntryControl> _editAccessControlEntryControls = new List<EditAccessControlEntryControl> ();

    // construction and disposing

    // methods and properties

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected AccessControlList CurrentAccessControlList
    {
      get { return (AccessControlList) CurrentObject.BusinessObject; }
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      EnableNewStateCombinationButton ();
    }

    private void EnableNewStateCombinationButton ()
    {
      DomainObjectCollection stateProperties = CurrentAccessControlList.ClassDefinition.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      int possibleStateCombinations = 1;
      if (stateProperties.Count > 0)
        possibleStateCombinations += ((StatePropertyDefinition) stateProperties[0]).DefinedStates.Count;

      if (CurrentAccessControlList.StateCombinations.Count < possibleStateCombinations)
        NewStateCombinationButton.Enabled = true;
      else
        NewStateCombinationButton.Enabled = false;
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadStateCombinations (interim);
      LoadAccessControlEntries (interim);
    }

    private void LoadStateCombinations (bool interim)
    {
      CreateEditStateCombinationControls (CurrentAccessControlList.StateCombinations);
      foreach (EditStateCombinationControl control in _editStateCombinationControls)
        control.LoadValues (interim);
    }

    private void CreateEditStateCombinationControls (DomainObjectCollection stateCombinations)
    {
      StateCombinationControls.Controls.Clear ();
      _editStateCombinationControls.Clear ();

      HtmlTable table = new HtmlTable ();
      StateCombinationControls.Controls.Add (table);

      for (int i = 0; i < stateCombinations.Count; i++)
      {
        StateCombination stateCombination = (StateCombination) stateCombinations[i];

        EditStateCombinationControl editStateCombinationControl = (EditStateCombinationControl) LoadControl ("EditStateCombinationControl.ascx");
        editStateCombinationControl.ID = "SC_" + i.ToString ();
        editStateCombinationControl.BusinessObject = stateCombination;

        HtmlTableRow row = new HtmlTableRow ();
        table.Rows.Add (row);
        HtmlTableCell cell = new HtmlTableCell ();
        row.Cells.Add (cell);
        cell.Controls.Add (editStateCombinationControl);

        _editStateCombinationControls.Add (editStateCombinationControl);
      }
    }

    private void LoadAccessControlEntries (bool interim)
    {
      CreateEditAccessControlEntryControls (CurrentAccessControlList.AccessControlEntries);
      foreach (EditAccessControlEntryControl control in _editAccessControlEntryControls)
        control.LoadValues (interim);
    }

    private void CreateEditAccessControlEntryControls (DomainObjectCollection accessControlEntries)
    {
      AccessControlEntryControls.Controls.Clear ();
      _editAccessControlEntryControls.Clear ();

      HtmlTable table = new HtmlTable ();
      AccessControlEntryControls.Controls.Add (table);

      for (int i = 0; i < accessControlEntries.Count; i++)
      {
        AccessControlEntry accessControlEntry = (AccessControlEntry) accessControlEntries[i];

        EditAccessControlEntryControl control = (EditAccessControlEntryControl) LoadControl ("EditAccessControlEntryControl.ascx");
        control.ID = "Ace_" + i.ToString ();
        control.BusinessObject = accessControlEntry;
        HtmlTableRow row = new HtmlTableRow ();
        table.Rows.Add (row);
        HtmlTableCell cell = new HtmlTableCell ();
        row.Cells.Add (cell);
        cell.Controls.Add (control);
        _editAccessControlEntryControls.Add (control);
      }
    }

    public override void SaveValues (bool interim)
    {
      base.SaveValues (interim);

      SaveStateCombinations (interim);
      SaveAccessControlEntries (interim);
    }

    private void SaveStateCombinations (bool interim)
    {
      foreach (EditStateCombinationControl control in _editStateCombinationControls)
        control.SaveValues (interim);
    }

    private void SaveAccessControlEntries (bool interim)
    {
      foreach (EditAccessControlEntryControl control in _editAccessControlEntryControls)
        control.SaveValues (interim);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= ValidateStateCombinations ();
      isValid &= ValidateAccessControlEntries ();

      return isValid;
    }

    private bool ValidateStateCombinations ()
    {
      bool isValid = true;
      foreach (EditStateCombinationControl control in _editStateCombinationControls)
        isValid &= control.Validate ();

      return isValid;
    }

    private bool ValidateAccessControlEntries ()
    {
      bool isValid = true;
      foreach (EditAccessControlEntryControl control in _editAccessControlEntryControls)
        isValid &= control.Validate ();

      return isValid;
    }

    protected void NewStateCombinationButton_Click (object sender, EventArgs e)
    {
      Page.PrepareValidation ();
      bool isValid = Validate ();
      if (!isValid)
      {
        return;
      }
      SaveValues (false);
      Page.IsDirty = true;
      
      StateCombination stateCombination = new StateCombination (CurrentAccessControlList.ClientTransaction);
      stateCombination.ClassDefinition = CurrentAccessControlList.ClassDefinition;
      stateCombination.AccessControlList = CurrentAccessControlList;

      LoadStateCombinations (false);
      //StateCombinationsRepeater.LoadValue (false);
      //StateCombinationsRepeater.IsDirty = true;
    }

    protected void NewAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      Page.PrepareValidation ();
      bool isValid = Validate ();
      if (!isValid)
      {
        return;
      }
      SaveValues (false);
      Page.IsDirty = true;

      AccessControlEntry accessControlEntry = new AccessControlEntry (CurrentAccessControlList.ClientTransaction);
      foreach (AccessTypeDefinition accessTypeDefinition in CurrentAccessControlList.ClassDefinition.AccessTypes)
      {
        Permission permission = new Permission (CurrentAccessControlList.ClientTransaction);
        permission.AccessType = accessTypeDefinition;
        permission.AccessControlEntry = accessControlEntry;
      }
      accessControlEntry.AccessControlList = CurrentAccessControlList;

      LoadAccessControlEntries (false);
      //AccessControlEntriesRepeater.LoadValue (false);
      //AccessControlEntriesRepeater.IsDirty = true;
    }
  }
}