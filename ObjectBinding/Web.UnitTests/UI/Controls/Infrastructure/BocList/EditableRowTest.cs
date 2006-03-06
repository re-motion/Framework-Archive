using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.Utilities;

using Rubicon.Web.UnitTests.UI.Controls;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls.Infrastructure.BocList
{

[TestFixture]
public class RowEditingControllerTest
{
  // types

  // static members and constants

  // member fields
  private Rubicon.ObjectBinding.Web.UI.Controls.BocList _bocList;
  private RowEditingController _controller;

  // construction and disposing

  public RowEditingControllerTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _bocList = new Rubicon.ObjectBinding.Web.UI.Controls.BocList ();

    _controller = new RowEditingController (_bocList);
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreSame (_bocList, _controller.Owner);
  }

  [Test]
  public void ControlInit ()
  {
//    Assert.IsNotNull (_controller.EditControls);
//    Assert.IsNotNull (_controller.ValidationControls);
  }
}
}
