using System;
using NUnit.Framework;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Web.UnitTests.Domain;
using Rubicon.Web.UnitTests.Configuration;

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

      _businessObject = TypeWithBoolean.Create();

      _propertyBooleanValue = (IBusinessObjectBooleanProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("BooleanValue");
      _propertyNaBooleanValue = (IBusinessObjectBooleanProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("NullableBooleanValue");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _bocBooleanValue.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }

    [Test]
    public void EvaluateWaiConformityLevelA()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _bocBooleanValue.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }


    [Test]
    public void EvaluateWaiConformityDebugLevelA()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
      _bocBooleanValue.EvaluateWaiConformity();

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
      _bocBooleanValue.Value = true;
      Assert.AreEqual (true, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void SetValueToNaBooleanFalse()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = false;
      Assert.AreEqual (false, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void SetValueToNaBooleanNull()
    {
      _bocBooleanValue.IsDirty = false;
      _bocBooleanValue.Value = null;
      Assert.AreEqual (null, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }


    [Test]
    public void LoadValueAndInterimTrue()
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
    public void LoadValueAndInterimFalseWithValueTrue()
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
    public void LoadValueAndInterimFalseWithValueFalse()
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
    public void LoadValueAndInterimFalseWithValueNullableBooelanTrue()
    {
      _businessObject.NullableBooleanValue = true;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyNaBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue (false);
      Assert.AreEqual (_businessObject.NullableBooleanValue, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanFalse()
    {
      _businessObject.NullableBooleanValue = false;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyNaBooleanValue;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue (false);
      Assert.AreEqual (_businessObject.NullableBooleanValue, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithValueNullableBooelanNull ()
    {
      _businessObject.NullableBooleanValue = null;
      _bocBooleanValue.DataSource = _dataSource;
      _bocBooleanValue.Property = _propertyNaBooleanValue;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadValue (false);
      Assert.AreEqual (_businessObject.NullableBooleanValue, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }


    [Test]
    public void LoadUnboundValueAndInterimTrue()
    {
      bool value = true;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, true);
      Assert.AreEqual (null, _bocBooleanValue.Value);
      Assert.IsTrue (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueTrue()
    {
      bool value = true;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueFalse()
    {
      bool value = false;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNull()
    {
      bool? value = null;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanTrue()
    {
      bool? value = true;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanFalse()
    {
      bool? value = false;
      _bocBooleanValue.Value = null;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithValueNullableBooelanNull()
    {
      bool? value = null;
      _bocBooleanValue.Value = true;
      _bocBooleanValue.IsDirty = true;

      _bocBooleanValue.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _bocBooleanValue.Value);
      Assert.IsFalse (_bocBooleanValue.IsDirty);
    }
  }
}