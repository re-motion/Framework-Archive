using System;
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
public class EditModeControllerTest : BocTest
{
  // types

  // static members and constants

  // member fields

  private Rubicon.ObjectBinding.Web.UI.Controls.BocList _bocList;
  private EditModeController _controller;
  private ControlInvoker _invoker;

  private TypeWithAllDataTypes[] _values;
  private TypeWithAllDataTypes[] _newValues;

  private ReflectionBusinessObjectClass _class;

  private BusinessObjectPropertyPath _stringValuePath;
  private BusinessObjectPropertyPath _int32ValuePath;

  private BocColumnDefinition[] _columns;
  private BocSimpleColumnDefinition _stringValueSimpleColumn;
  private BocSimpleColumnDefinition _int32ValueSimpleColumn;

  // construction and disposing

  public EditModeControllerTest ()
  {
  }

  // methods and properties

  [SetUp]
  public override void SetUp ()
  {
    base.SetUp();

    _values = new TypeWithAllDataTypes[5];
    _values[0] = new TypeWithAllDataTypes ("A", 0);
    _values[1] = new TypeWithAllDataTypes ("B", 1);
    _values[2] = new TypeWithAllDataTypes ("C", 2);
    _values[3] = new TypeWithAllDataTypes ("D", 3);
    _values[4] = new TypeWithAllDataTypes ("E", 4);

    _newValues = new TypeWithAllDataTypes[2];
    _newValues[0] = new TypeWithAllDataTypes ("G", 5);
    _newValues[1] = new TypeWithAllDataTypes ("H", 6);

    _class = new ReflectionBusinessObjectClass (typeof (TypeWithAllDataTypes));

    _stringValuePath = BusinessObjectPropertyPath.Parse (_class, "StringValue");
    _int32ValuePath = BusinessObjectPropertyPath.Parse (_class, "Int32Value");

    _stringValueSimpleColumn = new BocSimpleColumnDefinition ();
    _stringValueSimpleColumn.PropertyPath = _stringValuePath;

    _int32ValueSimpleColumn = new BocSimpleColumnDefinition ();
    _int32ValueSimpleColumn.PropertyPath = _int32ValuePath;

    _columns = new BocColumnDefinition[2];
    _columns[0] = _stringValueSimpleColumn;
    _columns[1] = _int32ValueSimpleColumn;

    _bocList = new Rubicon.ObjectBinding.Web.UI.Controls.BocList ();
    _bocList.ID = "BocList";
    NamingContainer.Controls.Add (_bocList);

    _controller = new EditModeController (_bocList);
    _controller.ID = "Controller";
    NamingContainer.Controls.Add (_controller);

    _invoker = new ControlInvoker (_controller);

    _bocList.FixedColumns.AddRange (_columns);
    _bocList.LoadUnboundValue (_values, false);
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreSame (_bocList, _controller.Owner);
    Assert.IsFalse (_controller.IsEditDetailsModeActive);
    Assert.IsFalse (_controller.IsListEditModeActive);
  }

  [Test]
  public void InitRecursive()
  {
    _invoker.InitRecursive();

    Assert.AreEqual (0, _controller.Controls.Count);
  }

  [Test]
  public void SwitchRowIntoEditMode ()
  {
    _invoker.InitRecursive();
    _controller.SwitchRowIntoEditMode (2, _columns, _columns);
     
    Assert.IsTrue (_controller.IsEditDetailsModeActive);
    Assert.AreEqual (2, _controller.ModifiableRowIndex.Value);
    
    Assert.AreEqual (1, _controller.Controls.Count);
    Assert.IsTrue (_controller.Controls[0] is ModifiableRow);

    ModifiableRow row = (ModifiableRow) _controller.Controls[0];
    Assert.AreEqual ("Controller_Row0", row.ID);
  }

  [Test]
  public void SwitchListIntoEditMode ()
  {
    _invoker.InitRecursive();
    _controller.SwitchListIntoEditMode (_columns, _columns);
     
    Assert.IsTrue (_controller.IsListEditModeActive);

    Assert.AreEqual (5, _controller.Controls.Count);
    for (int i = 0; i < _controller.Controls.Count; i++)
    {
      Assert.IsTrue (_controller.Controls[i] is ModifiableRow, "Row: {0}", i);
      ModifiableRow row = (ModifiableRow) _controller.Controls[i];
      Assert.AreEqual (string.Format ("Controller_Row{0}", i), row.ID);
    }
  }

