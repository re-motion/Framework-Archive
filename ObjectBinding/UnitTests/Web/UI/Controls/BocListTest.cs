using System;
using NUnit.Framework;
using Remotion.NullableValueTypes;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
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
    Invoker.InitRecursive();

    _bocList = new BocListMock();
    _bocList.ID = "BocList";
    NamingContainer.Controls.Add (_bocList);
  
    _businessObject = TypeWithReference.Create();

    _propertyReferenceList = (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceList");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = (IBusinessObject)_businessObject;
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
  public void GetTrackedClientIDsInEditModeWithoutRowEditModeActive()
  {
    _bocList.ReadOnly = NaBoolean.False;
    Assert.IsFalse (_bocList.IsRowEditModeActive);
    string[] actual = _bocList.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void SetValueToList()
  {
    IBusinessObject[] list = new IBusinessObject[] {(IBusinessObject) TypeWithString.Create()};
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
  public void LoadValueAndInterimTrueWithListAndDirty()
  {
    _businessObject.ReferenceList = new TypeWithReference[] {TypeWithReference.Create(), TypeWithReference.Create()};
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = null;
    _bocList.IsDirty = true;

    _bocList.LoadValue (true);
    Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsTrue (_bocList.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimTrueWithNullAndDirty()
  {
    _businessObject.ReferenceList = null;
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = new TypeWithReference[0];
    _bocList.IsDirty = true;

    _bocList.LoadValue (true);
    Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsTrue (_bocList.IsDirty);
  }   

  [Test]
  public void LoadValueAndInterimTrueWithListAndNotDirty()
  {
    _businessObject.ReferenceList = new TypeWithReference[] {TypeWithReference.Create(), TypeWithReference.Create()};
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = null;
    _bocList.IsDirty = false;

    _bocList.LoadValue (true);
    Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimTrueWithNullAndNotDirty()
  {
    _businessObject.ReferenceList = null;
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = new TypeWithReference[0];
    _bocList.IsDirty = false;

    _bocList.LoadValue (true);
    Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }   

  [Test]
  public void LoadValueAndInterimFalseWithListAndDirty()
  {
    _businessObject.ReferenceList = new TypeWithReference[] {TypeWithReference.Create(), TypeWithReference.Create()};
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = null;
    _bocList.IsDirty = true;

    _bocList.LoadValue (false);
    Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithNullAndDirty()
  {
    _businessObject.ReferenceList = null;
    _bocList.DataSource = _dataSource;
    _bocList.Property = _propertyReferenceList;
    _bocList.Value = new TypeWithReference[0];
    _bocList.IsDirty = true;

    _bocList.LoadValue (false);
    Assert.AreSame (_businessObject.ReferenceList, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }   

  [Test]
  public void LoadUnboundValueAndInterimTrueWithListAndDirty()
  {
    TypeWithReference[] value = new TypeWithReference[] {TypeWithReference.Create(), TypeWithReference.Create()};
    _bocList.DataSource = _dataSource;
    _bocList.Value = null;
    _bocList.IsDirty = true;

    _bocList.LoadUnboundValue (value, true);
    Assert.AreSame (value, _bocList.Value);
    Assert.IsTrue (_bocList.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimTrueWithNullAndDirty()
  {
    TypeWithReference[] value = null;
    _bocList.DataSource = _dataSource;
    _bocList.Value = new TypeWithReference[0];
    _bocList.IsDirty = true;

    _bocList.LoadUnboundValue (value, true);
    Assert.AreSame (value, _bocList.Value);
    Assert.IsTrue (_bocList.IsDirty);
  }   

  [Test]
  public void LoadUnboundValueAndInterimTrueWithListAndNotDirty()
  {
    TypeWithReference[] value = new TypeWithReference[] {TypeWithReference.Create(), TypeWithReference.Create()};
    _bocList.DataSource = _dataSource;
    _bocList.Value = null;
    _bocList.IsDirty = false;

    _bocList.LoadUnboundValue (value, true);
    Assert.AreSame (value, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimTrueWithNullAndNotDirty()
  {
    TypeWithReference[] value = null;
    _bocList.DataSource = _dataSource;
    _bocList.Value = new TypeWithReference[0];
    _bocList.IsDirty = false;

    _bocList.LoadUnboundValue (value, true);
    Assert.AreSame (value, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }   

  [Test]
  public void LoadUnboundValueAndInterimFalseWithListAndDirty()
  {
    TypeWithReference[] value = new TypeWithReference[] {TypeWithReference.Create(), TypeWithReference.Create()};
    _bocList.DataSource = _dataSource;
    _bocList.Value = null;
    _bocList.IsDirty = true;

    _bocList.LoadUnboundValue (value, false);
    Assert.AreSame (value, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithNullAndDirty()
  {
    TypeWithReference[] value = null;
    _bocList.DataSource = _dataSource;
    _bocList.Value = new TypeWithReference[0];
    _bocList.IsDirty = true;

    _bocList.LoadUnboundValue (value, false);
    Assert.AreSame (value, _bocList.Value);
    Assert.IsFalse (_bocList.IsDirty);
  }   
}

}
