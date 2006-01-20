using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.UI;
using Rubicon.NullableValueTypes;
using Rubicon.Web.Configuration;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocListTest: BocTest
{
  private BocListMock _bocList;

  public BocListTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocList = new BocListMock();
    _bocList.ID = "BocList";
    NamingContainer.Controls.Add (_bocList);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocList.ReadOnly = NaBoolean.True;
    string[] actual = _bocList.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeWithoutEditDetailsModeActive()
  {
    _bocList.ReadOnly = NaBoolean.False;
    Assert.IsFalse (_bocList.IsEditDetailsModeActive);
    string[] actual = _bocList.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  // EditDetailsMode currently not testable
  //  [Test]
  //  public void GetTrackedClientIDsInEditMode()
  //  {
  //    _bocList.ReadOnly = NaBoolean.False;
  //    string[] actual = _bocList.GetTrackedClientIDs();
  //    Assert.IsNotNull (actual);
  //    Assert.Ignore("Not implemented.");
  //  }
}

}
