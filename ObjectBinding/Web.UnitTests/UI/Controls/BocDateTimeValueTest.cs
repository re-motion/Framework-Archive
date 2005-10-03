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
public class BocDateTimeValueTest
{
  private BocDateTimeValueMock _bocDateTimeValue;

  public BocDateTimeValueTest()
  {
  }

  
  [SetUp]
  public virtual void SetUp()
  {
    _bocDateTimeValue = new BocDateTimeValueMock();
    _bocDateTimeValue.ID = "BocDateTimeValue";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocDateTimeValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocDateTimeValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocDateTimeValue.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocDateTimeValue.DateTextBoxStyle.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocDateTimeValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocDateTimeValue.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithTimeTextBoxActive()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocDateTimeValue.ValueType = BocDateTimeValueType.DateTime;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocDateTimeValue.WcagHelperMock.HasError);
    Assert.AreEqual (2, _bocDateTimeValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, _bocDateTimeValue.WcagHelperMock.Control);
    Assert.AreEqual ("ActualValueType", _bocDateTimeValue.WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithDateTimeTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.DateTimeTextBoxStyle.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocDateTimeValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocDateTimeValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, _bocDateTimeValue.WcagHelperMock.Control);
    Assert.AreEqual ("DateTimeTextBoxStyle.AutoPostBack", _bocDateTimeValue.WcagHelperMock.Property);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithDateTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.DateTextBoxStyle.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocDateTimeValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocDateTimeValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, _bocDateTimeValue.WcagHelperMock.Control);
    Assert.AreEqual ("DateTextBoxStyle.AutoPostBack", _bocDateTimeValue.WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithDateTextBoxAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.DateTextBox.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocDateTimeValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocDateTimeValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, _bocDateTimeValue.WcagHelperMock.Control);
    Assert.AreEqual ("DateTextBox.AutoPostBack", _bocDateTimeValue.WcagHelperMock.Property);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTimeTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.TimeTextBoxStyle.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocDateTimeValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocDateTimeValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, _bocDateTimeValue.WcagHelperMock.Control);
    Assert.AreEqual ("TimeTextBoxStyle.AutoPostBack", _bocDateTimeValue.WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTimeTextBoxAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDateTimeValue.TimeTextBox.AutoPostBack = true;
    _bocDateTimeValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocDateTimeValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocDateTimeValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocDateTimeValue, _bocDateTimeValue.WcagHelperMock.Control);
    Assert.AreEqual ("TimeTextBox.AutoPostBack", _bocDateTimeValue.WcagHelperMock.Property);
  }
}

}
