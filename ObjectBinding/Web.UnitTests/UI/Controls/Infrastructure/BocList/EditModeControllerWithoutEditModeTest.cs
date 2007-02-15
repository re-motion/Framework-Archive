using System;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Rubicon.Globalization;

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
    Assert.IsFalse (Controller.IsRowEditModeActive);
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

  [Test]
  public void LoadViewStateWithNull ()
  {
    Invoker.InitRecursive();

    ControllerInvoker.LoadViewState (null);

    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.IsFalse (Controller.IsListEditModeActive);
  }

  [Test]
  public void EnsureEditModeRestored ()
  {
    Assert.IsFalse (Controller.IsRowEditModeActive);

    Controller.EnsureEditModeRestored (Columns);
    
    Assert.IsFalse (Controller.IsRowEditModeActive);
  }

  [Test]
  public void EnsureEditModeRestoredWithValueNull ()
  {
    Controller.OwnerControl.LoadUnboundValue (null, false);    
   
    Assert.IsFalse (Controller.IsRowEditModeActive);

    Controller.EnsureEditModeRestored (Columns);
    
    Assert.IsFalse (Controller.IsRowEditModeActive);
  }
}

}
