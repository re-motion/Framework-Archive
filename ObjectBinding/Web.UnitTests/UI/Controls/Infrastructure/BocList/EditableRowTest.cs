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
public class EditableRowTest : BocTest
{
  // types

  // static members and constants

  // member fields
  
  private Rubicon.ObjectBinding.Web.UI.Controls.BocList _bocList;
  private EditableRow _editableRow;

  private TypeWithAllDataTypes _value01;

  private ReflectionBusinessObjectClass _typeWithAllDataTypesClass;

  private BusinessObjectPropertyPath _typeWithAllDataTypesStringValuePath;
  private BusinessObjectPropertyPath _typeWithAllDataTypesInt32ValuePath;

  private BocSimpleColumnDefinition _typeWithAllDataTypesStringValueSimpleColumn;
  private BocSimpleColumnDefinition _typeWithAllDataTypesInt32ValueSimpleColumn;
  private BocCompoundColumnDefinition _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
  private BocCustomColumnDefinition _typeWithAllDataTypesStringValueCustomColumn;
  private BocCommandColumnDefinition _commandColumn;
  private BocEditDetailsColumnDefinition _editDetailsColumn;
  private BocDropDownMenuColumnDefinition _dropDownMenuColumn;

  // construction and disposing

  public EditableRowTest ()
  {
  }

  // methods and properties

