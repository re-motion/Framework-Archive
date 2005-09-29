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
  private WebConfigurationMock _debugLevelAWebConfiguration;
  private WebConfigurationMock _debugLevelUndefinedWebConfiguration;
  private WebConfigurationMock _levelAWebConfiguration;

  [SetUp]
  public virtual void SetUp()
  {
    _webButton = new WebButtonMock();
    _webButton.ID = "WebButton";

    _debugLevelAWebConfiguration = new WebConfigurationMock();
    _debugLevelAWebConfiguration.Wcag.Debug = true;
    _debugLevelAWebConfiguration.Wcag.ConformanceLevel = WaiConformanceLevel.A;

    _debugLevelUndefinedWebConfiguration = new WebConfigurationMock();
    _debugLevelUndefinedWebConfiguration.Wcag.Debug = true;
    _debugLevelUndefinedWebConfiguration.Wcag.ConformanceLevel = WaiConformanceLevel.Undefined;

    _levelAWebConfiguration = new WebConfigurationMock();
    _levelAWebConfiguration.Wcag.Debug = false;
    _levelAWebConfiguration.Wcag.ConformanceLevel = WaiConformanceLevel.A;
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = _debugLevelUndefinedWebConfiguration;
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    _webButton.EvaluateWaiConformity();
    // Assert.Succeed();
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = _levelAWebConfiguration;
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    _webButton.EvaluateWaiConformity();
    // Assert.Succeed();
  }

	[Test]
  [ExpectedException (typeof (WcagException), "Property UseLegacyButton of Control WebButton does comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithUseLegacyButtonIsFalse()
  {
    WebConfigurationMock.Current = _debugLevelAWebConfiguration;
    _webButton.UseLegacyButton = NaBooleanEnum.False;
    _webButton.EvaluateWaiConformity();
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (WcagException), "Property UseLegacyButton of Control WebButton does comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithUseLegacyButtonIsUndefined()
  {
    WebConfigurationMock.Current = _debugLevelAWebConfiguration;
    _webButton.UseLegacyButton = NaBooleanEnum.Undefined;
    _webButton.EvaluateWaiConformity();
    Assert.Fail();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithUseLegacyButtonIsTrue()
  {
    WebConfigurationMock.Current = _debugLevelAWebConfiguration;
    _webButton.UseLegacyButton = NaBooleanEnum.True;
    _webButton.EvaluateWaiConformity();
    // Assert.Succeed();
  }
}

}
