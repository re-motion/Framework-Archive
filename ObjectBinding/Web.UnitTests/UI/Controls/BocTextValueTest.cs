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
public class BocTextValueTest
{
  private BocTextValueMock _bocTextValue;

  public BocTextValueTest()
  {
  }

  
  [SetUp]
  public virtual void SetUp()
  {
    _bocTextValue = new BocTextValueMock();
    _bocTextValue.ID = "BocTextValue";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocTextValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocTextValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocTextValue.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocTextValue.TextBoxStyle.AutoPostBack = true;
    _bocTextValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocTextValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocTextValue.WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocTextValue.TextBoxStyle.AutoPostBack = true;
    _bocTextValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocTextValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocTextValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocTextValue, _bocTextValue.WcagHelperMock.Control);
    Assert.AreEqual ("TextBoxStyle.AutoPostBack", _bocTextValue.WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTextBoxAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocTextValue.TextBox.AutoPostBack = true;
    _bocTextValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocTextValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocTextValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocTextValue, _bocTextValue.WcagHelperMock.Control);
    Assert.AreEqual ("TextBox.AutoPostBack", _bocTextValue.WcagHelperMock.Property);
  }
}

}