  [SetUp]
  public override void SetUp ()
  {
    base.SetUp();

    _bocList = new Rubicon.ObjectBinding.Web.UI.Controls.BocList ();
    _bocList.ID = "BocList";
    NamingContainer.Controls.Add (_bocList);

    _editableRow = new EditableRow (_bocList);
    _editableRow.ID = "Row";
    NamingContainer.Controls.Add (_editableRow);

    _value01 = new TypeWithAllDataTypes();
    _value01.StringValue = "A";
    _value01.Int32Value = 1;

    _typeWithAllDataTypesClass = new ReflectionBusinessObjectClass (typeof (TypeWithAllDataTypes));

    _typeWithAllDataTypesStringValuePath = BusinessObjectPropertyPath.Parse (_typeWithAllDataTypesClass, "StringValue");
    _typeWithAllDataTypesInt32ValuePath = BusinessObjectPropertyPath.Parse (_typeWithAllDataTypesClass, "Int32Value");

    _typeWithAllDataTypesStringValueSimpleColumn = new BocSimpleColumnDefinition ();
    _typeWithAllDataTypesStringValueSimpleColumn.PropertyPath = _typeWithAllDataTypesStringValuePath;

    _typeWithAllDataTypesInt32ValueSimpleColumn = new BocSimpleColumnDefinition ();
    _typeWithAllDataTypesInt32ValueSimpleColumn.PropertyPath = _typeWithAllDataTypesInt32ValuePath;

    _typeWithAllDataTypesStringValueFirstValueCompoundColumn = new BocCompoundColumnDefinition();
    _typeWithAllDataTypesStringValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithAllDataTypesStringValuePath));
    _typeWithAllDataTypesStringValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithAllDataTypesStringValuePath));
    _typeWithAllDataTypesStringValueFirstValueCompoundColumn.FormatString = "{0}, {1}";

    _typeWithAllDataTypesStringValueCustomColumn = new BocCustomColumnDefinition ();
    _typeWithAllDataTypesStringValueCustomColumn.PropertyPath = _typeWithAllDataTypesStringValuePath;
    _typeWithAllDataTypesStringValueCustomColumn.IsSortable = true;

    _commandColumn = new BocCommandColumnDefinition ();
    _editDetailsColumn = new BocEditDetailsColumnDefinition ();
    _dropDownMenuColumn = new BocDropDownMenuColumnDefinition ();
  }

  
  [Test]
  public void Initialize ()
  {
    Assert.AreSame (_bocList, _editableRow.OwnerControl);
    Assert.IsNull (_editableRow.DataSourceFactory);
    Assert.IsNull (_editableRow.ControlFactory);
  }


  [Test]
  public void CreateControlsWithEmptyColumns ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_editableRow.HasEditControls());
    Assert.IsFalse (_editableRow.HasValidators());

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    _editableRow.CreateControls (new BocColumnDefinition[0], _value01);

    Assert.IsTrue (_editableRow.HasEditControls());
    Assert.IsTrue (_editableRow.HasValidators());

    Assert.IsNotNull (_editableRow.DataSourceFactory);
    Assert.IsNotNull (_editableRow.ControlFactory);

    IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
    Assert.IsNotNull (dataSource);
    Assert.AreSame (_value01, dataSource.BusinessObject);
  }

  [Test]
  public void CreateControlsWithColumns ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_editableRow.HasEditControls());
    Assert.IsFalse (_editableRow.HasValidators());
    Assert.IsFalse (_editableRow.HasEditControl (0));

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[7];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
    columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
    columns[3] = _commandColumn;
    columns[4] = _editDetailsColumn;
    columns[5] = _dropDownMenuColumn;
    columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _editableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_editableRow.HasEditControls());
    Assert.IsTrue (_editableRow.HasValidators());

    Assert.IsNotNull (_editableRow.DataSourceFactory);
    Assert.IsNotNull (_editableRow.ControlFactory);

    IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
    Assert.IsNotNull (dataSource);
    Assert.AreSame (_value01, dataSource.BusinessObject);

    Assert.IsTrue (_editableRow.HasEditControl (0));
    Assert.IsFalse (_editableRow.HasEditControl (1));
    Assert.IsFalse (_editableRow.HasEditControl (2));
    Assert.IsFalse (_editableRow.HasEditControl (3));
    Assert.IsFalse (_editableRow.HasEditControl (4));
    Assert.IsFalse (_editableRow.HasEditControl (5));
    Assert.IsTrue (_editableRow.HasEditControl (6));

    IBusinessObjectBoundEditableWebControl textBoxFirstValue = _editableRow.GetEditControl (0);
    Assert.IsTrue (textBoxFirstValue is BocTextValue);
    Assert.AreSame (dataSource, textBoxFirstValue.DataSource);
    Assert.AreSame (_typeWithAllDataTypesStringValuePath.LastProperty, textBoxFirstValue.Property);

    IBusinessObjectBoundEditableWebControl textBoxSecondValue = _editableRow.GetEditControl (6);
    Assert.IsTrue (textBoxSecondValue is BocTextValue);
    Assert.AreSame (dataSource, textBoxSecondValue.DataSource);
    Assert.AreSame (_typeWithAllDataTypesInt32ValuePath.LastProperty, textBoxSecondValue.Property);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "BocList 'BocList': DataSourceFactory has not been set prior to invoking CreateControls().")]
  public void CreateControlsDataSourceFactoryNull ()
  {
    Invoker.InitRecursive();
    _editableRow.ControlFactory = new EditableRowControlFactory();
    _editableRow.CreateControls (new BocColumnDefinition[0], _value01);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "BocList 'BocList': ControlFactory has not been set prior to invoking CreateControls().")]
  public void CreateControlsControlFactoryNull ()
  {
    Invoker.InitRecursive();
    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.CreateControls (new BocColumnDefinition[0], _value01);
  }

  [Test]
  public void EnsureValidators ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_editableRow.HasValidators());

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[7];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
    columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
    columns[3] = _commandColumn;
    columns[4] = _editDetailsColumn;
    columns[5] = _dropDownMenuColumn;
    columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_editableRow.HasValidators());

    Assert.IsTrue (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));
    Assert.IsFalse (_editableRow.HasValidators (2));
    Assert.IsFalse (_editableRow.HasValidators (3));
    Assert.IsFalse (_editableRow.HasValidators (4));
    Assert.IsFalse (_editableRow.HasValidators (5));
    Assert.IsTrue (_editableRow.HasValidators (6));

    ControlCollection validators0 = _editableRow.GetValidators (0);
    Assert.IsNotNull (validators0);
    Assert.AreEqual (0, validators0.Count);

    ControlCollection validators6 = _editableRow.GetValidators (6);
    Assert.IsNotNull (validators6);
    Assert.AreEqual (2, validators6.Count);
    Assert.IsTrue (validators6[0] is RequiredFieldValidator);
    Assert.IsTrue (validators6[1] is NumericValidator);
  }

  [Test]
  public void EnsureValidatorsWithoutCreateControls ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_editableRow.HasValidators());

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    _editableRow.EnsureValidatorsRestored();

    Assert.IsFalse (_editableRow.HasValidators());

    BocColumnDefinition[] columns = new BocColumnDefinition[7];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
    columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
    columns[3] = _commandColumn;
    columns[4] = _editDetailsColumn;
    columns[5] = _dropDownMenuColumn;
    columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_editableRow.HasValidators());

    Assert.IsTrue (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));
    Assert.IsFalse (_editableRow.HasValidators (2));
    Assert.IsFalse (_editableRow.HasValidators (3));
    Assert.IsFalse (_editableRow.HasValidators (4));
    Assert.IsFalse (_editableRow.HasValidators (5));
    Assert.IsTrue (_editableRow.HasValidators (6));
 
    ControlCollection validators0 = _editableRow.GetValidators (0);
    Assert.IsNotNull (validators0);
    Assert.AreEqual (0, validators0.Count);

    ControlCollection validators6 = _editableRow.GetValidators (6);
    Assert.IsNotNull (validators6);
    Assert.AreEqual (2, validators6.Count);
    Assert.IsTrue (validators6[0] is RequiredFieldValidator);
    Assert.IsTrue (validators6[1] is NumericValidator);
  }

  [Test]
  public void ControlInit ()
  {
    Assert.IsFalse (_editableRow.HasControls());
    Assert.IsFalse (_editableRow.HasEditControls());
    Assert.IsFalse (_editableRow.HasValidators());

    Invoker.InitRecursive ();

    Assert.IsFalse (_editableRow.HasControls());
    Assert.IsFalse (_editableRow.HasEditControls());
    Assert.IsFalse (_editableRow.HasValidators());
  }

  [Test]
  public void ControlLoad ()
  {
    Assert.IsFalse (_editableRow.HasControls());
    Assert.IsFalse (_editableRow.HasEditControls());
    Assert.IsFalse (_editableRow.HasValidators());

    Invoker.LoadRecursive ();

    Assert.IsFalse (_editableRow.HasControls());
    Assert.IsFalse (_editableRow.HasEditControls());
    Assert.IsFalse (_editableRow.HasValidators());
  }

  [Test]
  public void LoadValue ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _editableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
    dataSource.LoadValues (false);

    BocTextValue textBoxStringValue = (BocTextValue) _editableRow.GetEditControl (0);
    BocTextValue textBoxInt32Value = (BocTextValue) _editableRow.GetEditControl (1);

    Assert.AreEqual ("A", textBoxStringValue.Value);
    Assert.AreEqual (1, textBoxInt32Value.Value);
  }

  [Test]
  public void SaveValue ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _editableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
    dataSource.LoadValues (false);

    BocTextValue textBoxStringValue = (BocTextValue) _editableRow.GetEditControl (0);
    BocTextValue textBoxInt32Value = (BocTextValue) _editableRow.GetEditControl (1);

    Assert.AreEqual ("A", textBoxStringValue.Value);
    Assert.AreEqual (1, textBoxInt32Value.Value);

    textBoxStringValue.Value = "New Value A";
    textBoxInt32Value.Value = "100";

    dataSource.SaveValues (false);

    Assert.AreEqual ("New Value A", _value01.StringValue);
    Assert.AreEqual (100, _value01.Int32Value);
  }


  [Test]
  public void HasEditControl ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_editableRow.HasEditControls());
    Assert.IsFalse (_editableRow.HasEditControl (0));
    Assert.IsFalse (_editableRow.HasEditControl (1));

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_editableRow.HasEditControls());
    Assert.IsTrue (_editableRow.HasEditControl (0));
    Assert.IsFalse (_editableRow.HasEditControl (1));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void HasEditControlWithNegativeIndex ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    _editableRow.HasEditControl (-1);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void HasEditControlWithIndexOutOfPositiveRange ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    _editableRow.HasEditControl (3);
  }


  [Test]
  public void GetEditControl ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_editableRow.HasEditControls());
    Assert.IsFalse (_editableRow.HasEditControl (0));
    Assert.IsFalse (_editableRow.HasEditControl (0));

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_editableRow.HasEditControls());
    Assert.IsTrue (_editableRow.HasEditControl (0));
    Assert.IsFalse (_editableRow.HasEditControl (1));
    
    IBusinessObjectBoundEditableWebControl control = _editableRow.GetEditControl (0);
    Assert.IsNotNull (control);
    Assert.IsTrue (control is BocTextValue);

    Assert.IsNull (_editableRow.GetEditControl (1));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void GetEditControlWithNegativeIndex ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    _editableRow.HasEditControl (-1);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void GetEditControlWithIndexOutOfPositiveRange ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    _editableRow.HasEditControl (3);
  }


  [Test]
  public void HasValidators ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_editableRow.HasValidators());
    Assert.IsFalse (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_editableRow.HasValidators());
    Assert.IsTrue (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));

    _editableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_editableRow.HasValidators());
    Assert.IsTrue (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));
  }

  [Test]
  public void HasValidatorsWithoutCreateControls ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_editableRow.HasValidators());
    Assert.IsFalse (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    _editableRow.EnsureValidatorsRestored();

    Assert.IsFalse (_editableRow.HasValidators());
    Assert.IsFalse (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_editableRow.HasValidators());
    Assert.IsTrue (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));

    _editableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_editableRow.HasValidators());
    Assert.IsTrue (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void HasValidatorsWithNegativeIndex ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored();

    _editableRow.HasValidators (-1);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void HasValidatorsWithIndexOutOfPositiveRange ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored();

    _editableRow.HasValidators (3);
  }


  [Test]
  public void GetValidators ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_editableRow.HasValidators());
    Assert.IsTrue (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));

    ControlCollection validators = _editableRow.GetValidators (0);
    Assert.IsNotNull (validators);
    Assert.AreEqual (2, validators.Count);
    Assert.IsTrue (validators[0] is RequiredFieldValidator);
    Assert.IsTrue (validators[1] is NumericValidator);

    Assert.IsNull (_editableRow.GetValidators (1));
  }

  [Test]
  public void GetValidatorsWithoutCreateControls ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    _editableRow.EnsureValidatorsRestored();
    
    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_editableRow.HasValidators());
    Assert.IsTrue (_editableRow.HasValidators (0));
    Assert.IsFalse (_editableRow.HasValidators (1));

    ControlCollection validators = _editableRow.GetValidators (0);
    Assert.IsNotNull (validators);
    Assert.AreEqual (2, validators.Count);
    Assert.IsTrue (validators[0] is RequiredFieldValidator);
    Assert.IsTrue (validators[1] is NumericValidator);

    Assert.IsNull (_editableRow.GetValidators (1));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void GetValidatorsWithNegativeIndex ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored();

    _editableRow.GetValidators (-1);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void GetValidatorsWithIndexOutOfPositiveRange ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored();

    _editableRow.GetValidators (3);
  }
  

  [Test]
  public void IsRequired ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    Assert.IsFalse (_editableRow.IsRequired (0));
    Assert.IsTrue (_editableRow.IsRequired (1));
    Assert.IsFalse (_editableRow.IsRequired (2));
  }

  
  [Test]
  public void IsDirty ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
    dataSource.LoadValues (false);

    Assert.IsFalse (_editableRow.IsDirty ());

    BocTextValue textBoxStringValue = (BocTextValue) _editableRow.GetEditControl (0);
    textBoxStringValue.Value = "a";

    Assert.IsTrue (_editableRow.IsDirty());
  }

  [Test]
  public void GetTrackedIDs ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
    dataSource.LoadValues (false);

    string id = "NamingContainer_Row_{0}_Boc_TextBox";
    string[] trackedIDs = new string[2];
    trackedIDs[0] = string.Format (id, 0);
    trackedIDs[1] = string.Format (id, 1);

    Assert.AreEqual (trackedIDs, _editableRow.GetTrackedClientIDs());
  }


  [Test]
  public void ValidateWithValidValues ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored ();

    IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
    dataSource.LoadValues (false);
    
    SetValues (_editableRow, "A", "300");

    Assert.IsTrue (_editableRow.Validate());
  }

  [Test]
  public void ValidateWithInvalidValues ()
  {
    Invoker.InitRecursive();

    _editableRow.DataSourceFactory = new EditableRowDataSourceFactory();
    _editableRow.ControlFactory = new EditableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _editableRow.CreateControls (columns, _value01);
    _editableRow.EnsureValidatorsRestored ();

    IBusinessObjectReferenceDataSource dataSource = _editableRow.GetDataSource();
    dataSource.LoadValues (false);
    
    SetValues (_editableRow, "A", "");

    Assert.IsFalse (_editableRow.Validate());
  }


  private void SetValues (EditableRow row, string stringValue, string int32Value)
  {
    ArgumentUtility.CheckNotNull ("row", row);

    BocTextValue stringValueField = (BocTextValue) row.GetEditControl (0);
    stringValueField.TextBox.Text = stringValue;
    stringValueField.Text = stringValue;
    
    BocTextValue int32ValueField = (BocTextValue) row.GetEditControl (1);
    int32ValueField.TextBox.Text = int32Value;
    int32ValueField.Text = int32Value;
  }
}

}
