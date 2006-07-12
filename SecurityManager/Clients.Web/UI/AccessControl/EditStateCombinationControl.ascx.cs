using System;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Web.UI.Controls;
using Rubicon.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditStateCombinationControl : BaseControl
  {
    // types

    // static members and constants

    private static readonly object s_deleteEvent = new object ();

    // member fields

    // construction and disposing

    // methods and properties
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected StateCombination CurrentStateCombination
    {
      get { return (StateCombination) CurrentObject.BusinessObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      BocMenuItem deleteMenuItem = new BocMenuItem ();
      deleteMenuItem.ItemID = "Delete";
      deleteMenuItem.Text = "$res:DeleteStateCombinationCommand";
      deleteMenuItem.Command.Type = CommandType.Event;
      deleteMenuItem.Command.Click += new CommandClickEventHandler (DeleteStateCombinationCommand_Click);
      StateDefinitionField.OptionsMenuItems.Add (deleteMenuItem);
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      if (!interim)
        FillStateDefinitionField ();

      StateDefinition currentStateDefinition = GetStateDefinition (CurrentStateCombination);
      StateDefinitionField.LoadUnboundValue (currentStateDefinition, interim);
    }

    private void FillStateDefinitionField ()
    {
      DomainObjectCollection stateProperties = CurrentStateCombination.Class.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      //bool isRequired = IsEmptyStateDefinitionAlreadyInUse (CurrentStateCombination.ClassDefinition);
      //StateDefinitionField.Required = isRequired ? NaBooleanEnum.True : NaBooleanEnum.False;

      List<StateDefinition> possibleStateDefinitions = new List<StateDefinition> ();
      if (stateProperties.Count > 0)
      {
        StatePropertyDefinition statePropertyDefinition = (StatePropertyDefinition) stateProperties[0];
        foreach (StateDefinition stateDefinition in statePropertyDefinition.DefinedStates)
        {
          //if (!IsStateDefinitionAlreadyInUse (CurrentStateCombination.ClassDefinition, stateDefinition))
          possibleStateDefinitions.Add (stateDefinition);
        }
      }
      StateDefinitionField.SetBusinessObjectList (possibleStateDefinitions);
    }

    //private bool IsStateDefinitionAlreadyInUse (SecurableClassDefinition classDefinition, params StateDefinition[] stateDefinitions)
    //{
    //  StateCombination stateCombination = classDefinition.FindStateCombination (new List<StateDefinition> (stateDefinitions));
    //  if (stateCombination == CurrentStateCombination || stateCombination == null)
    //    return false;
    //  return true;
    //}

    //private bool IsEmptyStateDefinitionAlreadyInUse (SecurableClassDefinition classDefinition)
    //{
    //  StateCombination emptyStateCombination = classDefinition.FindStateCombination (new List<StateDefinition>());
    //  return emptyStateCombination != null;
    //}

    private StateDefinition GetStateDefinition (StateCombination stateCombination)
    {
      if (stateCombination.StateUsages.Count == 0)
        return null;

      StateUsage stateUsage = (StateUsage) stateCombination.StateUsages[0];
      return stateUsage.StateDefinition;
    }

    public override void SaveValues (bool interim)
    {
      base.SaveValues (interim);

      string id = StateDefinitionField.BusinessObjectID;
      if (StringUtility.IsNullOrEmpty (id))
      {
        foreach (StateUsage stateUsage in CurrentStateCombination.StateUsages)
          stateUsage.Delete ();
      }
      else 
      {
        StateDefinition stateDefinition = StateDefinition.GetObject (ObjectID.Parse (id), CurrentStateCombination.ClientTransaction);
        StateUsage stateUsage;
        if (CurrentStateCombination.StateUsages.Count == 0)
        {
          stateUsage = new StateUsage (CurrentStateCombination.ClientTransaction);
          stateUsage.StateCombination = CurrentStateCombination;
        }
        else
        {
          stateUsage = (StateUsage) CurrentStateCombination.StateUsages[0];
        }
        if (stateUsage.StateDefinition != stateDefinition)
          stateUsage.StateDefinition = stateDefinition;
      }
    }

    protected void DeleteStateCombinationCommand_Click (object sender, CommandClickEventArgs e)
    {
      EventHandler handler = (EventHandler) Events[s_deleteEvent];
      if (handler != null)
        handler (this, EventArgs.Empty);
    }

    public event EventHandler Delete
    {
      add { Events.AddHandler (s_deleteEvent, value); }
      remove { Events.RemoveHandler (s_deleteEvent, value); }
    }
  }
}