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
public class ModifiableRowTest
{
  // types

  // static members and constants

  // member fields
  
  private Rubicon.ObjectBinding.Web.UI.Controls.BocList _bocList;
  private ModifiableRow _modifiableRow;
  private ControlInvoker _invoker;

  private TypeWithString _value01;

  private ReflectionBusinessObjectClass _typeWithStringClass;

  private BusinessObjectPropertyPath _typeWithStringFirstValuePath;
  private BusinessObjectPropertyPath _typeWithStringSecondValuePath;

  private BocSimpleColumnDefinition _typeWithStringFirstValueSimpleColumn;
  private BocSimpleColumnDefinition _typeWithStringSecondValueSimpleColumn;
  private BocCompoundColumnDefinition _typeWithStringFirstValueFirstValueCompoundColumn;
  private BocCustomColumnDefinition _typeWithStringFirstValueCustomColumn;
  private BocCommandColumnDefinition _commandColumn;
  private BocEditDetailsColumnDefinition _editDetailsColumn;
  private BocDropDownMenuColumnDefinition _dropDownMenuColumn;

  // construction and disposing

  public ModifiableRowTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _bocList = new Rubicon.ObjectBinding.Web.UI.Controls.BocList ();
    _modifiableRow = new ModifiableRow (_bocList);
    _invoker = new ControlInvoker (_modifiableRow);

    _value01 = new TypeWithString();
    _value01.FirstValue = "01-1";
    _value01.SecondValue = "01-2";

    _typeWithStringClass = new ReflectionBusinessObjectClass (typeof (TypeWithString));

    _typeWithStringFirstValuePath = BusinessObjectPropertyPath.Parse (_typeWithStringClass, "FirstValue");
    _typeWithStringSecondValuePath = BusinessObjectPropertyPath.Parse (_typeWithStringClass, "SecondValue");

    _typeWithStringFirstValueSimpleColumn = new BocSimpleColumnDefinition ();
    _typeWithStringFirstValueSimpleColumn.PropertyPath = _typeWithStringFirstValuePath;

    _typeWithStringSecondValueSimpleColumn = new BocSimpleColumnDefinition ();
    _typeWithStringSecondValueSimpleColumn.PropertyPath = _typeWithStringSecondValuePath;

