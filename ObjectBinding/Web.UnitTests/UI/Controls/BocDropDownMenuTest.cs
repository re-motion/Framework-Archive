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
public class BocDropDownMenuTest: BocTest
{
  private BocDropDownMenuMock _bocDropDownMenu;
  private TypeWithReference _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectReferenceProperty _propertyReferenceValue;

  public BocDropDownMenuTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocDropDownMenu = new BocDropDownMenuMock();
    _bocDropDownMenu.ID = "BocDropDownMenu";
    NamingContainer.Controls.Add (_bocDropDownMenu);
 
    _businessObject = new TypeWithReference();
    
    _propertyReferenceValue = (IBusinessObjectReferenceProperty) _businessObject.GetBusinessObjectProperty ("ReferenceValue");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocDropDownMenu.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocDropDownMenu.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDropDownMenu.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocDropDownMenu, WcagHelperMock.Control);
    Assert.IsNull (WcagHelperMock.Property);
  }


  [Test]
  public void SetValueToObject()
  {
    TypeWithReference referencedObject = new TypeWithReference();
    _bocDropDownMenu.Value = referencedObject;
    Assert.AreEqual (referencedObject, _bocDropDownMenu.Value);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocDropDownMenu.Value = null;
    Assert.AreEqual (null, _bocDropDownMenu.Value);
  }
    

  [Test]
  public void LoadValueBoundAndInterimTrueWithObject()
  {
    _businessObject.ReferenceValue = new TypeWithReference();
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Property = _propertyReferenceValue;
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (true);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueBoundAndInterimTrueWithObjectAndNoProperty()
  {
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (true);
    Assert.AreEqual (_businessObject, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueBoundAndInterimTrueWithNull()
  {
    _businessObject.ReferenceValue = null;
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Property = _propertyReferenceValue;
    _bocDropDownMenu.Value = new TypeWithReference();

    _bocDropDownMenu.LoadValue (true);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueBoundAndInterimFalseWithObject()
  {
    _businessObject.ReferenceValue = new TypeWithReference();
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Property = _propertyReferenceValue;
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (false);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
  }


  [Test]
  public void LoadValueBoundAndInterimFalseWithObjectAndNoProperty()
  {
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (false);
    Assert.AreEqual (_businessObject, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueBoundAndInterimFalseWithNull()
  {
    _businessObject.ReferenceValue = null;
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Property = _propertyReferenceValue;
    _bocDropDownMenu.Value = new TypeWithReference();

    _bocDropDownMenu.LoadValue (false);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueUnboundAndInterimTrueWithObject()
  {
    TypeWithReference value = new TypeWithReference();
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (value, true);
    Assert.AreEqual (value, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueUnboundAndInterimTrueWithNull()
  {
    TypeWithReference value = null;
    _bocDropDownMenu.Value = new TypeWithReference();

    _bocDropDownMenu.LoadValue (value, true);
    Assert.AreEqual (value, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueUnboundAndInterimFalseWithObject()
  {
    TypeWithReference value = new TypeWithReference();
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (value, false);
    Assert.AreEqual (value, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueUnboundAndInterimFalseWithNull()
  {
    TypeWithReference value = null;
    _bocDropDownMenu.Value = new TypeWithReference();

    _bocDropDownMenu.LoadValue (value, false);
    Assert.AreEqual (value, _bocDropDownMenu.Value);
  }
}

}
