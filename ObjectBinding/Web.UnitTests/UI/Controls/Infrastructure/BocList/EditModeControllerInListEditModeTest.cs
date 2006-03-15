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
public class EditModeControllerInListEditModeTest : EditModeControllerTestBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public EditModeControllerInListEditModeTest ()
  {
  }

  // methods and properties

  [Test]
  public void SwitchListIntoEditMode ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);

    Assert.AreEqual (5, Controller.Controls.Count);
    string idFormat = "Controller_Row{0}";
    Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
    Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
    Assert.AreEqual (string.Format (idFormat, 2), Controller.Controls[2].ID);
    Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[3].ID);
    Assert.AreEqual (string.Format (idFormat, 4), Controller.Controls[4].ID);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  public void SwitchListIntoEditModeWithValueEmpty ()
  {
    Invoker.InitRecursive();
    BocList.LoadUnboundValue (new IBusinessObject[0], false);
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (0, Controller.Controls.Count);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Cannot initialize list edit mode: The BocList 'BocList' does not have a Value.")]
  public void SwitchListIntoEditModeWithValueNull ()
  {
    Invoker.InitRecursive();
    BocList.LoadUnboundValue (null, false);
    Controller.SwitchListIntoEditMode (Columns, Columns);
  }


  [Test]
  public void EndListEditModeAndSaveChanges ()
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
    Controller.EndListEditMode (true, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsListEditModeActive);

    CheckValues (Values[0], "New Value A", 100);
    CheckValues (Values[1], "New Value B", 200);
    CheckValues (Values[2], "New Value C", 300);
    CheckValues (Values[3], "New Value D", 400);
    CheckValues (Values[4], "New Value E", 500);
  }

  [Test]
  public void EndListEditModeAndDiscardChanges ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesCancelingEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesCancelingEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesCancelingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesCancelingEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesCancelingEventMessage (4, Values[4]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (4, Values[4]));

    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    
    SetValues ((ModifiableRow) Controller.Controls[0], "New Value A", "100");
    SetValues ((ModifiableRow) Controller.Controls[1], "New Value B", "200");
    SetValues ((ModifiableRow) Controller.Controls[2], "New Value C", "300");
    SetValues ((ModifiableRow) Controller.Controls[3], "New Value D", "400");
    SetValues ((ModifiableRow) Controller.Controls[4], "New Value E", "500");
    Controller.EndListEditMode (false, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsListEditModeActive);

    CheckValues (Values[0], "A", 1);
    CheckValues (Values[1], "B", 2);
    CheckValues (Values[2], "C", 3);
    CheckValues (Values[3], "D", 4);
    CheckValues (Values[4], "E", 5);
  }


  [Test]
  public void AddRows ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Controller.AddRows (NewValues, Columns, Columns);
    
    Assert.AreEqual (7, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    Assert.AreSame (NewValues[1], Controller.OwnerControl.Value[6]);

    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (7, Controller.Controls.Count);
    string idFormat = "Controller_Row{0}";
    Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
    Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
    Assert.AreEqual (string.Format (idFormat, 2), Controller.Controls[2].ID);
    Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[3].ID);
    Assert.AreEqual (string.Format (idFormat, 4), Controller.Controls[4].ID);
    Assert.AreEqual (string.Format (idFormat, 5), Controller.Controls[5].ID);
    Assert.AreEqual (string.Format (idFormat, 6), Controller.Controls[6].ID);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  public void AddRow ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Assert.AreEqual (5, Controller.AddRow (NewValues[0], Columns, Columns));
    
    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);

    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (6, Controller.Controls.Count);
    string idFormat = "Controller_Row{0}";
    Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
    Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
    Assert.AreEqual (string.Format (idFormat, 2), Controller.Controls[2].ID);
    Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[3].ID);
    Assert.AreEqual (string.Format (idFormat, 4), Controller.Controls[4].ID);
    Assert.AreEqual (string.Format (idFormat, 5), Controller.Controls[5].ID);

    Assert.AreEqual (0, ActualEvents.Count);
  }


  [Test]
  public void RemoveRow ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Controller.RemoveRow (Values[2], Columns);
  
    Assert.AreEqual (4, Controller.OwnerControl.Value.Count);
    Assert.AreSame (Values[0], Controller.OwnerControl.Value[0]);
    Assert.AreSame (Values[1], Controller.OwnerControl.Value[1]);
    Assert.AreSame (Values[3], Controller.OwnerControl.Value[2]);
    Assert.AreSame (Values[4], Controller.OwnerControl.Value[3]);

    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (4, Controller.Controls.Count);
    string idFormat = "Controller_Row{0}";
    Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
    Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
    Assert.AreEqual (string.Format (idFormat, 2), Controller.Controls[2].ID);
    Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[3].ID);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  public void RemoveRows ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
     
    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Controller.RemoveRow (Values[2], Columns);
  
    Assert.AreEqual (4, Controller.OwnerControl.Value.Count);
    Assert.AreSame (Values[0], Controller.OwnerControl.Value[0]);
    Assert.AreSame (Values[1], Controller.OwnerControl.Value[1]);
    Assert.AreSame (Values[3], Controller.OwnerControl.Value[2]);
    Assert.AreSame (Values[4], Controller.OwnerControl.Value[3]);

    Assert.IsTrue (Controller.IsListEditModeActive);
    Assert.AreEqual (4, Controller.Controls.Count);
    string idFormat = "Controller_Row{0}";
    Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
    Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
    Assert.AreEqual (string.Format (idFormat, 2), Controller.Controls[2].ID);
    Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[3].ID);

    Assert.AreEqual (0, ActualEvents.Count);
  }


  [Test]
  public void ValidateWithValidValues ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);

    SetValues ((ModifiableRow) Controller.Controls[0], "New Value A", "100");
    SetValues ((ModifiableRow) Controller.Controls[1], "New Value B", "200");
    SetValues ((ModifiableRow) Controller.Controls[2], "New Value C", "300");
    SetValues ((ModifiableRow) Controller.Controls[3], "New Value D", "400");
    SetValues ((ModifiableRow) Controller.Controls[4], "New Value E", "500");

    Assert.IsTrue (Controller.Validate());
  }

  [Test]
  public void ValidateWithInvalidValues ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);

    SetValues ((ModifiableRow) Controller.Controls[0], "New Value A", "");
    SetValues ((ModifiableRow) Controller.Controls[1], "New Value B", "");
    SetValues ((ModifiableRow) Controller.Controls[2], "New Value C", "");
    SetValues ((ModifiableRow) Controller.Controls[3], "New Value D", "");
    SetValues ((ModifiableRow) Controller.Controls[4], "New Value E", "");

    Assert.IsFalse (Controller.Validate());
  }


  [Test]
  public void IsRequired ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);
   
    Assert.IsTrue (Controller.IsListEditModeActive);

    Assert.IsFalse (Controller.IsRequired (0));
    Assert.IsTrue (Controller.IsRequired (1));
  }

  [Test]
  public void IsDirty ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);

    ModifiableRow row = (ModifiableRow) Controller.Controls[2];
    Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
        (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
    stringValueField.Value = "New Value";

    Assert.IsTrue (Controller.IsDirty());
  }

  [Test]
  public void GetTrackedIDs ()
  {
    Invoker.InitRecursive();
    Controller.SwitchListIntoEditMode (Columns, Columns);

    string id = "NamingContainer_Controller_Row{0}_{1}_Boc_TextBox";
    string[] trackedIDs = new string[10];
    for (int i = 0; i < 5; i++)
    {
      trackedIDs[2 * i] = string.Format (id, i, 0);
      trackedIDs[2 * i + 1] = string.Format (id, i, 1);
    }

    Assert.AreEqual (trackedIDs, Controller.GetTrackedClientIDs());
  }

}

}
