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
public class BocMultilineTextValueTest
{
  private BocMultilineTextValueMock _bocMultilineTextValue;

  public BocMultilineTextValueTest()
  {
  }

  
  [SetUp]
  public virtual void SetUp()
  {
    _bocMultilineTextValue = new BocMultilineTextValueMock();
    _bocMultilineTextValue.ID = "BocMultilineTextValue";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocMultilineTextValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocMultilineTextValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocMultilineTextValue.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocMultilineTextValue.TextBoxStyle.AutoPostBack = true;
    _bocMultilineTextValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocMultilineTextValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocMultilineTextValue.WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocMultilineTextValue.TextBoxStyle.AutoPostBack = true;
    _bocMultilineTextValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocMultilineTextValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocMultilineTextValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocMultilineTextValue, _bocMultilineTextValue.WcagHelperMock.Control);
    Assert.AreEqual ("TextBoxStyle.AutoPostBack", _bocMultilineTextValue.WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTextBoxAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocMultilineTextValue.TextBox.AutoPostBack = true;
    _bocMultilineTextValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocMultilineTextValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocMultilineTextValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocMultilineTextValue, _bocMultilineTextValue.WcagHelperMock.Control);
    Assert.AreEqual ("TextBox.AutoPostBack", _bocMultilineTextValue.WcagHelperMock.Property);
  }
}

}
