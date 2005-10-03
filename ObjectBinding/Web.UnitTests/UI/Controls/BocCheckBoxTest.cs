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
public class BocCheckBoxTest
{
  private BocCheckBoxMock _bocCheckBox;

  public BocCheckBoxTest()
  {
  }

  
  [SetUp]
  public virtual void SetUp()
  {
    _bocCheckBox = new BocCheckBoxMock();
    _bocCheckBox.ID = "BocCheckBox";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocCheckBox.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocCheckBox.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocCheckBox.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocCheckBox.AutoPostBack = NaBoolean.True;
    _bocCheckBox.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocCheckBox.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocCheckBox.WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocCheckBox.AutoPostBack = NaBoolean.True;
    _bocCheckBox.EvaluateWaiConformity ();

    Assert.IsTrue (_bocCheckBox.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocCheckBox.WcagHelperMock.Priority);
    Assert.AreSame (_bocCheckBox, _bocCheckBox.WcagHelperMock.Control);
    Assert.AreEqual ("AutoPostBack", _bocCheckBox.WcagHelperMock.Property);
  }
}

}
