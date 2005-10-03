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
public class BocBooleanValueTest
{
  private BocBooleanValueMock _bocBooleanValue;

  public BocBooleanValueTest()
  {
  }

  
  [SetUp]
  public virtual void SetUp()
  {
    _bocBooleanValue = new BocBooleanValueMock();
    _bocBooleanValue.ID = "BocBooleanValue";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocBooleanValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocBooleanValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocBooleanValue.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocBooleanValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocBooleanValue.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocBooleanValue.WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocBooleanValue.EvaluateWaiConformity ();

    Assert.IsTrue (_bocBooleanValue.WcagHelperMock.HasError);
    Assert.AreEqual (1, _bocBooleanValue.WcagHelperMock.Priority);
    Assert.AreSame (_bocBooleanValue, _bocBooleanValue.WcagHelperMock.Control);
    Assert.IsNull (_bocBooleanValue.WcagHelperMock.Property);
  }
}

}
