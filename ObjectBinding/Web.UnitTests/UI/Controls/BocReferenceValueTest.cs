using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocReferenceValueTest: BocTest
{
  private BocReferenceValueMock _bocReferenceValue;

  public BocReferenceValueTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocReferenceValue = new BocReferenceValueMock();
    _bocReferenceValue.ID = "BocReferenceValue";
    _bocReferenceValue.ShowOptionsMenu = false;
    _bocReferenceValue.Command.Type = CommandType.None;
    _bocReferenceValue.Command.Show = CommandShow.Always;
    _bocReferenceValue.InternalValue = Guid.Empty.ToString();
    NamingContainer.Controls.Add (_bocReferenceValue);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocReferenceValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.DropDownListStyle.AutoPostBack = true;
    _bocReferenceValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocReferenceValue, WcagHelperMock.Control);
    Assert.AreEqual ("DropDownListStyle.AutoPostBack", WcagHelperMock.Property);
  }


	[Test]
  public void EvaluateWaiConformityDebugExceptionLevelAWithShowOptionsMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocReferenceValue, WcagHelperMock.Control);
    Assert.AreEqual ("ShowOptionsMenu", WcagHelperMock.Property);
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
  public void EvaluateWaiConformityDebugLevelAWithEventCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command.Type = CommandType.Event;
    _bocReferenceValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocReferenceValue, WcagHelperMock.Control);
    Assert.AreEqual ("Command", WcagHelperMock.Property);
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
  public void EvaluateWaiConformityDebugLevelAWithWxeFunctionCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    _bocReferenceValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocReferenceValue, WcagHelperMock.Control);
    Assert.AreEqual ("Command", WcagHelperMock.Property);
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
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command.Type = CommandType.Href;
    _bocReferenceValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithoutCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command = null;
    _bocReferenceValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocReferenceValue.ReadOnly = NaBoolean.True;
    string[] actual = _bocReferenceValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditMode()
  {
    _bocReferenceValue.ReadOnly = NaBoolean.False;
    string[] actual = _bocReferenceValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocReferenceValue.DropDownList.ClientID, actual[0]);
  }
}

}
