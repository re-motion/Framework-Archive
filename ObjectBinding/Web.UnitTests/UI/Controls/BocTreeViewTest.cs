using System;
using System.Collections;
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
public class BocTreeViewTest: BocTest
{
  private BocTreeViewMock _bocTreeView;
  private TypeWithReference _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectReferenceProperty _propertyReferenceValue;
  private IBusinessObjectReferenceProperty _propertyReferenceList;

  public BocTreeViewTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocTreeView = new BocTreeViewMock();
    _bocTreeView.ID = "BocTreeView";
    NamingContainer.Controls.Add (_bocTreeView);

    _businessObject = new TypeWithReference();
    
    _propertyReferenceValue = (IBusinessObjectReferenceProperty) _businessObject.GetBusinessObjectProperty ("ReferenceValue");
    _propertyReferenceList = (IBusinessObjectReferenceProperty) _businessObject.GetBusinessObjectProperty ("ReferenceList");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocTreeView.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocTreeView.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocTreeView.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocTreeView, WcagHelperMock.Control);
    Assert.IsNull (WcagHelperMock.Property);
  }

  
  // EditDetailsMode currently not testable
  //  [Test]
  //  public void GetTrackedClientIDsInEditMode()
  //  {
  //    _bocTreeView.ReadOnly = NaBoolean.False;
  //    string[] actual = _bocTreeView.GetTrackedClientIDs();
  //    Assert.IsNotNull (actual);
  //    Assert.Ignore("Not implemented.");
  //  }

  [Test]
  public void SetValueToList()
  {
    IBusinessObject[] list = new IBusinessObject[] {new TypeWithString()};
    _bocTreeView.Value = list;
    Assert.AreEqual (list, _bocTreeView.Value);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocTreeView.Value = null;
    Assert.AreEqual (null, _bocTreeView.Value);
  }
    

  [Test]
  public void LoadValueBoundAndInterimTrueWithObject()
  {
    _bocTreeView.DataSource = _dataSource;
    _bocTreeView.Value = null;

    _bocTreeView.LoadValue (true);
    IList actual = (IList) _bocTreeView.Value;
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Count);
    Assert.AreEqual (_businessObject, actual[0]);
  }

  [Test]
  public void LoadValueBoundAndInterimFalseWithObject()
  {
    _bocTreeView.DataSource = _dataSource;
    _bocTreeView.Value = null;

    _bocTreeView.LoadValue (false);
    IList actual = (IList) _bocTreeView.Value;
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Count);
    Assert.AreEqual (_businessObject, actual[0]);
  }

  [Test]
  public void LoadValueUnboundAndInterimTrueWithList()
  {
    TypeWithReference[] value = new TypeWithReference[] {new TypeWithReference(), new TypeWithReference()};
    _bocTreeView.Value = null;

    _bocTreeView.LoadValue (value, true);
    Assert.AreEqual (value, _bocTreeView.Value);
  }

  [Test]
  public void LoadValueUnboundAndInterimTrueWithNull()
  {
    TypeWithReference[] value = null;
    _bocTreeView.Value = new TypeWithReference[0];

    _bocTreeView.LoadValue (value, true);
    Assert.AreEqual (value, _bocTreeView.Value);
  }   

  [Test]
  public void LoadValueUnboundAndInterimFalseWithList()
  {
    TypeWithReference[] value = new TypeWithReference[] {new TypeWithReference(), new TypeWithReference()};
    _bocTreeView.Value = null;

    _bocTreeView.LoadValue (value, false);
    Assert.AreEqual (value, _bocTreeView.Value);
  }

  [Test]
  public void LoadValueUnboundAndInterimFalseWithNull()
  {
    TypeWithReference[] value = null;
    _bocTreeView.Value = new TypeWithReference[0];

    _bocTreeView.LoadValue (value, false);
    Assert.AreEqual (value, _bocTreeView.Value);
  }   

}

}
