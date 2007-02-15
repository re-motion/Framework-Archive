using System;
using NUnit.Framework;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Web.UnitTests.Domain;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocCheckBoxTest: BocTest
{
  private BocCheckBoxMock _bocCheckBox;
  private TypeWithBoolean _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectBooleanProperty _propertyBooleanValue;
  private IBusinessObjectBooleanProperty _propertyNaBooleanValue;

  public BocCheckBoxTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocCheckBox = new BocCheckBoxMock();
    _bocCheckBox.ID = "BocCheckBox";
    NamingContainer.Controls.Add (_bocCheckBox);
 
    _businessObject = new TypeWithBoolean();
    
    _propertyBooleanValue = (IBusinessObjectBooleanProperty) _businessObject.GetBusinessObjectProperty ("BooleanValue");
    _propertyNaBooleanValue = (IBusinessObjectBooleanProperty) _businessObject.GetBusinessObjectProperty ("NaBooleanValue");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocCheckBox.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocCheckBox.AutoPostBack = NaBoolean.True;
    _bocCheckBox.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocCheckBox.AutoPostBack = NaBoolean.True;
    _bocCheckBox.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocCheckBox, WcagHelperMock.Control);
    Assert.AreEqual ("AutoPostBack", WcagHelperMock.Property);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocCheckBox.ReadOnly = NaBoolean.True;
    string[] actual = _bocCheckBox.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditMode()
  {
    _bocCheckBox.ReadOnly = NaBoolean.False;
    string[] actual = _bocCheckBox.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocCheckBox.CheckBox.ClientID, actual[0]);
  }


  [Test]
  public void SetValueToTrue()
  {
    _bocCheckBox.IsDirty = false;
    _bocCheckBox.Value = true;
    Assert.AreEqual (true, _bocCheckBox.Value);
    Assert.IsTrue (_bocCheckBox.IsDirty);
  }
    
  [Test]
  public void SetValueToFalse()
  {
    _bocCheckBox.IsDirty = false;
    _bocCheckBox.Value = false;
    Assert.AreEqual (false, _bocCheckBox.Value);
    Assert.IsTrue (_bocCheckBox.IsDirty);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocCheckBox.DefaultValue = NaBoolean.False;
    _bocCheckBox.IsDirty = false;
    _bocCheckBox.Value = null;
    Assert.AreEqual (false, _bocCheckBox.Value);
    Assert.IsTrue (_bocCheckBox.IsDirty);
  }
    
  [Test]
  public void SetValueToNaBooleanTrue()
  {
    _bocCheckBox.IsDirty = false;
    _bocCheckBox.Value = NaBoolean.True;
    Assert.AreEqual (true, _bocCheckBox.Value);
    Assert.IsTrue (_bocCheckBox.IsDirty);
  }
    
  [Test]
  public void SetValueToNaBooleanFalse()
  {
    _bocCheckBox.IsDirty = false;
    _bocCheckBox.Value = NaBoolean.False;
    Assert.AreEqual (false, _bocCheckBox.Value);
    Assert.IsTrue (_bocCheckBox.IsDirty);
  }
    
  [Test]
  public void SetValueToNaBooleanNull()
  {
    _bocCheckBox.DefaultValue = NaBoolean.False;
    _bocCheckBox.IsDirty = false;
    _bocCheckBox.Value = NaBoolean.Null;
    Assert.AreEqual (false, _bocCheckBox.Value);
    Assert.IsTrue (_bocCheckBox.IsDirty);
  }


  [Test]
  public void LoadValueAndInterimTrue()
  {
    _businessObject.BooleanValue = true;
    _bocCheckBox.DataSource = _dataSource;
    _bocCheckBox.Property = _propertyBooleanValue;
    _bocCheckBox.Value = false;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadValue (true);
    Assert.AreEqual (false, _bocCheckBox.Value);
    Assert.IsTrue (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithValueTrue()
  {
    _businessObject.BooleanValue = true;
    _bocCheckBox.DataSource = _dataSource;
    _bocCheckBox.Property = _propertyBooleanValue;
    _bocCheckBox.Value = false;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadValue (false);
    Assert.AreEqual (_businessObject.BooleanValue, _bocCheckBox.Value);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithValueFalse()
  {
    _businessObject.BooleanValue = false;
    _bocCheckBox.DataSource = _dataSource;
    _bocCheckBox.Property = _propertyBooleanValue;
    _bocCheckBox.Value = true;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadValue (false);
    Assert.AreEqual (_businessObject.BooleanValue, _bocCheckBox.Value);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithValueNaBooelanTrue()
  {
    _businessObject.NaBooleanValue = NaBoolean.True;
    _bocCheckBox.DataSource = _dataSource;
    _bocCheckBox.Property = _propertyNaBooleanValue;
    _bocCheckBox.Value = false;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadValue (false);
    NaBoolean actual = NaBoolean.FromBoxedBoolean (_bocCheckBox.Value);
    Assert.AreEqual (_businessObject.NaBooleanValue, actual);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithValueNaBooelanFalse()
  {
    _businessObject.NaBooleanValue = NaBoolean.False;
    _bocCheckBox.DataSource = _dataSource;
    _bocCheckBox.Property = _propertyNaBooleanValue;
    _bocCheckBox.Value = true;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadValue (false);
    NaBoolean actual = NaBoolean.FromBoxedBoolean (_bocCheckBox.Value);
    Assert.AreEqual (_businessObject.NaBooleanValue, actual);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithValueNaBooelanNull()
  {
    _businessObject.NaBooleanValue = NaBoolean.Null;
    _bocCheckBox.DefaultValue = NaBoolean.False;
    _bocCheckBox.DataSource = _dataSource;
    _bocCheckBox.Property = _propertyNaBooleanValue;
    _bocCheckBox.Value = true;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadValue (false);
    Assert.AreEqual (false, _bocCheckBox.Value);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }


  [Test]
  public void LoadUnboundValueAndInterimTrue()
  {
    bool value = true;
    _bocCheckBox.Value = false;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadUnboundValue (value, true);
    Assert.AreEqual (false, _bocCheckBox.Value);
    Assert.IsTrue (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithValueTrue()
  {
    bool value = true;
    _bocCheckBox.Value = false;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocCheckBox.Value);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithValueFalse()
  {
    bool value = false;
    _bocCheckBox.Value = true;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocCheckBox.Value);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithValueNull()
  {
    _bocCheckBox.DefaultValue = NaBooleanEnum.False;
    _bocCheckBox.Value = true;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadUnboundValue (null, false);
    Assert.AreEqual (false, _bocCheckBox.Value);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithValueNaBooelanTrue()
  {
    NaBoolean value = NaBoolean.True;
    _bocCheckBox.Value = false;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadUnboundValue (value, false);
    NaBoolean actual = NaBoolean.FromBoxedBoolean (_bocCheckBox.Value);
    Assert.AreEqual (value, actual);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithValueNaBooelanFalse()
  {
    NaBoolean value = NaBoolean.False;
    _bocCheckBox.Value = true;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadUnboundValue (value, false);
    NaBoolean actual = NaBoolean.FromBoxedBoolean (_bocCheckBox.Value);
    Assert.AreEqual (value, actual);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithValueNaBooelanNull()
  {
    NaBoolean value = NaBoolean.Null;
    _bocCheckBox.DefaultValue = NaBooleanEnum.False;
    _bocCheckBox.Value = true;
    _bocCheckBox.IsDirty = true;

    _bocCheckBox.LoadUnboundValue (value, false);
    Assert.AreEqual (false, _bocCheckBox.Value);
    Assert.IsFalse (_bocCheckBox.IsDirty);
  }
}

}
