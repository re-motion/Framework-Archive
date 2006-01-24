using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UnitTests.Domain;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocBooleanValueTest: BocTest
{
  private BocBooleanValueMock _bocBooleanValue;
  private TypeWithBoolean _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectBooleanProperty _propertyBooleanValue;
  private IBusinessObjectBooleanProperty _propertyNaBooleanValue;

  public BocBooleanValueTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocBooleanValue = new BocBooleanValueMock();
    _bocBooleanValue.ID = "BocBooleanValue";
    NamingContainer.Controls.Add (_bocBooleanValue);

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
    _bocBooleanValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocBooleanValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocBooleanValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocBooleanValue, WcagHelperMock.Control);
    Assert.IsNull (WcagHelperMock.Property);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocBooleanValue.ReadOnly = NaBoolean.True;
    string[] actual = _bocBooleanValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditMode()
  {
    _bocBooleanValue.ReadOnly = NaBoolean.False;
    string[] actual = _bocBooleanValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocBooleanValue.HiddenField.ClientID, actual[0]);
  }


  [Test]
  public void SetValueToTrue()
  {
    _bocBooleanValue.IsDirty = false;
    _bocBooleanValue.Value = true;
    Assert.AreEqual (true, _bocBooleanValue.Value);
    Assert.IsTrue (_bocBooleanValue.IsDirty);
  }
    
  [Test]
  public void SetValueToFalse()
  {
    _bocBooleanValue.IsDirty = false;
    _bocBooleanValue.Value = false;
    Assert.AreEqual (false, _bocBooleanValue.Value);
    Assert.IsTrue (_bocBooleanValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocBooleanValue.IsDirty = false;
    _bocBooleanValue.Value = null;
    Assert.AreEqual (null, _bocBooleanValue.Value);
    Assert.IsTrue (_bocBooleanValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNaBooleanTrue()
  {
    _bocBooleanValue.IsDirty = false;
    _bocBooleanValue.Value = NaBoolean.True;
    Assert.AreEqual (true, _bocBooleanValue.Value);
    Assert.IsTrue (_bocBooleanValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNaBooleanFalse()
  {
    _bocBooleanValue.IsDirty = false;
    _bocBooleanValue.Value = NaBoolean.False;
    Assert.AreEqual (false, _bocBooleanValue.Value);
    Assert.IsTrue (_bocBooleanValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNaBooleanNull()
  {
    _bocBooleanValue.IsDirty = false;
    _bocBooleanValue.Value = NaBoolean.Null;
    Assert.AreEqual (null, _bocBooleanValue.Value);
    Assert.IsTrue (_bocBooleanValue.IsDirty);
  }


  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimTrue()
  {
    _businessObject.BooleanValue = true;
    _bocBooleanValue.DataSource = _dataSource;
    _bocBooleanValue.Property = _propertyBooleanValue;
    _bocBooleanValue.Value = null;
    _bocBooleanValue.IsDirty = true;

    _bocBooleanValue.LoadValue (true);
    Assert.AreEqual (null, _bocBooleanValue.Value);
    Assert.IsTrue (_bocBooleanValue.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithValueTrue()
  {
    _businessObject.BooleanValue = true;
    _bocBooleanValue.DataSource = _dataSource;
    _bocBooleanValue.Property = _propertyBooleanValue;
    _bocBooleanValue.Value = null;
    _bocBooleanValue.IsDirty = true;

    _bocBooleanValue.LoadValue (false);
    Assert.AreEqual (_businessObject.BooleanValue, _bocBooleanValue.Value);
    Assert.IsFalse (_bocBooleanValue.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithValueFalse()
  {
    _businessObject.BooleanValue = false;
    _bocBooleanValue.DataSource = _dataSource;
    _bocBooleanValue.Property = _propertyBooleanValue;
    _bocBooleanValue.Value = null;
    _bocBooleanValue.IsDirty = true;

    _bocBooleanValue.LoadValue (false);
    Assert.AreEqual (_businessObject.BooleanValue, _bocBooleanValue.Value);
    Assert.IsFalse (_bocBooleanValue.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithValueNaBooelanTrue()
  {
    _businessObject.NaBooleanValue = NaBoolean.True;
    _bocBooleanValue.DataSource = _dataSource;
    _bocBooleanValue.Property = _propertyNaBooleanValue;
    _bocBooleanValue.Value = null;
    _bocBooleanValue.IsDirty = true;

    _bocBooleanValue.LoadValue (false);
    NaBoolean actual = NaBoolean.FromBoxedBoolean (_bocBooleanValue.Value);
    Assert.AreEqual (_businessObject.NaBooleanValue, actual);
    Assert.IsFalse (_bocBooleanValue.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithValueNaBooelanFalse()
  {
    _businessObject.NaBooleanValue = NaBoolean.False;
    _bocBooleanValue.DataSource = _dataSource;
    _bocBooleanValue.Property = _propertyNaBooleanValue;
    _bocBooleanValue.Value = null;
    _bocBooleanValue.IsDirty = true;

    _bocBooleanValue.LoadValue (false);
    NaBoolean actual = NaBoolean.FromBoxedBoolean (_bocBooleanValue.Value);
    Assert.AreEqual (_businessObject.NaBooleanValue, actual);
    Assert.IsFalse (_bocBooleanValue.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithValueNaBooelanNull()
  {
    _businessObject.NaBooleanValue = NaBoolean.Null;
    _bocBooleanValue.DataSource = _dataSource;
    _bocBooleanValue.Property = _propertyNaBooleanValue;
    _bocBooleanValue.Value = true;
    _bocBooleanValue.IsDirty = true;

    _bocBooleanValue.LoadValue (false);
    NaBoolean actual = NaBoolean.FromBoxedBoolean (_bocBooleanValue.Value);
    Assert.AreEqual (_businessObject.NaBooleanValue, actual);
    Assert.IsFalse (_bocBooleanValue.IsDirty);
  }
}

}
