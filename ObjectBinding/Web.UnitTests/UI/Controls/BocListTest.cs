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
public class BocListTest: BocTest
{
  private BocListMock _bocList;
  private TypeWithReference _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectReferenceProperty _propertyReferenceList;

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
  
    _businessObject = new TypeWithReference();
    
    _propertyReferenceList = (IBusinessObjectReferenceProperty) _businessObject.GetBusinessObjectProperty ("ReferenceList");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = _businessObject;
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

  [Test]
  public void SetValueToList()
  {
    IBusinessObject[] list = new IBusinessObject[] {new TypeWithString()};
    _bocList.IsDirty = false;
    _bocList.Value = list;
    Assert.AreEqual (list, _bocList.Value);
    Assert.IsTrue (_bocList.IsDirty);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocList.IsDirty = false;
    _bocList.Value = null;
    Assert.AreEqual (null, _bocList.Value);
    Assert.IsTrue (_bocList.IsDirty);
  }
 

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimTrueWithList()
  {
    _businessObject.ReferenceList = new TypeWithReference[] {new TypeWithReference(), new TypeWithReference()};
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = null;
    _bocList.IsDirty = true;

    _bocList.LoadValue (true);
    Assert.AreEqual (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimTrueWithNull()
  {
    _businessObject.ReferenceList = null;
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = new TypeWithReference[0];
    _bocList.IsDirty = true;

    _bocList.LoadValue (true);
    Assert.AreEqual (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }   

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithList()
  {
    _businessObject.ReferenceList = new TypeWithReference[] {new TypeWithReference(), new TypeWithReference()};
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = null;
    _bocList.IsDirty = true;

    _bocList.LoadValue (false);
    Assert.AreEqual (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }

  [Test]
  public void IsDirtyAfterLoadValueBoundAndInterimFalseWithNull()
  {
    _businessObject.ReferenceList = null;
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = new TypeWithReference[0];
    _bocList.IsDirty = true;

    _bocList.LoadValue (false);
    Assert.AreEqual (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }   
}

}
