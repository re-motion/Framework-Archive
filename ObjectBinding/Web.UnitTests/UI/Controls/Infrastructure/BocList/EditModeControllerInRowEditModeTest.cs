using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.Globalization;
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


  [Test]
  public void GetFactoriesFromOwnerControl ()
  {
    Controller.OwnerControl.EditModeDataSourceFactory = new ModifiableRowDataSourceFactory();
    Controller.OwnerControl.EditModeControlFactory = new ModifiableRowControlFactory();

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (0, Columns, Columns);
    ModifiableRow row = (ModifiableRow) Controller.Controls[0];

    Assert.AreSame (Controller.OwnerControl.EditModeDataSourceFactory, row.DataSourceFactory);
    Assert.AreSame (Controller.OwnerControl.EditModeControlFactory, row.ControlFactory);
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
  public void SwitchRowIntoEditModeWhileEditDetailsModeIsActiveOnOtherRowWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesSavedEventMessage (1, Values[1]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (1, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (1, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value B", "200");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    CheckValues (Values[1], "New Value B", 200);
  }

  [Test]
  public void SwitchRowIntoEditModeWhileEditDetailsModeIsActiveOnOtherRowWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (1, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (1, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value B", "");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (1, Controller.ModifiableRowIndex.Value);
    
    CheckValues (Values[1], "B", 2);
  }
  
  [Test]
  public void SwitchRowIntoEditModeWhileEditDetailsModeIsActiveOnThisRowWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "300");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    CheckValues (Values[2], "New Value C", 300);
  }

  [Test]
  public void SwitchRowIntoEditModeWhileEditDetailsModeIsActiveOnThisRowWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    CheckValues (Values[2], "C", 3);
  }
    
  [Test]
  public void SwitchRowIntoEditModeWhileListEditModeIsActiveWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavingEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesSavingEventMessage (4, Values[4]));
    expectedEvents.Add (FormatChangesSavedEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesSavedEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavedEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesSavedEventMessage (4, Values[4]));

    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value A", "100");
    SetValues ((ModifiableRow) Controller.Controls[1], "New Value B", "200");
    SetValues ((ModifiableRow) Controller.Controls[2], "New Value C", "300");
    SetValues ((ModifiableRow) Controller.Controls[3], "New Value D", "400");
    SetValues ((ModifiableRow) Controller.Controls[4], "New Value E", "500");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    CheckValues (Values[0], "New Value A", 100);
    CheckValues (Values[1], "New Value B", 200);
    CheckValues (Values[2], "New Value C", 300);
    CheckValues (Values[3], "New Value D", 400);
    CheckValues (Values[4], "New Value E", 500);
  }

  [Test]
  public void SwitchRowIntoEditModeWhileListEditModeIsActiveWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavingEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesSavingEventMessage (4, Values[4]));

    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value A", "");
    SetValues ((ModifiableRow) Controller.Controls[1], "New Value B", "");
    SetValues ((ModifiableRow) Controller.Controls[2], "New Value C", "");
    SetValues ((ModifiableRow) Controller.Controls[3], "New Value D", "");
    SetValues ((ModifiableRow) Controller.Controls[4], "New Value E", "");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);
    
    CheckValues (Values[0], "A", 1);
    CheckValues (Values[1], "B", 2);
    CheckValues (Values[2], "C", 3);
    CheckValues (Values[3], "D", 4);
    CheckValues (Values[4], "E", 5);
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
  public void AddAndEditRowWhileEditDetailsModeIsActiveWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "300");

    Assert.IsTrue (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (5, Controller.ModifiableRowIndex.Value);
    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    
    CheckValues (Values[2], "New Value C", 300);
  }
  
  [Test]
  public void AddAndEditRowWhileEditDetailsModeIsActiveWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value C", "");

    Assert.IsFalse (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);
    
    CheckValues (Values[2], "C", 3);
  }
  
  [Test]
  public void AddAndEditRowWhileListEditModeIsActiveWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavingEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesSavingEventMessage (4, Values[4]));
    expectedEvents.Add (FormatChangesSavedEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesSavedEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavedEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesSavedEventMessage (4, Values[4]));

    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value A", "100");
    SetValues ((ModifiableRow) Controller.Controls[1], "New Value B", "200");
    SetValues ((ModifiableRow) Controller.Controls[2], "New Value C", "300");
    SetValues ((ModifiableRow) Controller.Controls[3], "New Value D", "400");
    SetValues ((ModifiableRow) Controller.Controls[4], "New Value E", "500");

    Assert.IsTrue (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (5, Controller.ModifiableRowIndex.Value);
    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    
    CheckValues (Values[0], "New Value A", 100);
    CheckValues (Values[1], "New Value B", 200);
    CheckValues (Values[2], "New Value C", 300);
    CheckValues (Values[3], "New Value D", 400);
    CheckValues (Values[4], "New Value E", 500);
  }
  
  [Test]
  public void AddAndEditRowWhileListEditModeIsActiveWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavingEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesSavingEventMessage (4, Values[4]));

    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value A", "");
    SetValues ((ModifiableRow) Controller.Controls[1], "New Value B", "");
    SetValues ((ModifiableRow) Controller.Controls[2], "New Value C", "");
    SetValues ((ModifiableRow) Controller.Controls[3], "New Value D", "");
    SetValues ((ModifiableRow) Controller.Controls[4], "New Value E", "");

    Assert.IsFalse (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);
    
    CheckValues (Values[0], "A", 1);
    CheckValues (Values[1], "B", 2);
    CheckValues (Values[2], "C", 3);
    CheckValues (Values[3], "D", 4);
    CheckValues (Values[4], "E", 5);
  }

  
  [Test]
  public void EndEditDetailsModeAndSaveChangesWithValidValues ()
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
  public void EndEditDetailsModeAndDiscardChangesWithValidValues ()
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
  public void EndEditDetailsModeWithNewRowAndSaveChangesWithValidValues ()
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
  public void EndEditDetailsModeWithNewRowAndDiscardChangesWithValidValues ()
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
  public void EndEditDetailsModeWithoutBeingActive ()
  {
    Invoker.InitRecursive();
     
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    
    Controller.EndEditDetailsMode (true, Columns);

    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  public void EndEditDetailsModeWithoutBeingActiveAndValueNull ()
  {
    Invoker.InitRecursive();
    Controller.OwnerControl.LoadUnboundValue (null, false);
     
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    
    Controller.EndEditDetailsMode (true, Columns);

    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (0, ActualEvents.Count);
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
  public void CreateValidators ()
  {
    IResourceManager resourceManager = (IResourceManager) NullResourceManager.Instance;

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);

    BaseValidator[] validators = Controller.CreateValidators (resourceManager);
    
    Assert.IsNotNull (validators);
    Assert.AreEqual (1, validators.Length);
    Assert.IsTrue (validators[0] is EditDetailsValidator);
    Assert.AreEqual (resourceManager.GetString (BocList.ResourceIdentifier.EditDetailsErrorMessage), validators[0].ErrorMessage);
  }

  [Test]
  public void CreateValidatorsWithErrorMessageFromOwnerControl ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    Controller.OwnerControl.ErrorMessage = "Foo Bar";
    
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);

    BaseValidator[] validators = Controller.CreateValidators (NullResourceManager.Instance);
    
    Assert.IsNotNull (validators);
    Assert.AreEqual (1, validators.Length);
    Assert.IsTrue (validators[0] is EditDetailsValidator);
    Assert.AreEqual ("Foo Bar", validators[0].ErrorMessage);
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


  [Test]
  public void SaveAndLoadViewState ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);

    object viewState = ControllerInvoker.SaveViewState();
    Assert.IsNotNull (viewState);

    Controller.EndEditDetailsMode (false, Columns);
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    Assert.IsTrue (Controller.ModifiableRowIndex.IsNull);

    ControllerInvoker.LoadViewState (viewState);
    Assert.IsTrue (Controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, Controller.ModifiableRowIndex.Value);
  }
}

}
