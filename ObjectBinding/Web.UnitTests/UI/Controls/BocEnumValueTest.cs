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
public class BocEnumValueTest
{
  private BocEnumValueMock _bocEnumValue;

  public BocEnumValueTest()
  {
  }

  
  [SetUp]
  public virtual void SetUp()
  {
    _bocEnumValue = new BocEnumValueMock();
    _bocEnumValue.ID = "BocEnumValue";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocEnumValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocEnumValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocEnumValue.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocEnumValue.ListControlStyle.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocEnumValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocEnumValue.WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithListControlStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocEnumValue.ListControlStyle.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocEnumValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocEnumValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocEnumValue, _bocEnumValue.WcagHelperMock.Control);
    Assert.AreEqual ("ListControlStyle.AutoPostBack", _bocEnumValue.WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithListControlAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocEnumValue.ListControl.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocEnumValue.WcagHelperMock.HasWarning);
    Assert.AreEqual (1, _bocEnumValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocEnumValue, _bocEnumValue.WcagHelperMock.Control);
    Assert.AreEqual ("ListControl.AutoPostBack", _bocEnumValue.WcagHelperMock.Property);
  }
}

}
