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

namespace Rubicon.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (EditPermissionsFormResources))]
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

    protected override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadAccessControlLists (interim);

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

        HtmlGenericControl div = new HtmlGenericControl ("div");
        AccessControlListsPlaceHolder.Controls.Add (div);
        div.Controls.Add (editAccessControlListControl);

        _editAccessControlListControls.Add (editAccessControlListControl);
      }
    }

    protected override void SaveValues (bool interim)
    {
      base.SaveValues (interim);

      SaveAccessControlLists (interim);

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
      isValid &= CurrentObject.Validate ();

      return isValid;
    }

    private bool ValidateAccessControlLists ()
    {
      bool isValid = true;
      foreach (EditAccessControlListControl control in _editAccessControlListControls)
        isValid &= control.Validate ();
      
      return isValid;
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      CurrentFunction.CurrentTransaction.Rollback ();
      throw new WxeUserCancelException ();
    }

    protected void NewButton_Click (object sender, EventArgs e)
    {
      SecurableClassDefinition classDefinition = CurrentFunction.SecurableClassDefinition;

      AccessControlList accessControlList = new AccessControlList (classDefinition.ClientTransaction);
      accessControlList.ClassDefinition = classDefinition;

      LoadAccessControlLists (false);
      //AccessControlListsRepeater.LoadValue (false);
      //AccessControlListsRepeater.IsDirty = true;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      EnableNewButton ();
    }

    private void EnableNewButton ()
    {
      DomainObjectCollection stateProperties = CurrentFunction.SecurableClassDefinition.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      int possibleStateCombinations = 1;
      if (stateProperties.Count > 0)
        possibleStateCombinations += ((StatePropertyDefinition) stateProperties[0]).DefinedStates.Count;

      if (CurrentFunction.SecurableClassDefinition.AccessControlLists.Count < possibleStateCombinations)
        NewButton.Enabled = true;
      else
        NewButton.Enabled = false;
    }
  }
}
