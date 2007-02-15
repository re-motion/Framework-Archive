using System;
using System.Web.UI;
using NUnit.Framework;
using Rubicon.Web.UI;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.UI;
using Rubicon.Web.UnitTests.UI.Controls;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

public class BocTest
{
  private WcagHelperMock _wcagHelperMock;
  private Page _page;
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

    _page = new Page();

    _namingContainer = new NamingContainerMock();
    _namingContainer.ID = "NamingContainer";
    _page.Controls.Add (_namingContainer);

    _invoker = new ControlInvoker (_namingContainer);
  }

  [TearDown]
  public virtual void TearDown()
  {
    WcagHelper.SetInstance (new WcagHelperMock ());
    HttpContextHelper.SetCurrent (null);
  }

  protected WcagHelperMock WcagHelperMock
  {
    get { return _wcagHelperMock; }
  }

  public Page Page
  {
    get { return _page; }
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
