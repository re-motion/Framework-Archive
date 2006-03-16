using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Rubicon.ObjectBinding.Web.UnitTests.Domain;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls.Infrastructure.BocList
{

[TestFixture]
public class EditModeControllerInEditDetailsModeTest : EditModeControllerTestBase
{
  // types

  // static members and constants

  // member fields


  // construction and disposing

  public EditModeControllerInEditDetailsModeTest ()
  {
  }


  // methods and properties

  [Test]
  public void SwitchRowIntoEditMode ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    Assert.AreEqual (1, Controller.Controls.Count);
    Assert.IsTrue (Controller.Controls[0] is ModifiableRow);

    ModifiableRow row = (ModifiableRow) Controller.Controls[0];
    Assert.AreEqual ("Controller_Row0", row.ID);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Cannot initialize edit details mode: The BocList 'BocList' does not have a Value.")]
  public void SwitchRowIntoEditModeWithValueNull ()
  {
    Invoker.InitRecursive();
    BocList.LoadUnboundValue (null, false);
    Controller.SwitchRowIntoEditMode (0, Columns, Columns);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void SwitchRowIntoEditModeWithIndexToHigh ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (5, Columns, Columns);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void SwitchRowIntoEditModeWithIndexToLow ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (-1, Columns, Columns);
  }

  [Test]
  [Ignore ("TODO: Implement Test")]
  public void SwitchRowIntoEditModeWhileEditDetailsModeIsActiveOnOtherRow ()
  {
  }
  
  [Test]
  [Ignore ("TODO: Implement Test")]
  public void SwitchRowIntoEditModeWhileEditDetailsModeIsActiveOnThisRow ()
  {
  }
  
  [Test]
  [Ignore ("TODO: Implement Test")]
  public void SwitchRowIntoEditModeWhileEditDetailsModeIsActiveWithInvalidValues ()
  {
  }
  
  [Test]
  [Ignore ("TODO: Implement Test")]
  public void SwitchRowIntoEditModeWhileListEditModeIsActive ()
  {
  }

  
  [Test]
  public void AddAndEditRow ()
  {
    Invoker.InitRecursive();
    Assert.IsTrue (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (5, Controller.ModifiableRowIndex.Value);
    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    
    Assert.AreEqual (1, Controller.Controls.Count);
    Assert.IsTrue (Controller.Controls[0] is ModifiableRow);

    ModifiableRow row = (ModifiableRow) Controller.Controls[0];
    Assert.AreEqual ("Controller_Row0", row.ID);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  [Ignore ("TODO: Implement Test")]
  public void AddAndEditRowWhileEditDetailsModeIsActive ()
  {
  }
  
  [Test]
  [Ignore ("TODO: Implement Test")]
  public void AddAndEditRowWhileEditDetailsModeIsActiveWithInvalidValues ()
  {
  }
  
  [Test]
  [Ignore ("TODO: Implement Test")]
  public void AddAndEditRowWhileListEditModeIsActive ()
  {
  }

  
  [Test]
  public void EndEditDetailsModeAndSaveChangesWithValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "300");
    Controller.EndEditDetailsMode (true, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    Assert.IsTrue (Controller.ModifiableRowIndex.IsNull);

    CheckValues (Values[2], "New Value C", 300);
  }

  [Test]
  public void EndEditDetailsModeAndDiscardChangesWithValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesCancelingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "300");
    Controller.EndEditDetailsMode (false, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    Assert.IsTrue (Controller.ModifiableRowIndex.IsNull);

    CheckValues (Values[2], "C", 3);
  }

  [Test]
  public void EndEditDetailsModeWithNewRowAndSaveChangesWithValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (5, NewValues[0]));
    expectedEvents.Add (FormatChangesSavedEventMessage (5, NewValues[0]));

    Invoker.InitRecursive();    
    Controller.AddAndEditRow (NewValues[0], Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (5, Controller.ModifiableRowIndex.Value);

    SetValues ((ModifiableRow) Controller.Controls[0], "New Value F", "600");
    Controller.EndEditDetailsMode (true, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    Assert.IsTrue (Controller.ModifiableRowIndex.IsNull);

    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    CheckValues (NewValues[0], "New Value F", 600);
  }

  [Test]
  public void EndEditDetailsModeWithNewRowAndDiscardChangesWithValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesCancelingEventMessage (5, NewValues[0]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (-1, NewValues[0]));

    Invoker.InitRecursive();
    Controller.AddAndEditRow (NewValues[0], Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (5, Controller.ModifiableRowIndex.Value);

    SetValues ((ModifiableRow) Controller.Controls[0], "New Value F", "600");
    Controller.EndEditDetailsMode (false, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    Assert.IsTrue (Controller.ModifiableRowIndex.IsNull);

    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);
    CheckValues (NewValues[0], "F", 6);
  }

  [Test]
  public void EndEditDetailsModeAndSaveChangesWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "");
    Controller.EndEditDetailsMode (true, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsTrue(Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);

    CheckValues (Values[2], "C", 3);
  }

  [Test]
  public void EndEditDetailsModeAndDiscardChangesWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesCancelingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "");
    Controller.EndEditDetailsMode (false, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    Assert.IsTrue (Controller.ModifiableRowIndex.IsNull);

    CheckValues (Values[2], "C", 3);
  }


  [Test]
  public void EnsureEditModeRestored ()
  {
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    ControllerInvoker.LoadViewState (CreateViewState (null, false, 2, false));
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    
    Controller.EnsureEditModeRestored (Columns);
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Cannot restore edit details mode: The Value collection of the BocList 'BocList' no longer contains the previously modified row.")]
  public void EnsureEditModeRestoredWithInvalidRowIndex ()
  {
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    ControllerInvoker.LoadViewState (CreateViewState (null, false, 6, false));
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
 
    Controller.EnsureEditModeRestored (Columns);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Cannot restore edit mode: The BocList 'BocList' does not have a Value.")]
  public void EnsureEditModeRestoredWithValueNull ()
  {
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    ControllerInvoker.LoadViewState (CreateViewState (null, false, 6, false));
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Controller.OwnerControl.LoadUnboundValue (null, false);

    Controller.EnsureEditModeRestored (Columns);
  }

  [Test]
  public void AddRows ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Controller.AddRows (NewValues, Columns, Columns);
    
    Assert.AreEqual (7, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    Assert.AreSame (NewValues[1], Controller.OwnerControl.Value[6]);
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  public void AddRow ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Assert.AreEqual (5, Controller.AddRow (NewValues[0], Columns, Columns));
    
    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);

    Assert.AreEqual (0, ActualEvents.Count);
  }


  [Test]
  [ExpectedException (typeof (InvalidOperationException),
      "Cannot remove rows while the BocList 'BocList' is in edit details mode. Call EndEditDetailsMode() before removing the rows.")]
  public void RemoveRows ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Controller.RemoveRows (new IBusinessObject[] {Values[2]});
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException),
      "Cannot remove a row while the BocList 'BocList' is in edit details mode. Call EndEditDetailsMode() before removing the row.")]
  public void RemoveRow ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Controller.RemoveRow (Values[2], Columns);
  }

  
  [Test]
  public void ValidateWithValidValues ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);

    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "300");

    Assert.IsTrue (Controller.Validate());
  }

  [Test]
  public void ValidateWithInvalidValues ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "");

    Assert.IsFalse (Controller.Validate());
  }

  
  [Test]
  public void IsRequired ()
  {
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    
    Assert.IsTrue (Controller.IsEditDetailsModeActive);

    Assert.IsFalse (Controller.IsRequired (0));
    Assert.IsTrue (Controller.IsRequired (1));
  }

  [Test]
  public void IsDirty ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    
    ModifiableRow row = (ModifiableRow) Controller.Controls[0];
    Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
        (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
    stringValueField.Value = "New Value";

    Assert.IsTrue (Controller.IsDirty());
  }

  [Test]
  public void GetTrackedIDs ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);

    string id = "NamingContainer_Controller_Row{0}_{1}_Boc_TextBox";
    string[] trackedIDs = new string[2];
    trackedIDs[0] = string.Format (id, 0, 0);
    trackedIDs[1] = string.Format (id, 0, 1);

    Assert.AreEqual (trackedIDs, Controller.GetTrackedClientIDs());
  }

}

}
