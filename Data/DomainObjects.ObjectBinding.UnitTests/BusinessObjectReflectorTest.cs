using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests
{
  public abstract class StubBusinessObject
  {    
    public abstract IBusinessObjectWithIdentity ReferenceProperty { get; set;}
  }

  [TestFixture]
  public class BusinessObjectReflectorTest
  {
    private MockRepository _mocks;
    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectWithIdentity _mockBusinessObjectWithIdentity;
    private BusinessObjectReflector _businessObjectReflector;
    private IBusinessObjectReferenceProperty _referenceProperty;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();

      _mockBusinessObject = (IBusinessObject) _mocks.CreateMultiMock<StubBusinessObject> (typeof (IBusinessObject));
      _mockBusinessObjectWithIdentity = _mocks.CreateMock<IBusinessObjectWithIdentity> ();
     
      Type businessObjectType = typeof (StubBusinessObject);
      _referenceProperty = new ReferenceProperty (businessObjectType.GetProperty ("ReferenceProperty"), false, typeof (IBusinessObjectWithIdentity), false);

      _businessObjectReflector = new BusinessObjectReflector (_mockBusinessObject);
    }

    [Test]
    public void GetPropertyString ()
    {
      Expect.Call(_mockBusinessObject.GetProperty(_referenceProperty)).Return (_mockBusinessObjectWithIdentity);
      Expect.Call (_mockBusinessObjectWithIdentity.DisplayNameSafe).Return ("Value");
      _mocks.ReplayAll ();

      object actualValue = _businessObjectReflector.GetPropertyString (_referenceProperty, null);

      _mocks.VerifyAll ();
      Assert.AreEqual ("Value", actualValue);
    }
  }
}
