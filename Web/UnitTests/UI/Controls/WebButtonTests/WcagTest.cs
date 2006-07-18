using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.Web.UnitTests.UI.Controls.WebButtonTests
{

[TestFixture]
public class WcagTest : BaseTest
{
  private TestWebButton _webButton;

  protected override void SetUpPage()
  {
    base.SetUpPage();
    _webButton = new TestWebButton();
    _webButton.ID = "WebButton";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    _webButton.EvaluateWaiConformity();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    _webButton.EvaluateWaiConformity();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithUseLegacyButtonIsFalse()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    _webButton.EvaluateWaiConformity();
    
    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_webButton, WcagHelperMock.Control);
    Assert.AreEqual ("UseLegacyButton", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithUseLegacyButtonIsUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _webButton.UseLegacyButton = NaBooleanEnum.Undefined;
    _webButton.EvaluateWaiConformity();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_webButton, WcagHelperMock.Control);
    Assert.AreEqual ("UseLegacyButton", WcagHelperMock.Property);
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
