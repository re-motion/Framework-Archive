using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.Globalization;
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
public class EditModeControllerWithoutEditModeTest : EditModeControllerTestBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public EditModeControllerWithoutEditModeTest ()
  {
  }

  // methods and properties

  [Test]
  public void Initialize ()
  {
    Assert.AreSame (BocList, Controller.OwnerControl);
    Assert.IsFalse (Controller.IsEditDetailsModeActive);
    Assert.IsFalse (Controller.IsListEditModeActive);
  }

  [Test]
  public void InitRecursive()
  {
    Invoker.InitRecursive();

    Assert.AreEqual (0, Controller.Controls.Count);
  }

  [Test]
  public void CreateValidators ()
  {
    Invoker.InitRecursive();

    BaseValidator[] validators = Controller.CreateValidators (NullResourceManager.Instance);
    
    Assert.IsNotNull (validators);
    Assert.AreEqual (0, validators.Length);
  }

  [Test]
  public void Validate ()
  {
    Invoker.InitRecursive();
    Invoker.LoadRecursive();

    Assert.IsTrue (Controller.Validate());
  }

  [Test]
  public void IsRequired ()
  {
    Invoker.InitRecursive();
    Assert.IsFalse (Controller.IsRequired (0));
    Assert.IsFalse (Controller.IsRequired (1));
  }

  [Test]
  public void IsDirty ()
  {
    Invoker.InitRecursive();
    Assert.IsFalse (Controller.IsDirty());
  }

  [Test]
  public void GetTrackedIDs ()
  {
    Invoker.InitRecursive();

    Assert.AreEqual (new string[0], Controller.GetTrackedClientIDs());
  }

  [Test]
  public void SaveAndLoadViewState ()
  {
    Invoker.InitRecursive();

    object viewState = ControllerInvoker.SaveViewState();
    Assert.IsNotNull (viewState);
    ControllerInvoker.LoadViewState (viewState);
  }
}

}
