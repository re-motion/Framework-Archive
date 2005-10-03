using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.Web.UnitTests.UI.Controls
{

[TestFixture]
public class WebButtonTest
{
  private WebButtonMock _webButton;

  [SetUp]
  public virtual void SetUp()
  {
    _webButton = new WebButtonMock();
    _webButton.ID = "WebButton";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    _webButton.EvaluateWaiConformity();
    
    Assert.IsFalse (_webButton.WcagHelperMock.HasWarning);
    Assert.IsFalse (_webButton.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    _webButton.EvaluateWaiConformity();
    
    Assert.IsFalse (_webButton.WcagHelperMock.HasWarning);
    Assert.IsFalse (_webButton.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithUseLegacyButtonIsFalse()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    _webButton.EvaluateWaiConformity();
    
    Assert.IsTrue (_webButton.WcagHelperMock.HasError);
    Assert.AreEqual (1, _webButton.WcagHelperMock.Priority);
    Assert.AreSame (_webButton, _webButton.WcagHelperMock.Control);
    Assert.AreEqual ("UseLegacyButton", _webButton.WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithUseLegacyButtonIsUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _webButton.UseLegacyButton = NaBooleanEnum.Undefined;
    _webButton.EvaluateWaiConformity();

    Assert.IsTrue (_webButton.WcagHelperMock.HasError);
    Assert.AreEqual (1, _webButton.WcagHelperMock.Priority);
    Assert.AreSame (_webButton, _webButton.WcagHelperMock.Control);
    Assert.AreEqual ("UseLegacyButton", _webButton.WcagHelperMock.Property);
  }


  [Test]
  public void IsLegacyButtonEnabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    Assert.IsTrue (_webButton.IsLegacyButtonEnabled);
  }

  [Test]
  public void IsLegacyButtonEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    Assert.IsFalse (_webButton.IsLegacyButtonEnabled);
  }
}

}
