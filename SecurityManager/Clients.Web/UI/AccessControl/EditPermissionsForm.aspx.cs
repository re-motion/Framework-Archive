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
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Rubicon.SecurityManager.Clients.Web.WxeFunctions.AccessControl;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using System.Collections.Generic;
using Rubicon.Web.UI;

namespace Rubicon.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditPermissionsForm : BaseEditPage
  {

    // types

    // static members and constants

    // member fields

    private List<EditAccessControlListControl> _editAccessControlListControls = new List<EditAccessControlListControl> ();

    // construction and disposing

    // methods and properties

    protected new EditPermissionsFormFunction CurrentFunction
    {
      get { return (EditPermissionsFormFunction) base.CurrentFunction; }
    }

    protected override void OnPreRenderComplete (EventArgs e)
    {
      HtmlHeadAppender.Current.SetTitle (string.Format (AccessControlResources.Title, CurrentFunction.SecurableClassDefinition.DisplayName));
      base.OnPreRenderComplete (e);
    }

    protected override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadAccessControlLists (interim);

      CurrentObjectHeaderControls.BusinessObject = CurrentFunction.SecurableClassDefinition;
      CurrentObjectHeaderControls.LoadValues (interim);

      CurrentObject.BusinessObject = CurrentFunction.SecurableClassDefinition;
      CurrentObject.LoadValues (interim);
    }

    private void LoadAccessControlLists (bool interim)
    {
      CreateEditAccessControlListControls (CurrentFunction.SecurableClassDefinition.AccessControlLists);
      foreach (EditAccessControlListControl control in _editAccessControlListControls)
        control.LoadValues (interim);
    }

    private void CreateEditAccessControlListControls (DomainObjectCollection accessControlLists)
    {
      AccessControlListsPlaceHolder.Controls.Clear();
      _editAccessControlListControls.Clear ();
      for (int i = 0; i < accessControlLists.Count; i++)
      {
        AccessControlList accessControlList = (AccessControlList) accessControlLists[i];

        EditAccessControlListControl editAccessControlListControl = (EditAccessControlListControl) LoadControl ("EditAccessControlListControl.ascx");
        editAccessControlListControl.ID = "Acl_" + i.ToString ();
        editAccessControlListControl.BusinessObject = accessControlList;
        editAccessControlListControl.Delete += new EventHandler (EditAccessControlListControl_Delete);

        HtmlGenericControl div = new HtmlGenericControl ("div");
        div.Attributes.Add ("class", "accessControlListContainer");
        AccessControlListsPlaceHolder.Controls.Add (div);
        div.Controls.Add (editAccessControlListControl);

        _editAccessControlListControls.Add (editAccessControlListControl);
      }
    }

    protected override void SaveValues (bool interim)
    {
      base.SaveValues (interim);

      SaveAccessControlLists (interim);

      CurrentObjectHeaderControls.SaveValues (interim);
      CurrentObject.SaveValues (interim);
    }

    private void SaveAccessControlLists (bool interim)
    {
      foreach (EditAccessControlListControl control in _editAccessControlListControls)
        control.SaveValues (interim);
    }

    protected override bool ValidatePage ()
    {
      bool isValid = true;
      isValid &= base.ValidatePage ();
      isValid &= ValidateAccessControlLists ();
      isValid &= CurrentObjectHeaderControls.Validate ();
      isValid &= CurrentObject.Validate ();

      return isValid;
    }

    private bool ValidateAccessControlLists (params EditAccessControlListControl[] excludedControls)
    {
      List<EditAccessControlListControl> excludedControlList = new List<EditAccessControlListControl> (excludedControls);

      bool isValid = true;
      foreach (EditAccessControlListControl control in _editAccessControlListControls)
      {
        if (!excludedControlList.Contains (control))
          isValid &= control.Validate ();
      }
      
      return isValid;
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      CurrentFunction.CurrentTransaction.Rollback ();
      throw new WxeUserCancelException ();
    }

    protected void NewAccessControlListButton_Click (object sender, EventArgs e)
    {
      PrepareValidation ();
      bool isValid = ValidateAccessControlLists ();
      if (!isValid)
        return;

      SaveAccessControlLists (false);
      IsDirty = true;

      CurrentFunction.SecurableClassDefinition.CreateAccessControlList ();

      LoadAccessControlLists (false);
      //AccessControlListsRepeater.LoadValue (false);
      //AccessControlListsRepeater.IsDirty = true;
    }

    private void EditAccessControlListControl_Delete (object sender, EventArgs e)
    {
      EditAccessControlListControl editAccessControlListControl = (EditAccessControlListControl) sender;
      PrepareValidation ();
      bool isValid = ValidateAccessControlLists (editAccessControlListControl);
      if (!isValid)
        return;

      _editAccessControlListControls.Remove (editAccessControlListControl);
      AccessControlList accessControlList = (AccessControlList) editAccessControlListControl.DataSource.BusinessObject;
      accessControlList.Delete ();

      SaveAccessControlLists (false);
      IsDirty = true;

      LoadAccessControlLists (false);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      EnableNewAccessControlListButton ();
    }

    private void EnableNewAccessControlListButton ()
    {
      DomainObjectCollection stateProperties = CurrentFunction.SecurableClassDefinition.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      int possibleStateCombinations = 1;
      if (stateProperties.Count > 0)
        possibleStateCombinations += ((StatePropertyDefinition) stateProperties[0]).DefinedStates.Count;

      if (CurrentFunction.SecurableClassDefinition.StateCombinations.Count < possibleStateCombinations)
        NewAccessControlListButton.Enabled = true;
      else
        NewAccessControlListButton.Enabled = false;
    }
  }
}
