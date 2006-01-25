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
public class BocDateTimeValueTest: BocTest
{
  private BocDateTimeValueMock _bocDateTimeValue;
  private TypeWithDateTime _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectDateTimeProperty _propertyDateTimeValue;
  private IBusinessObjectDateTimeProperty _propertyNaDateTimeValue;

  public BocDateTimeValueTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocDateTimeValue = new BocDateTimeValueMock();
    _bocDateTimeValue.ID = "BocDateTimeValue";
    NamingContainer.Controls.Add (_bocDateTimeValue);
  
    _businessObject = new TypeWithDateTime();
    
    _propertyDateTimeValue = (IBusinessObjectDateTimeProperty) _businessObject.GetBusinessObjectProperty ("DateTimeValue");
    _propertyNaDateTimeValue = (IBusinessObjectDateTimeProperty) _businessObject.GetBusinessObjectProperty ("NaDateTimeValue");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocDateTimeValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocDateTimeValue.DateTextBoxStyle.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithTimeTextBoxActive()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocDateTimeValue.ValueType = BocDateTimeValueType.DateTime;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (2, WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
    Assert.AreEqual ("ActualValueType", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithDateTimeTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.DateTimeTextBoxStyle.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
    Assert.AreEqual ("DateTimeTextBoxStyle.AutoPostBack", WcagHelperMock.Property);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithDateTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.DateTextBoxStyle.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
    Assert.AreEqual ("DateTextBoxStyle.AutoPostBack", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithDateTextBoxAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.DateTextBox.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
    Assert.AreEqual ("DateTextBox.AutoPostBack", WcagHelperMock.Property);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTimeTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.TimeTextBoxStyle.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
    Assert.AreEqual ("TimeTextBoxStyle.AutoPostBack", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTimeTextBoxAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.TimeTextBox.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, WcagHelperMock.Control);
    Assert.AreEqual ("TimeTextBox.AutoPostBack", WcagHelperMock.Property);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocDateTimeValue.ReadOnly = NaBoolean.True;
    string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAndValueTypeIsDateTime()
  {
    _bocDateTimeValue.ReadOnly = NaBoolean.False;
    _bocDateTimeValue.ValueType = BocDateTimeValueType.DateTime;
    string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (2, actual.Length);
    Assert.AreEqual (_bocDateTimeValue.DateTextBox.ClientID, actual[0]);
    Assert.AreEqual (_bocDateTimeValue.TimeTextBox.ClientID, actual[1]);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAndValueTypeIsDate()
  {
    _bocDateTimeValue.ReadOnly = NaBoolean.False;
    _bocDateTimeValue.ValueType = BocDateTimeValueType.Date;
    string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocDateTimeValue.DateTextBox.ClientID, actual[0]);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAndValueTypeIsUndefined()
  {
    _bocDateTimeValue.ReadOnly = NaBoolean.False;
    _bocDateTimeValue.ValueType = BocDateTimeValueType.Undefined;
    string[] actual = _bocDateTimeValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (2, actual.Length);
    Assert.AreEqual (_bocDateTimeValue.DateTextBox.ClientID, actual[0]);
    Assert.AreEqual (_bocDateTimeValue.TimeTextBox.ClientID, actual[1]);
  }


  [Test]
  public void SetValueToDateTime()
  {
    DateTime dateTime = new DateTime (2006, 1, 1, 1, 1, 1);
    _bocDateTimeValue.IsDirty = false;
    _bocDateTimeValue.Value = dateTime;
    Assert.AreEqual (dateTime, _bocDateTimeValue.Value);
    Assert.IsTrue (_bocDateTimeValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocDateTimeValue.IsDirty = false;
    _bocDateTimeValue.Value = null;
    Assert.AreEqual (null, _bocDateTimeValue.Value);
    Assert.IsTrue (_bocDateTimeValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNaDateTime()
  {
    NaDateTime dateTime = new NaDateTime (2006, 1, 1, 1, 1, 1);
    _bocDateTimeValue.IsDirty = false;
    _bocDateTimeValue.Value = dateTime;
    NaDateTime actual = NaDateTime.FromBoxedDateTime (_bocDateTimeValue.Value);
    Assert.AreEqual (dateTime, actual);
    Assert.IsTrue (_bocDateTimeValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNaDateTimeNull()
  {
    _bocDateTimeValue.IsDirty = false;
    _bocDateTimeValue.Value = NaDateTime.Null;
    Assert.AreEqual (null, _bocDateTimeValue.Value);
    Assert.IsTrue (_bocDateTimeValue.IsDirty);
  }


  [Test]
  public void LoadValueAndInterimTrue()
  {
    _businessObject.DateTimeValue = new DateTime (2006, 1, 1, 1, 1, 1);
    _bocDateTimeValue.DataSource = _dataSource;
    _bocDateTimeValue.Property = _propertyDateTimeValue;
    _bocDateTimeValue.Value = null;
    _bocDateTimeValue.IsDirty = true;

    _bocDateTimeValue.LoadValue (true);
    Assert.AreEqual (null, _bocDateTimeValue.Value);
    Assert.IsTrue (_bocDateTimeValue.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithDateTime()
  {
    _businessObject.DateTimeValue = new DateTime (2006, 1, 1, 1, 1, 1);
    _bocDateTimeValue.DataSource = _dataSource;
    _bocDateTimeValue.Property = _propertyDateTimeValue;
    _bocDateTimeValue.Value = null;
    _bocDateTimeValue.IsDirty = true;

    _bocDateTimeValue.LoadValue (false);
    Assert.AreEqual (_businessObject.DateTimeValue, _bocDateTimeValue.Value);
    Assert.IsFalse (_bocDateTimeValue.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithValueNaDateTime()
  {
    _businessObject.NaDateTimeValue = new NaDateTime (2006, 1, 1, 1, 1, 1);
    _bocDateTimeValue.DataSource = _dataSource;
    _bocDateTimeValue.Property = _propertyNaDateTimeValue;
    _bocDateTimeValue.Value = null;
    _bocDateTimeValue.IsDirty = true;

    _bocDateTimeValue.LoadValue (false);
    NaDateTime actual = NaDateTime.FromBoxedDateTime (_bocDateTimeValue.Value);
    Assert.AreEqual (_businessObject.NaDateTimeValue, actual);
    Assert.IsFalse (_bocDateTimeValue.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithValueNaDateTimeNull()
  {
    _businessObject.NaDateTimeValue = NaDateTime.Null;
    _bocDateTimeValue.DataSource = _dataSource;
    _bocDateTimeValue.Property = _propertyNaDateTimeValue;
    _bocDateTimeValue.Value = DateTime.Now;
    _bocDateTimeValue.IsDirty = true;

    _bocDateTimeValue.LoadValue (false);
    NaDateTime actual = NaDateTime.FromBoxedDateTime (_bocDateTimeValue.Value);
    Assert.AreEqual (_businessObject.NaDateTimeValue, actual);
    Assert.IsFalse (_bocDateTimeValue.IsDirty);
  }


  [Test]
  public void LoadUnboundValueAndInterimTrue()
  {
    DateTime value = new DateTime (2006, 1, 1, 1, 1, 1);
    _bocDateTimeValue.Value = null;
    _bocDateTimeValue.IsDirty = true;

    _bocDateTimeValue.LoadUnboundValue (value, true);
    Assert.AreEqual (null, _bocDateTimeValue.Value);
    Assert.IsTrue (_bocDateTimeValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithDateTime()
  {
    DateTime value = new DateTime (2006, 1, 1, 1, 1, 1);
    _bocDateTimeValue.Value = null;
    _bocDateTimeValue.IsDirty = true;

    _bocDateTimeValue.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocDateTimeValue.Value);
    Assert.IsFalse (_bocDateTimeValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithValueNull()
  {
    object value = null;
    _bocDateTimeValue.Value = DateTime.Now;
    _bocDateTimeValue.IsDirty = true;

    _bocDateTimeValue.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocDateTimeValue.Value);
    Assert.IsFalse (_bocDateTimeValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithValueNaDateTime()
  {
    NaDateTime value = new NaDateTime (2006, 1, 1, 1, 1, 1);
    _bocDateTimeValue.Value = null;
    _bocDateTimeValue.IsDirty = true;

    _bocDateTimeValue.LoadUnboundValue (value, false);
    NaDateTime actual = NaDateTime.FromBoxedDateTime (_bocDateTimeValue.Value);
    Assert.AreEqual (value, actual);
    Assert.IsFalse (_bocDateTimeValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithValueNaDateTimeNull()
  {
    NaDateTime value = NaDateTime.Null;
    _bocDateTimeValue.Value = DateTime.Now;
    _bocDateTimeValue.IsDirty = true;

    _bocDateTimeValue.LoadUnboundValue (value, false);
    NaDateTime actual = NaDateTime.FromBoxedDateTime (_bocDateTimeValue.Value);
    Assert.AreEqual (value, actual);
    Assert.IsFalse (_bocDateTimeValue.IsDirty);
  }
}

}
