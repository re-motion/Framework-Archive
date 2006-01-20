using System;

using NUnit.Framework;

using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.ObjectBinding.Web.UnitTests.Domain;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocEnumValueTest: BocTest
{
  private BocEnumValueMock _bocEnumValue;
  private TypeWithEnum _typeWithEnum;

  public BocEnumValueTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocEnumValue = new BocEnumValueMock();
    _bocEnumValue.ID = "BocEnumValue";
    NamingContainer.Controls.Add (_bocEnumValue);
    _typeWithEnum = new TypeWithEnum();
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocEnumValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocEnumValue.ListControlStyle.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithListControlStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocEnumValue.ListControlStyle.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocEnumValue, WcagHelperMock.Control);
    Assert.AreEqual ("ListControlStyle.AutoPostBack", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithListControlAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocEnumValue.ListControl.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocEnumValue, WcagHelperMock.Control);
    Assert.AreEqual ("ListControl.AutoPostBack", WcagHelperMock.Property);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocEnumValue.ReadOnly = NaBoolean.True;
    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAsDropDownList()
  {
    _bocEnumValue.ReadOnly = NaBoolean.False;
    _bocEnumValue.ListControlStyle.ControlType = ListControlType.DropDownList;
    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID, actual[0]);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAsListBox()
  {
    _bocEnumValue.ReadOnly = NaBoolean.False;
    _bocEnumValue.ListControlStyle.ControlType = ListControlType.ListBox;
    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID, actual[0]);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAsRadioButtonList()
  {
    _bocEnumValue.ReadOnly = NaBoolean.False;
    _bocEnumValue.ListControlStyle.ControlType = ListControlType.RadioButtonList;
    IBusinessObjectProperty property = _typeWithEnum.GetBusinessObjectProperty ("EnumValue");
    Assert.IsNotNull (property, "Could not find property 'EnumValue'.");
    Assert.IsTrue (
        typeof (IBusinessObjectEnumerationProperty).IsAssignableFrom (property.GetType()), 
        "Property 'EnumValue' of invalid type.");
    _bocEnumValue.Property = (IBusinessObjectEnumerationProperty) property;
    _bocEnumValue.RefreshEnumList();

    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (3, actual.Length);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID + "_0", actual[0]);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID + "_1", actual[1]);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID + "_2", actual[2]);
  }
}

}
