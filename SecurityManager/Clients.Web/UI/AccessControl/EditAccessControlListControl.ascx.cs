using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditAccessControlListControl : BaseControl
  {
    // types

    // static members and constants

    private static readonly object s_deleteEvent = new object ();

    // member fields
    
    private List<EditStateCombinationControl> _editStateCombinationControls = new List<EditStateCombinationControl> ();
    private List<EditAccessControlEntryControl> _editAccessControlEntryControls = new List<EditAccessControlEntryControl> ();
    private bool _isCreatingNewStateCombination;

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
      DomainObjectCollection stateProperties = CurrentAccessControlList.Class.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      int possibleStateCombinations = 1;
      if (stateProperties.Count > 0)
        possibleStateCombinations += ((StatePropertyDefinition) stateProperties[0]).DefinedStates.Count;

      if (CurrentAccessControlList.Class.StateCombinations.Count < possibleStateCombinations)
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

      for (int i = 0; i < stateCombinations.Count; i++)
      {
        StateCombination stateCombination = (StateCombination) stateCombinations[i];

        EditStateCombinationControl editStateCombinationControl = (EditStateCombinationControl) LoadControl ("EditStateCombinationControl.ascx");
        editStateCombinationControl.ID = "SC_" + i.ToString ();
        editStateCombinationControl.BusinessObject = stateCombination;
        editStateCombinationControl.Delete += new EventHandler (EditStateCombinationControl_Delete);

        HtmlGenericControl div = new HtmlGenericControl ("div");
        div.Attributes.Add ("class", "stateCombinationContainer");
        StateCombinationControls.Controls.Add (div);
        div.Controls.Add (editStateCombinationControl);

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

      for (int i = 0; i < accessControlEntries.Count; i++)
      {
        AccessControlEntry accessControlEntry = (AccessControlEntry) accessControlEntries[i];

        EditAccessControlEntryControl editAccessControlEntryControl = (EditAccessControlEntryControl) LoadControl ("EditAccessControlEntryControl.ascx");
        editAccessControlEntryControl.ID = "Ace_" + i.ToString ();
        editAccessControlEntryControl.BusinessObject = accessControlEntry;
        editAccessControlEntryControl.Delete += new EventHandler(EditAccessControlEntryControl_Delete);

        HtmlGenericControl div = new HtmlGenericControl ("div");
        div.Attributes.Add ("class", "accessControlEntryContainer");
        AccessControlEntryControls.Controls.Add (div);
        div.Controls.Add (editAccessControlEntryControl);

        _editAccessControlEntryControls.Add (editAccessControlEntryControl);
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

    private bool ValidateStateCombinations (params EditStateCombinationControl[] excludedControls)
    {
      List<EditStateCombinationControl> excludedControlList = new List<EditStateCombinationControl> (excludedControls);

      bool isValid = true;
      foreach (EditStateCombinationControl control in _editStateCombinationControls)
      {
        if (!excludedControlList.Contains (control))
          isValid &= control.Validate ();
      }

      if (!_isCreatingNewStateCombination)
      {
        MissingStateCombinationsValidator.Validate ();
        isValid &= MissingStateCombinationsValidator.IsValid;
      }

      return isValid;
    }

    protected void MissingStateCombinationsValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = CurrentAccessControlList.StateCombinations.Count > 0;
    }

    private bool ValidateAccessControlEntries (params EditAccessControlEntryControl[] excludedControls)
    {
      List<EditAccessControlEntryControl> excludedControlList = new List<EditAccessControlEntryControl> (excludedControls);

      bool isValid = true;
      foreach (EditAccessControlEntryControl control in _editAccessControlEntryControls)
      {
        if (!excludedControlList.Contains (control))
          isValid &= control.Validate ();
      }

      return isValid;
    }

    protected void NewStateCombinationButton_Click (object sender, EventArgs e)
    {
      _isCreatingNewStateCombination = true;
      Page.PrepareValidation ();
      bool isValid = Validate ();
      if (!isValid)
      {
        return;
      }
      SaveValues (false);
      Page.IsDirty = true;

      CurrentAccessControlList.CreateStateCombination ();

      LoadStateCombinations (false);
      //StateCombinationsRepeater.LoadValue (false);
      //StateCombinationsRepeater.IsDirty = true;
      _isCreatingNewStateCombination = false;
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

      CurrentAccessControlList.CreateAccessControlEntry ();

      LoadAccessControlEntries (false);
      //AccessControlEntriesRepeater.LoadValue (false);
      //AccessControlEntriesRepeater.IsDirty = true;
    }

    void EditStateCombinationControl_Delete (object sender, EventArgs e)
    {
      EditStateCombinationControl editStateCombinationControl = (EditStateCombinationControl) sender;
      Page.PrepareValidation ();
      bool isValid = ValidateStateCombinations (editStateCombinationControl);
      if (!isValid)
        return;

      _editStateCombinationControls.Remove (editStateCombinationControl);
      StateCombination accessControlEntry = (StateCombination) editStateCombinationControl.DataSource.BusinessObject;
      accessControlEntry.Delete ();

      SaveStateCombinations (false);
      Page.IsDirty = true;

      LoadStateCombinations (false);
    }

    private void EditAccessControlEntryControl_Delete (object sender, EventArgs e)
    {
      EditAccessControlEntryControl editAccessControlEntryControl = (EditAccessControlEntryControl) sender;
      Page.PrepareValidation ();
      bool isValid = ValidateAccessControlEntries (editAccessControlEntryControl);
      if (!isValid)
        return;

      _editAccessControlEntryControls.Remove (editAccessControlEntryControl);
      AccessControlEntry accessControlEntry = (AccessControlEntry) editAccessControlEntryControl.DataSource.BusinessObject;
      accessControlEntry.Delete ();

      SaveAccessControlEntries (false);
      Page.IsDirty = true;

      LoadAccessControlEntries (false);
    }

    protected void DeleteAccessControlListButton_Click (object sender, EventArgs e)
    {
      EventHandler handler = (EventHandler) Events[s_deleteEvent];
      if (handler != null)
        handler (this, e);
    }

    public event EventHandler Delete
    {
      add { Events.AddHandler (s_deleteEvent, value); }
      remove { Events.RemoveHandler (s_deleteEvent, value); }
    }
  }
}