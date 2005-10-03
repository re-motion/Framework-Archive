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
public class BocTreeViewTest
{
  private BocTreeViewMock _bocTreeView;

  public BocTreeViewTest()
  {
  }

  
  [SetUp]
  public virtual void SetUp()
  {
    _bocTreeView = new BocTreeViewMock();
    _bocTreeView.ID = "BocTreeView";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocTreeView.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocTreeView.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocTreeView.WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocTreeView.EvaluateWaiConformity ();
    
    Assert.IsFalse (_bocTreeView.WcagHelperMock.HasWarning);
    Assert.IsFalse (_bocTreeView.WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocTreeView.EvaluateWaiConformity ();

    Assert.IsTrue (_bocTreeView.WcagHelperMock.HasError);
    Assert.AreEqual (1, _bocTreeView.WcagHelperMock.Priority);
    Assert.AreSame (_bocTreeView, _bocTreeView.WcagHelperMock.Control);
    Assert.IsNull (_bocTreeView.WcagHelperMock.Property);
  }
}

}
