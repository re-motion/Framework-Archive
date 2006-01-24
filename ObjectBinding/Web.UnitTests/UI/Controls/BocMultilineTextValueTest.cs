using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UnitTests.Domain;
using Rubicon.Web.Configuration;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.UnitTests.UI.Controls
{

[TestFixture]
public class BocMultilineTextValueTest: BocTest
{
  private BocMultilineTextValueMock _bocMultilineTextValue;
  private TypeWithString _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectStringProperty _propertyStringArray;

  public BocMultilineTextValueTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocMultilineTextValue = new BocMultilineTextValueMock();
    _bocMultilineTextValue.ID = "BocMultilineTextValue";
    NamingContainer.Controls.Add (_bocMultilineTextValue);

    _businessObject = new TypeWithString();
    
    _propertyStringArray = (IBusinessObjectStringProperty) _businessObject.GetBusinessObjectProperty ("StringArray");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocMultilineTextValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocMultilineTextValue.TextBoxStyle.AutoPostBack = true;
    _bocMultilineTextValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocMultilineTextValue.TextBoxStyle.AutoPostBack = true;
    _bocMultilineTextValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocMultilineTextValue, WcagHelperMock.Control);
    Assert.AreEqual ("TextBoxStyle.AutoPostBack", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTextBoxAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocMultilineTextValue.TextBox.AutoPostBack = true;
    _bocMultilineTextValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocMultilineTextValue, WcagHelperMock.Control);
    Assert.AreEqual ("TextBox.AutoPostBack", WcagHelperMock.Property);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocMultilineTextValue.ReadOnly = NaBoolean.True;
    string[] actual = _bocMultilineTextValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditMode()
  {
    _bocMultilineTextValue.ReadOnly = NaBoolean.False;
    string[] actual = _bocMultilineTextValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocMultilineTextValue.TextBox.ClientID, actual[0]);
  }


  [Test]
  public void SetValueString()
  {
    string[] value = new string[] {"Foo", "Bar"};
    _bocMultilineTextValue.IsDirty = false;
    _bocMultilineTextValue.Value = value;
    Assert.AreEqual (value, _bocMultilineTextValue.Value);
    Assert.IsTrue (_bocMultilineTextValue.IsDirty);
  }
    
  [Test]
  public void SetValueNull()
  {
    _bocMultilineTextValue.IsDirty = false;
    _bocMultilineTextValue.Value = null;
    Assert.AreEqual (null, _bocMultilineTextValue.Value);
    Assert.IsTrue (_bocMultilineTextValue.IsDirty);
  }
    

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimTrue()
  {
    _businessObject.StringArray = new string[] {"Foo", "Bar"};
    _bocMultilineTextValue.DataSource = _dataSource;
    _bocMultilineTextValue.Property = _propertyStringArray;
    _bocMultilineTextValue.Value = null;
    _bocMultilineTextValue.IsDirty = true;

    _bocMultilineTextValue.LoadValue (true);
    Assert.AreEqual (null, _bocMultilineTextValue.Value);
    Assert.IsTrue (_bocMultilineTextValue.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithString()
  {
    _businessObject.StringArray = new string[] {"Foo", "Bar"};
    _bocMultilineTextValue.DataSource = _dataSource;
    _bocMultilineTextValue.Property = _propertyStringArray;
    _bocMultilineTextValue.Value = null;
    _bocMultilineTextValue.IsDirty = true;

    _bocMultilineTextValue.LoadValue (false);
    Assert.AreEqual (_businessObject.StringArray, _bocMultilineTextValue.Value);
    Assert.IsFalse (_bocMultilineTextValue.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithNull()
  {
    _businessObject.StringArray = null;
    _bocMultilineTextValue.DataSource = _dataSource;
    _bocMultilineTextValue.Property = _propertyStringArray;
    _bocMultilineTextValue.Value = new string[] {"Foo", "Bar"};
    _bocMultilineTextValue.IsDirty = true;

    _bocMultilineTextValue.LoadValue (false);
    Assert.AreEqual (_businessObject.StringArray, _bocMultilineTextValue.Value);
    Assert.IsFalse (_bocMultilineTextValue.IsDirty);
  }
}

}
