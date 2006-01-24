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
public class BocTextValueTest: BocTest
{
  private BocTextValueMock _bocTextValue;
  private TypeWithString _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectStringProperty _propertyStringValue;

  public BocTextValueTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocTextValue = new BocTextValueMock();
    _bocTextValue.ID = "BocTextValue";
    NamingContainer.Controls.Add (_bocTextValue);

    _businessObject = new TypeWithString();
    
    _propertyStringValue = (IBusinessObjectStringProperty) _businessObject.GetBusinessObjectProperty ("StringValue");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocTextValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocTextValue.TextBoxStyle.AutoPostBack = true;
    _bocTextValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTextBoxStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocTextValue.TextBoxStyle.AutoPostBack = true;
    _bocTextValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocTextValue, WcagHelperMock.Control);
    Assert.AreEqual ("TextBoxStyle.AutoPostBack", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithTextBoxAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocTextValue.TextBox.AutoPostBack = true;
    _bocTextValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocTextValue, WcagHelperMock.Control);
    Assert.AreEqual ("TextBox.AutoPostBack", WcagHelperMock.Property);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocTextValue.ReadOnly = NaBoolean.True;
    string[] actual = _bocTextValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditMode()
  {
    _bocTextValue.ReadOnly = NaBoolean.False;
    string[] actual = _bocTextValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocTextValue.TextBox.ClientID, actual[0]);
  }


  [Test]
  public void SetValueToString()
  {
    string value = "Foo Bar";
    _bocTextValue.IsDirty = false;
    _bocTextValue.Value = value;
    Assert.AreEqual (value, _bocTextValue.Value);
    Assert.IsTrue (_bocTextValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocTextValue.IsDirty = false;
    _bocTextValue.Value = null;
    Assert.AreEqual (null, _bocTextValue.Value);
    Assert.IsTrue (_bocTextValue.IsDirty);
  }
    

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimTrue()
  {
    _businessObject.StringValue = "Foo Bar";
    _bocTextValue.DataSource = _dataSource;
    _bocTextValue.Property = _propertyStringValue;
    _bocTextValue.Value = null;
    _bocTextValue.IsDirty = true;

    _bocTextValue.LoadValue (true);
    Assert.AreEqual (null, _bocTextValue.Value);
    Assert.IsTrue (_bocTextValue.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithString()
  {
    _businessObject.StringValue = "Foo Bar";
    _bocTextValue.DataSource = _dataSource;
    _bocTextValue.Property = _propertyStringValue;
    _bocTextValue.Value = null;
    _bocTextValue.IsDirty = true;

    _bocTextValue.LoadValue (false);
    Assert.AreEqual (_businessObject.StringValue, _bocTextValue.Value);
    Assert.IsFalse (_bocTextValue.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithNull()
  {
    _businessObject.StringValue = null;
    _bocTextValue.DataSource = _dataSource;
    _bocTextValue.Property = _propertyStringValue;
    _bocTextValue.Value = "Foo Bar";
    _bocTextValue.IsDirty = true;

    _bocTextValue.LoadValue (false);
    Assert.AreEqual (_businessObject.StringValue, _bocTextValue.Value);
    Assert.IsFalse (_bocTextValue.IsDirty);
  }
}

}
