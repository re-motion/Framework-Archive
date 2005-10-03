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
public class BocDropDownMenuTest
{
  private BocDropDownMenuMock _bocDropDownMenu;

  public BocDropDownMenuTest()
  {
  }

  
  [SetUp]
  public virtual void SetUp()
  {
    _bocDropDownMenu = new BocDropDownMenuMock();
    _bocDropDownMenu.ID = "BocDropDownMenu";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocDropDownMenu.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocDropDownMenu.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocDropDownMenu.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocDropDownMenu.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocDropDownMenu.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocDropDownMenu.WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDropDownMenu.EvaluateWaiConformity ();

    Assert.IsTrue (_bocDropDownMenu.WcagHelperMock.HasError);
    Assert.AreEqual (1, _bocDropDownMenu.WcagHelperMock.Priority);
    Assert.AreSame (_bocDropDownMenu, _bocDropDownMenu.WcagHelperMock.Control);
    Assert.IsNull (_bocDropDownMenu.WcagHelperMock.Property);
  }
}

}