    _typeWithStringFirstValueFirstValueCompoundColumn = new BocCompoundColumnDefinition();
    _typeWithStringFirstValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithStringFirstValuePath));
    _typeWithStringFirstValueFirstValueCompoundColumn.PropertyPathBindings.Add (
        new PropertyPathBinding (_typeWithStringFirstValuePath));
    _typeWithStringFirstValueFirstValueCompoundColumn.FormatString = "{0}, {1}";

    _typeWithStringFirstValueCustomColumn = new BocCustomColumnDefinition ();
    _typeWithStringFirstValueCustomColumn.PropertyPath = _typeWithStringFirstValuePath;
    _typeWithStringFirstValueCustomColumn.IsSortable = true;

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
  public void CreateControlsWithNoColumns ()
  {
    Assert.IsFalse (_modifiableRow.HasControls());
    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());

    _modifiableRow.CreateControls (new BocColumnDefinition[0], _value01);

    Assert.IsTrue (_modifiableRow.HasControls());
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
    Assert.IsFalse (_modifiableRow.HasControls());
    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());

    BocColumnDefinition[] columns = new BocColumnDefinition[7];
    columns[0] = _typeWithStringFirstValueSimpleColumn;
    columns[1] = _typeWithStringFirstValueFirstValueCompoundColumn;
    columns[2] = _typeWithStringFirstValueCustomColumn;
    columns[3] = _commandColumn;
    columns[4] = _editDetailsColumn;
    columns[5] = _dropDownMenuColumn;
    columns[6] = _typeWithStringSecondValueSimpleColumn;

    _modifiableRow.CreateControls (columns, _value01);

    Assert.IsTrue (_modifiableRow.HasControls());
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
    Assert.AreSame (_typeWithStringFirstValuePath.LastProperty, textBoxFirstValue.Property);

    IBusinessObjectBoundModifiableWebControl textBoxSecondValue = _modifiableRow.GetEditControl (6);
    Assert.IsTrue (textBoxSecondValue is BocTextValue);
    Assert.AreSame (dataSource, textBoxSecondValue.DataSource);
    Assert.AreSame (_typeWithStringSecondValuePath.LastProperty, textBoxSecondValue.Property);
  }

  [Test]
  public void EnsureValidators ()
  {
    BocColumnDefinition[] columns = new BocColumnDefinition[7];
    columns[0] = _typeWithStringFirstValueSimpleColumn;
    columns[1] = _typeWithStringFirstValueFirstValueCompoundColumn;
    columns[2] = _typeWithStringFirstValueCustomColumn;
    columns[3] = _commandColumn;
    columns[4] = _editDetailsColumn;
    columns[5] = _dropDownMenuColumn;
    columns[6] = _typeWithStringSecondValueSimpleColumn;

    _modifiableRow.CreateControls (columns, _value01);

    Assert.IsFalse (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));
    Assert.IsFalse (_modifiableRow.HasValidators (2));
    Assert.IsFalse (_modifiableRow.HasValidators (3));
    Assert.IsFalse (_modifiableRow.HasValidators (4));
    Assert.IsFalse (_modifiableRow.HasValidators (5));
    Assert.IsFalse (_modifiableRow.HasValidators (6));

    BocTextValue textBoxFirstValue = (BocTextValue) _modifiableRow.GetEditControl (0);
    textBoxFirstValue.Required = NaBooleanEnum.True;
    textBoxFirstValue.TextBoxStyle.MaxLength = 20;

    _modifiableRow.EnsureValidatorsRestored();

    Assert.IsTrue (_modifiableRow.HasValidators (0));
    Assert.IsFalse (_modifiableRow.HasValidators (1));
    Assert.IsFalse (_modifiableRow.HasValidators (2));
    Assert.IsFalse (_modifiableRow.HasValidators (3));
    Assert.IsFalse (_modifiableRow.HasValidators (4));
    Assert.IsFalse (_modifiableRow.HasValidators (5));
    Assert.IsFalse (_modifiableRow.HasValidators (6));

    ControlCollection validators = _modifiableRow.GetValidators (0);
    Assert.IsNotNull (validators);
    Assert.AreEqual (2, validators.Count);
    Assert.IsTrue (validators[0] is RequiredFieldValidator);
    Assert.IsTrue (validators[1] is LengthValidator);
  }

  [Test]
  public void ControlInit ()
  {
    Assert.IsFalse (_modifiableRow.HasControls());
    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());

    _invoker.InitRecursive ();

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

    _invoker.LoadRecursive ();

    Assert.IsFalse (_modifiableRow.HasControls());
    Assert.IsFalse (_modifiableRow.HasEditControls());
    Assert.IsFalse (_modifiableRow.HasValidators());
  }

  [Test]
  public void LoadValue ()
  {
    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithStringFirstValueSimpleColumn;
    columns[1] = _typeWithStringSecondValueSimpleColumn;

    _modifiableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    dataSource.LoadValues (false);

    BocTextValue textBoxFirstValue = (BocTextValue) _modifiableRow.GetEditControl (0);
    BocTextValue textBoxSecondValue = (BocTextValue) _modifiableRow.GetEditControl (1);

    Assert.AreEqual ("01-1", textBoxFirstValue.Value);
    Assert.AreEqual ("01-2", textBoxSecondValue.Value);
  }

  [Test]
  public void SaveValue ()
  {
    BocColumnDefinition[] columns = new BocColumnDefinition[2];
    columns[0] = _typeWithStringFirstValueSimpleColumn;
    columns[1] = _typeWithStringSecondValueSimpleColumn;

    _modifiableRow.CreateControls (columns, _value01);

    IBusinessObjectReferenceDataSource dataSource = _modifiableRow.GetDataSource();
    dataSource.LoadValues (false);

    BocTextValue textBoxFirstValue = (BocTextValue) _modifiableRow.GetEditControl (0);
    BocTextValue textBoxSecondValue = (BocTextValue) _modifiableRow.GetEditControl (1);

    Assert.AreEqual ("01-1", textBoxFirstValue.Value);
    Assert.AreEqual ("01-2", textBoxSecondValue.Value);

    textBoxFirstValue.Value = "a";
    textBoxSecondValue.Value = "b";

    dataSource.SaveValues (false);

    Assert.AreEqual ("a", _value01.FirstValue);
    Assert.AreEqual ("b", _value01.SecondValue);
  }
}
}
