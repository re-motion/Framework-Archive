using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.UnitTests.UI;
using Rubicon.Web.UnitTests.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

public class BocTest
{
  private WcagHelperMock _wcagHelperMock;
  private NamingContainerMock _namingContainer;
  private ControlInvoker _invoker;

  public BocTest()
  {
  }
  
  [SetUp]
  public virtual void SetUp()
  {
    _wcagHelperMock = new WcagHelperMock();
    WcagHelper.SetInstance (_wcagHelperMock);

    _namingContainer = new NamingContainerMock();
    _namingContainer.ID = "NamingContainer";

    _invoker = new ControlInvoker (_namingContainer);
  }

  [TearDown]
  public virtual void TearDown()
  {
  }

  protected WcagHelperMock WcagHelperMock
  {
    get { return _wcagHelperMock; }
  }

  public NamingContainerMock NamingContainer
  {
    get { return _namingContainer; }
  }

  public ControlInvoker Invoker
  {
    get { return _invoker; }
  }
}

}
