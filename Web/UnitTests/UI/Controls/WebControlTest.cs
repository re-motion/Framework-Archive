using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.UnitTests.UI;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UnitTests.UI.Controls
{

public class WebControlTest
{
  private WcagHelperMock _wcagHelperMock;

  public WebControlTest()
  {
  }
  
  [SetUp]
  public virtual void SetUp()
  {
    _wcagHelperMock = new WcagHelperMock();
    WcagHelper.SetInstance (_wcagHelperMock);
  }

  [TearDown]
  public virtual void TearDown()
  {
  }

  protected WcagHelperMock WcagHelperMock
  {
    get { return _wcagHelperMock; }
  }
}

}
