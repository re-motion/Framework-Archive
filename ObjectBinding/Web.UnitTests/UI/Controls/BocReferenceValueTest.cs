using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocReferenceValueTest
{
  private BocReferenceValueMock _bocReferenceValue;

  [SetUp]
  public virtual void SetUp()
  {
    _bocReferenceValue = new BocReferenceValueMock();
    _bocReferenceValue.ID = "BocReferenceValue";
    _bocReferenceValue.ShowOptionsMenu = false;
    _bocReferenceValue.Command.Type = CommandType.None;
    _bocReferenceValue.Command.Show = CommandShow.Always;
    _bocReferenceValue.InternalValue = Guid.Empty.ToString();
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelUndefined();
    _bocReferenceValue.EvaluateWaiConformity ();
    // Assert.Succeed();
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.EvaluateWaiConformity ();
    //Assert.Succeed();
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'ShowOptionsMenu' for BocReferenceValueMock 'BocReferenceValue' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithShowOptionsMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.EvaluateWaiConformity ();
    Assert.Fail();
  }

  [Test]
  public void IsOptionsMenuInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsFalse (_bocReferenceValue.HasOptionsMenu);
  }

  [Test]
  public void IsOptionsMenuVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsTrue (_bocReferenceValue.HasOptionsMenu);
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'Command' for BocReferenceValueMock 'BocReferenceValue' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithEventCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocReferenceValue.Command.Type = CommandType.Event;
    _bocReferenceValue.EvaluateWaiConformity ();
    Assert.Fail();
  }

  [Test]
  public void IsEventCommandDisabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.Command.Type = CommandType.Event;
    Assert.IsFalse (_bocReferenceValue.IsCommandEnabled (false));
  }

  [Test]
  public void IsEventCommandEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocReferenceValue.Command.Type = CommandType.Event;
    Assert.IsTrue (_bocReferenceValue.IsCommandEnabled (false));
  }


	[Test]
  [ExpectedException (typeof (WcagException), "The value of property 'Command' for BocReferenceValueMock 'BocReferenceValue' does not comply with a priority 1 checkpoint.")]
  public void EvaluateWaiConformityDebugLevelAWithWxeFunctionCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    _bocReferenceValue.EvaluateWaiConformity ();
    Assert.Fail();
  }

  [Test]
  public void IsWxeFunctionCommandDisabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    Assert.IsFalse (_bocReferenceValue.IsCommandEnabled (false));
  }

  [Test]
  public void IsWxeFunctionCommandEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    Assert.IsTrue (_bocReferenceValue.IsCommandEnabled (false));
  }

	
  [Test]
  public void EvaluateWaiConformityDebugLevelAWithHrefCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocReferenceValue.Command.Type = CommandType.Href;
    _bocReferenceValue.EvaluateWaiConformity ();
    // Assert.Succeed();
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithoutCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugLevelA();
    _bocReferenceValue.Command = null;
    _bocReferenceValue.EvaluateWaiConformity ();
    // Assert.Succeed();
  }
}

}
