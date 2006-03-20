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
public class ModifiableRowTest : BocTest
{
  // types

  // static members and constants

  // member fields
  
  private Rubicon.ObjectBinding.Web.UI.Controls.BocList _bocList;
  private ModifiableRow _modifiableRow;

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

  public ModifiableRowTest ()
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

    _modifiableRow = new ModifiableRow (_bocList);
    _modifiableRow.ID = "Row";
    NamingContainer.Controls.Add (_modifiableRow);

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
    Assert.AreSame (_bocList, _modifiableRow.OwnerControl);
    Assert.IsNull (_modifiableRow.DataSourceFactory);
    Assert.IsNull (_modifiableRow.ControlFactory);
  }


  [Test]
  public void CreateControlsWithEmptyColumns ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    _modifiableRow.CreateControls (new BocColumnDefinition[0], _value01);

    Assert.IsTrue (_modifiableRow.HasEditControls());
    Assert.IsTrue (_modifiableRow.HasValidators());

    Assert.IsNotNull (_modifiableRow.DataSourceFactory);
    Assert.IsNotNull (_modifiableRow.ControlFactory);

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    Assert.IsNotNull (dataSource);
    Assert.AreSame (_value01, dataSource.BusinessObject);
  }

  [Test]
  public void CreateControlsWithColumns ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());
    Assert.IsFalse (_modifiableRow.HasEditControl (0));

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[7];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
    columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
    columns[3] = _commandColumn;
    columns[4] = _editDetailsColumn;
    columns[5] = _dropDownMenuColumn;
    columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _modifiableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_modifiableRow.HasEditControls());
    Assert.IsTrue (_modifiableRow.HasValidators());

    Assert.IsNotNull (_modifiableRow.DataSourceFactory);
    Assert.IsNotNull (_modifiableRow.ControlFactory);

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    Assert.IsNotNull (dataSource);
    Assert.AreSame (_value01, dataSource.BusinessObject);

    Assert.IsTrue (_modifiableRow.HasEditControl (0));
    Assert.IsFalse (_modifiableRow.HasEditControl (1));
    Assert.IsFalse (_modifiableRow.HasEditControl (2));
    Assert.IsFalse (_modifiableRow.HasEditControl (3));
    Assert.IsFalse (_modifiableRow.HasEditControl (4));
    Assert.IsFalse (_modifiableRow.HasEditControl (5));
    Assert.IsTrue (_modifiableRow.HasEditControl (6));

    IBusinessObjectBoundModifiableWebControl textBoxFirstValue = _modifiableRow.GetEditControl (0);
    Assert.IsTrue (textBoxFirstValue is BocTextValue);
    Assert.AreSame (dataSource, textBoxFirstValue.DataSource);
    Assert.AreSame (_typeWithAllDataTypesStringValuePath.LastProperty, textBoxFirstValue.Property);

    IBusinessObjectBoundModifiableWebControl textBoxSecondValue = _modifiableRow.GetEditControl (6);
    Assert.IsTrue (textBoxSecondValue is BocTextValue);
    Assert.AreSame (dataSource, textBoxSecondValue.DataSource);
    Assert.AreSame (_typeWithAllDataTypesInt32ValuePath.LastProperty, textBoxSecondValue.Property);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "BocList 'BocList': No ModifiableRowDataSourceFactory has been assigned to the ModifiableRow prior to invoking CreateControls().")]
  public void CreateControlsDataSourceFactoryNull ()
  {
    Invoker.InitRecursive();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();
    _modifiableRow.CreateControls (new BocColumnDefinition[0], _value01);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "BocList 'BocList': No ModifiableRowControlFactory has been assigned to the ModifiableRow prior to invoking CreateControls().")]
  public void CreateControlsControlFactoryNull ()
  {
    Invoker.InitRecursive();
    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.CreateControls (new BocColumnDefinition[0], _value01);
  }

  [Test]
  public void EnsureValidators ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_modifiableRow.HasValidators());

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[7];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
    columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
    columns[3] = _commandColumn;
    columns[4] = _editDetailsColumn;
    columns[5] = _dropDownMenuColumn;
    columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_modifiableRow.HasValidators());

    Assert.IsTrue (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));
    Assert.IsFalse (_modifiableRow.HasValidators (2));
    Assert.IsFalse (_modifiableRow.HasValidators (3));
    Assert.IsFalse (_modifiableRow.HasValidators (4));
    Assert.IsFalse (_modifiableRow.HasValidators (5));
    Assert.IsTrue (_modifiableRow.HasValidators (6));

    ControlCollection validators0 = _modifiableRow.GetValidators (0);
    Assert.IsNotNull (validators0);
    Assert.AreEqual (0, validators0.Count);

    ControlCollection validators6 = _modifiableRow.GetValidators (6);
    Assert.IsNotNull (validators6);
    Assert.AreEqual (2, validators6.Count);
    Assert.IsTrue (validators6[0] is RequiredFieldValidator);
    Assert.IsTrue (validators6[1] is NumericValidator);
  }

  [Test]
  public void EnsureValidatorsWithoutCreateControls ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_modifiableRow.HasValidators());

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    _modifiableRow.EnsureValidatorsRestored();

    Assert.IsFalse (_modifiableRow.HasValidators());

    BocColumnDefinition[] columns = new BocColumnDefinition[7];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesStringValueFirstValueCompoundColumn;
    columns[2] = _typeWithAllDataTypesStringValueCustomColumn;
    columns[3] = _commandColumn;
    columns[4] = _editDetailsColumn;
    columns[5] = _dropDownMenuColumn;
    columns[6] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_modifiableRow.HasValidators());

    Assert.IsTrue (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));
    Assert.IsFalse (_modifiableRow.HasValidators (2));
    Assert.IsFalse (_modifiableRow.HasValidators (3));
    Assert.IsFalse (_modifiableRow.HasValidators (4));
    Assert.IsFalse (_modifiableRow.HasValidators (5));
    Assert.IsTrue (_modifiableRow.HasValidators (6));
 
    ControlCollection validators0 = _modifiableRow.GetValidators (0);
    Assert.IsNotNull (validators0);
    Assert.AreEqual (0, validators0.Count);

    ControlCollection validators6 = _modifiableRow.GetValidators (6);
    Assert.IsNotNull (validators6);
    Assert.AreEqual (2, validators6.Count);
    Assert.IsTrue (validators6[0] is RequiredFieldValidator);
    Assert.IsTrue (validators6[1] is NumericValidator);
  }

  [Test]
  public void ControlInit ()
  {
    Assert.IsFalse (_modifiableRow.HasControls());
    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());

    Invoker.InitRecursive ();

    Assert.IsFalse (_modifiableRow.HasControls());
    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());
  }

  [Test]
  public void ControlLoad ()
  {
    Assert.IsFalse (_modifiableRow.HasControls());
    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());

    Invoker.LoadRecursive ();

    Assert.IsFalse (_modifiableRow.HasControls());
    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());
  }

  [Test]
  public void LoadValue ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _modifiableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    dataSource.LoadValues (false);

    BocTextValue textBoxStringValue = (BocTextValue) _modifiableRow.GetEditControl (0);
    BocTextValue textBoxInt32Value = (BocTextValue) _modifiableRow.GetEditControl (1);

    Assert.AreEqual ("A", textBoxStringValue.Value);
    Assert.AreEqual (1, textBoxInt32Value.Value);
  }

  [Test]
  public void SaveValue ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;

    _modifiableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    dataSource.LoadValues (false);

    BocTextValue textBoxStringValue = (BocTextValue) _modifiableRow.GetEditControl (0);
    BocTextValue textBoxInt32Value = (BocTextValue) _modifiableRow.GetEditControl (1);

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

    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasEditControl (0));
    Assert.IsFalse (_modifiableRow.HasEditControl (1));

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_modifiableRow.HasEditControls());
    Assert.IsTrue (_modifiableRow.HasEditControl (0));
    Assert.IsFalse (_modifiableRow.HasEditControl (1));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void HasEditControlWithNegativeIndex ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    _modifiableRow.HasEditControl (-1);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void HasEditControlWithIndexOutOfPositiveRange ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    _modifiableRow.HasEditControl (3);
  }


  [Test]
  public void GetEditControl ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasEditControl (0));
    Assert.IsFalse (_modifiableRow.HasEditControl (0));

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_modifiableRow.HasEditControls());
    Assert.IsTrue (_modifiableRow.HasEditControl (0));
    Assert.IsFalse (_modifiableRow.HasEditControl (1));
    
    IBusinessObjectBoundModifiableWebControl control = _modifiableRow.GetEditControl (0);
    Assert.IsNotNull (control);
    Assert.IsTrue (control is BocTextValue);

    Assert.IsNull (_modifiableRow.GetEditControl (1));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void GetEditControlWithNegativeIndex ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    _modifiableRow.HasEditControl (-1);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void GetEditControlWithIndexOutOfPositiveRange ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    _modifiableRow.HasEditControl (3);
  }


  [Test]
  public void HasValidators ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_modifiableRow.HasValidators());
    Assert.IsFalse (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_modifiableRow.HasValidators());
    Assert.IsTrue (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));

    _modifiableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_modifiableRow.HasValidators());
    Assert.IsTrue (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));
  }

  [Test]
  public void HasValidatorsWithoutCreateControls ()
  {
    Invoker.InitRecursive();

    Assert.IsFalse (_modifiableRow.HasValidators());
    Assert.IsFalse (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    _modifiableRow.EnsureValidatorsRestored();

    Assert.IsFalse (_modifiableRow.HasValidators());
    Assert.IsFalse (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_modifiableRow.HasValidators());
    Assert.IsTrue (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));

    _modifiableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_modifiableRow.HasValidators());
    Assert.IsTrue (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void HasValidatorsWithNegativeIndex ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored();

    _modifiableRow.HasValidators (-1);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void HasValidatorsWithIndexOutOfPositiveRange ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored();

    _modifiableRow.HasValidators (3);
  }


  [Test]
  public void GetValidators ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_modifiableRow.HasValidators());
    Assert.IsTrue (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));

    ControlCollection validators = _modifiableRow.GetValidators (0);
    Assert.IsNotNull (validators);
    Assert.AreEqual (2, validators.Count);
    Assert.IsTrue (validators[0] is RequiredFieldValidator);
    Assert.IsTrue (validators[1] is NumericValidator);

    Assert.IsNull (_modifiableRow.GetValidators (1));
  }

  [Test]
  public void GetValidatorsWithoutCreateControls ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    _modifiableRow.EnsureValidatorsRestored();
    
    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_modifiableRow.HasValidators());
    Assert.IsTrue (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));

    ControlCollection validators = _modifiableRow.GetValidators (0);
    Assert.IsNotNull (validators);
    Assert.AreEqual (2, validators.Count);
    Assert.IsTrue (validators[0] is RequiredFieldValidator);
    Assert.IsTrue (validators[1] is NumericValidator);

    Assert.IsNull (_modifiableRow.GetValidators (1));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void GetValidatorsWithNegativeIndex ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored();

    _modifiableRow.GetValidators (-1);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void GetValidatorsWithIndexOutOfPositiveRange ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[1] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored();

    _modifiableRow.GetValidators (3);
  }
  

  [Test]
  public void IsRequired ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    Assert.IsFalse (_modifiableRow.IsRequired (0));
    Assert.IsTrue (_modifiableRow.IsRequired (1));
    Assert.IsFalse (_modifiableRow.IsRequired (2));
  }

  
  [Test]
  public void IsDirty ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    dataSource.LoadValues (false);

    Assert.IsFalse (_modifiableRow.IsDirty ());

    BocTextValue textBoxStringValue = (BocTextValue) _modifiableRow.GetEditControl (0);
    textBoxStringValue.Value = "a";

    Assert.IsTrue (_modifiableRow.IsDirty());
  }

  [Test]
  public void GetTrackedIDs ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    dataSource.LoadValues (false);

    string id = "NamingContainer_Row_{0}_Boc_TextBox";
    string[] trackedIDs = new string[2];
    trackedIDs[0] = string.Format (id, 0);
    trackedIDs[1] = string.Format (id, 1);

    Assert.AreEqual (trackedIDs, _modifiableRow.GetTrackedClientIDs());
  }


  [Test]
  public void ValidateWithValidValues ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored ();

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    dataSource.LoadValues (false);
    
    SetValues (_modifiableRow, "A", "300");

    Assert.IsTrue (_modifiableRow.Validate());
  }

  [Test]
  public void ValidateWithInvalidValues ()
  {
    Invoker.InitRecursive();

    _modifiableRow.DataSourceFactory = new ModifiableRowDataSourceFactory();
    _modifiableRow.ControlFactory = new ModifiableRowControlFactory();

    BocColumnDefinition[] columns = new BocColumnDefinition[3];
    columns[0] = _typeWithAllDataTypesStringValueSimpleColumn;
    columns[1] = _typeWithAllDataTypesInt32ValueSimpleColumn;
    columns[2] = _commandColumn;

    _modifiableRow.CreateControls (columns, _value01);
    _modifiableRow.EnsureValidatorsRestored ();

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    dataSource.LoadValues (false);
    
    SetValues (_modifiableRow, "A", "");

    Assert.IsFalse (_modifiableRow.Validate());
  }


  private void SetValues (ModifiableRow row, string stringValue, string int32Value)
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
