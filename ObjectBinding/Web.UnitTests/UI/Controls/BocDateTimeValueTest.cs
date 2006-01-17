using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocDateTimeValueTest: BocTest
{
  private BocDateTimeValueMock _bocDateTimeValue;

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
}

}
