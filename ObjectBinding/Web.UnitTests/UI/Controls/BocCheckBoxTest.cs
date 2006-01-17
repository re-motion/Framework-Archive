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
public class BocCheckBoxTest: BocTest
{
  private BocCheckBoxMock _bocCheckBox;

  public BocCheckBoxTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocCheckBox = new BocCheckBoxMock();
    _bocCheckBox.ID = "BocCheckBox";
    NamingContainer.Controls.Add (_bocCheckBox);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocCheckBox.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocCheckBox.AutoPostBack = NaBoolean.True;
    _bocCheckBox.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocCheckBox.AutoPostBack = NaBoolean.True;
    _bocCheckBox.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocCheckBox, WcagHelperMock.Control);
    Assert.AreEqual ("AutoPostBack", WcagHelperMock.Property);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocCheckBox.ReadOnly = NaBoolean.True;
    string[] actual = _bocCheckBox.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditMode()
  {
    _bocCheckBox.ReadOnly = NaBoolean.False;
    string[] actual = _bocCheckBox.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocCheckBox.CheckBox.ClientID, actual[0]);
  }
}

}
