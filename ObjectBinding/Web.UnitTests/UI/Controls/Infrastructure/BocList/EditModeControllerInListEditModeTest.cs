using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Rubicon.ObjectBinding.Web.UnitTests.Domain;
using Rubicon.Utilities;
using Rubicon.Web;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls.Infrastructure.BocList
{

[TestFixture]
public class EditModeControllerTest
{
  // types

  // static members and constants

  // member fields

  private Rubicon.ObjectBinding.Web.UI.Controls.BocList _bocList;
  private EditModeController _controller;
  private ControlInvoker _invoker;

  // construction and disposing

  public EditModeControllerTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _bocList = new Rubicon.ObjectBinding.Web.UI.Controls.BocList ();
    _controller = new EditModeController (_bocList);
    //_invoker = new ControlInvoker (_controller);
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreSame (_bocList, _controller.Owner);
  }

  [Test]
  public void InitRecursive ()
  {
    //Assert.AreEqual (2, _controller.Controls.Count);
  }
}

}
