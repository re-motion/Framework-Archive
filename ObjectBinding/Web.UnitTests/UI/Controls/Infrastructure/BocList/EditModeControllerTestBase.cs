using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.Development.UnitTesting;
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

public class EditModeControllerTestBase : BocTest
{
  // types

  // static members and constants

  // member fields

  private StringCollectionChecker _stringCollectionChecker;
  private StringCollection _actualEvents;

  private Rubicon.ObjectBinding.Web.UI.Controls.BocList _bocList;
  private EditModeController _controller;
  private ControlInvoker _controllerInvoker;

  private TypeWithAllDataTypes[] _values;
  private TypeWithAllDataTypes[] _newValues;

  private ReflectionBusinessObjectClass _class;

  private BusinessObjectPropertyPath _stringValuePath;
  private BusinessObjectPropertyPath _int32ValuePath;

  private BocColumnDefinition[] _columns;

  private BocSimpleColumnDefinition _stringValueSimpleColumn;
  private BocSimpleColumnDefinition _int32ValueSimpleColumn;

  // construction and disposing

  public EditModeControllerTestBase ()
  {
  }

  // methods and properties

  [SetUp]
  public override void SetUp ()
  {
    base.SetUp();

    _stringCollectionChecker = new StringCollectionChecker();
    _actualEvents = new StringCollection();

    _values = new TypeWithAllDataTypes[5];
    _values[0] = new TypeWithAllDataTypes ("A", 1);
    _values[1] = new TypeWithAllDataTypes ("B", 2);
    _values[2] = new TypeWithAllDataTypes ("C", 3);
    _values[3] = new TypeWithAllDataTypes ("D", 4);
    _values[4] = new TypeWithAllDataTypes ("E", 5);

    _newValues = new TypeWithAllDataTypes[2];
    _newValues[0] = new TypeWithAllDataTypes ("F", 6);
    _newValues[1] = new TypeWithAllDataTypes ("G", 7);

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
    
    _controllerInvoker = new ControlInvoker (_controller);

    _bocList.EditableRowChangesCanceled += new BocListItemEventHandler (Boclist_EditableRowChangesCanceled);
    _bocList.EditableRowChangesCanceling += new BocListEditableRowChangesEventHandler (Boclist_EditableRowChangesCanceling);
    _bocList.EditableRowChangesSaved += new BocListItemEventHandler (Boclist_EditableRowChangesSaved);
    _bocList.EditableRowChangesSaving += new BocListEditableRowChangesEventHandler (Boclist_EditableRowChangesSaving);

    _bocList.FixedColumns.AddRange (_columns);
    _bocList.LoadUnboundValue (_values, false);
  }

  protected StringCollection ActualEvents
  {
    get { return _actualEvents; }
  }

  protected Rubicon.ObjectBinding.Web.UI.Controls.BocList BocList
  {
    get { return _bocList; }
  }

  protected EditModeController Controller
  {
    get { return _controller; }
  }

  public ControlInvoker ControllerInvoker
  {
    get { return _controllerInvoker; }
  }

  protected TypeWithAllDataTypes[] Values
  {
    get { return _values; }
  }

  protected TypeWithAllDataTypes[] NewValues
  {
    get { return _newValues; }
  }

  protected BocColumnDefinition[] Columns
  {
    get { return _columns; }
  }

  protected void SetValues (EditableRow row, string stringValue, string int32Value)
  {
    ArgumentUtility.CheckNotNull ("row", row);

    BocTextValue stringValueField = (BocTextValue) row.GetEditControl (0);
    stringValueField.TextBox.Text = stringValue;
    stringValueField.Text = stringValue;
    
    BocTextValue int32ValueField = (BocTextValue) row.GetEditControl (1);
    int32ValueField.TextBox.Text = int32Value;
    int32ValueField.Text = int32Value;
  }

  protected void CheckValues (TypeWithAllDataTypes value, string stringValue, int int32Value)
  {
    ArgumentUtility.CheckNotNull ("value", value);

    Assert.AreEqual (stringValue, value.StringValue);
    Assert.AreEqual (int32Value, value.Int32Value);
  }

  protected void CheckEvents (StringCollection expected, StringCollection actual)
  {
    _stringCollectionChecker.AreEqual (expected, actual);
  }

  protected string FormatChangesCanceledEventMessage (int index, IBusinessObject businessObject)
  {
    return FormatEventMessage ("ChangesCanceled", index, businessObject);
  }

  protected string FormatChangesCancelingEventMessage (int index, IBusinessObject businessObject)
  {
    return FormatEventMessage ("ChangesCanceling", index, businessObject);
  }

  protected string FormatChangesSavedEventMessage (int index, IBusinessObject businessObject)
  {
    return FormatEventMessage ("ChangesSaved", index, businessObject);
  }

  protected string FormatChangesSavingEventMessage (int index, IBusinessObject businessObject)
  {
    return FormatEventMessage ("ChangesSaving", index, businessObject);
  }

  private string FormatEventMessage (string eventName, int index, IBusinessObject businessObject)
  {
    return string.Format ("{0}: {1}, {2}", eventName, index, businessObject.ToString());
  }

  private void Boclist_EditableRowChangesCanceled (object sender, BocListItemEventArgs e)
  {
    _actualEvents.Add (FormatChangesCanceledEventMessage (e.ListIndex, e.BusinessObject));
  }

  private void Boclist_EditableRowChangesCanceling (object sender, BocListEditableRowChangesEventArgs e)
  {
    _actualEvents.Add (FormatChangesCancelingEventMessage (e.ListIndex, e.BusinessObject));
  }

  private void Boclist_EditableRowChangesSaved (object sender, BocListItemEventArgs e)
  {
    _actualEvents.Add (FormatChangesSavedEventMessage (e.ListIndex, e.BusinessObject));
  }

  private void Boclist_EditableRowChangesSaving (object sender, BocListEditableRowChangesEventArgs e)
  {
    _actualEvents.Add (FormatChangesSavingEventMessage (e.ListIndex, e.BusinessObject));
  }

  protected object CreateViewState (
      object baseViewState,
      bool isListEditModeActive, NaInt32 editableRowIndex, bool isEditNewRow, EditableRowIDProvider rowIDProvider)
  {
    object[] values = new object[5];

    values[0] = baseViewState;
    values[1] = isListEditModeActive;
    values[2] = editableRowIndex;
    values[3] = isEditNewRow;
    values[4] = rowIDProvider;

    return values;
  }
}

}