  [Test]
  public void ValidateDuringEditDetailsModeWithValidValues ()
  {
    _invoker.InitRecursive();
    _controller.SwitchRowIntoEditMode (2, _columns, _columns);
    
    ModifiableRow row = (ModifiableRow) _controller.Controls[0];
    
    Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
        (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
    stringValueField.TextBox.Text = "New Value";
    
    Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue int32ValueField = 
        (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (1);
    int32ValueField.TextBox.Text = "1";

    Assert.IsTrue (_controller.Validate());
  }

  [Test]
  public void ValidateDuringEditDetailsModeWithInvalidValues ()
  {
    _invoker.InitRecursive();
    _controller.SwitchRowIntoEditMode (2, _columns, _columns);
    
    ModifiableRow row = (ModifiableRow) _controller.Controls[0];
    
    Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
        (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
    stringValueField.TextBox.Text = "New Value";
    
    Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue int32ValueField = 
        (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (1);
    int32ValueField.TextBox.Text = "";

    Assert.IsFalse (_controller.Validate());
  }

  [Test]
  public void ValidateDuringListEditModeWithValidValues ()
  {
    _invoker.InitRecursive();
    _controller.SwitchListIntoEditMode (_columns, _columns);

    for (int i = 0; i < _controller.Controls.Count; i++)
    {
      ModifiableRow row = (ModifiableRow) _controller.Controls[i];
      
      Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
          (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
      stringValueField.TextBox.Text = "New Value";
      
      Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue int32ValueField = 
          (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (1);
      int32ValueField.TextBox.Text = i.ToString();
    }

    Assert.IsTrue (_controller.Validate());
  }

  [Test]
  public void ValidateDuringListEditModeWithInvalidValues ()
  {
    _invoker.InitRecursive();
    _controller.SwitchListIntoEditMode (_columns, _columns);

    for (int i = 0; i < _controller.Controls.Count; i++)
    {
      ModifiableRow row = (ModifiableRow) _controller.Controls[2];
      
      Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
          (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
      stringValueField.TextBox.Text = "New Value";
      
      Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue int32ValueField = 
          (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (1);
      int32ValueField.TextBox.Text = "";
    }

    Assert.IsFalse (_controller.Validate());
  }

  [Test]
  public void ValidateWithoutEditMode ()
  {
    _invoker.InitRecursive();
    _invoker.LoadRecursive();

    Assert.IsTrue (_controller.Validate());
  }

  [Test]
  public void IsRequiredDuringEditDetailsMode ()
  {
    _controller.SwitchRowIntoEditMode (2, _columns, _columns);
    
    Assert.IsTrue (_controller.IsEditDetailsModeActive);

    Assert.IsFalse (_controller.IsRequired (0));
    Assert.IsTrue (_controller.IsRequired (1));
  }

  [Test]
  public void IsRequiredDuringListEditMode ()
  {
    _invoker.InitRecursive();
    _controller.SwitchListIntoEditMode (_columns, _columns);
   
    Assert.IsTrue (_controller.IsListEditModeActive);

    Assert.IsFalse (_controller.IsRequired (0));
    Assert.IsTrue (_controller.IsRequired (1));
  }

  [Test]
  public void IsRequiredWithoutEditMode ()
  {
    _invoker.InitRecursive();
    Assert.IsFalse (_controller.IsRequired (0));
    Assert.IsFalse (_controller.IsRequired (1));
  }

  [Test]
  public void IsDirtyDuringEditDetailsMode ()
  {
    _invoker.InitRecursive();
    _controller.SwitchRowIntoEditMode (2, _columns, _columns);
    
    ModifiableRow row = (ModifiableRow) _controller.Controls[0];
    Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
        (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
    stringValueField.Value = "New Value";

    Assert.IsTrue (_controller.IsDirty());
  }

  [Test]
  public void IsDirtyDuringListEditMode ()
  {
    _invoker.InitRecursive();
    _controller.SwitchListIntoEditMode (_columns, _columns);

    ModifiableRow row = (ModifiableRow) _controller.Controls[2];
    Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
        (Rubicon.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
    stringValueField.Value = "New Value";

    Assert.IsTrue (_controller.IsDirty());
  }

  [Test]
  public void IsDirtyWithoutEditMode ()
  {
    _invoker.InitRecursive();
    Assert.IsFalse (_controller.IsDirty());
  }

  [Test]
  public void GetTrackedIDsDuringEditDetailsMode ()
  {
    _invoker.InitRecursive();
    _controller.SwitchRowIntoEditMode (2, _columns, _columns);

    string id = "NamingContainer_Controller_Row{0}_{1}_Boc_TextBox";
    string[] trackedIDs = new string[2];
    trackedIDs[0] = string.Format (id, 0, 0);
    trackedIDs[1] = string.Format (id, 0, 1);

    Assert.AreEqual (trackedIDs, _controller.GetTrackedClientIDs());
  }

  [Test]
  public void GetTrackedIDsDuringListEditMode ()
  {
    _invoker.InitRecursive();
    _controller.SwitchListIntoEditMode (_columns, _columns);

    string id = "NamingContainer_Controller_Row{0}_{1}_Boc_TextBox";
    string[] trackedIDs = new string[10];
    for (int i = 0; i < 5; i++)
    {
      trackedIDs[2 * i] = string.Format (id, i, 0);
      trackedIDs[2 * i + 1] = string.Format (id, i, 1);
    }

    Assert.AreEqual (trackedIDs, _controller.GetTrackedClientIDs());
  }

  [Test]
  public void GetTrackedIDsWithoutEditMode ()
  {
    _invoker.InitRecursive();

    Assert.AreEqual (new string[0], _controller.GetTrackedClientIDs());
  }
}

}
