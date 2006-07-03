using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.ObjectBinding;
using Rubicon.Security;

namespace Rubicon.Core.UnitTests.ObjectBidning
{
  [TestFixture]
  public class BusinessObjectPropertyPathSecurityTest
  {
    private MockRepository _mocks;
    private IBusinessObjectProperty _mockProperty;
    private IBusinessObjectReferenceProperty _mockReferenceProperty;
    private IBusinessObjectClass _mockBusinessObjectClass;
    private IBusinessObject _mockBusinessObject;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository();
      _mockProperty = _mocks.CreateMock<IBusinessObjectProperty> ();
      _mockReferenceProperty = _mocks.CreateMock<IBusinessObjectReferenceProperty> ();
      _mockBusinessObjectClass = _mocks.CreateMock<IBusinessObjectClass> ();
      _mockBusinessObject = _mocks.CreateMock<IBusinessObject> ();
      SetupResult.For (_mockBusinessObject.BusinessObjectClass).Return (_mockBusinessObjectClass);
      SetupResult.For (_mockReferenceProperty.IsList).Return (false);
    }

    [Test]
    public void GetValue_FromAndSinglePropertyWithAccessGranted ()
    {
      object expectedObject = new object();
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (expectedObject);
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockProperty);
      object actualObject = path.GetValue (_mockBusinessObject, false, false);

      _mocks.VerifyAll ();
      Assert.AreSame (expectedObject, actualObject);
    }

    [Test]
    public void GetValue_FromCascadedPropertyWithAccessGranted ()
    {
      object expectedObject = new object ();
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockReferenceProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetProperty (_mockReferenceProperty)).Return (_mockBusinessObject);
        Expect.Call (_mockProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetProperty (_mockReferenceProperty)).Return (_mockBusinessObject);
        Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (expectedObject);
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockReferenceProperty, _mockProperty);
      object actualObject = path.GetValue (_mockBusinessObject, false, false);

      _mocks.VerifyAll ();
      Assert.AreSame (expectedObject, actualObject);
    }

    [Test]
    public void GetValue_FromSinglePropertyWithAccessDenied ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (false);
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockProperty);
      object actualObject = path.GetValue (_mockBusinessObject, false, false);

      _mocks.VerifyAll ();
      Assert.IsNull (actualObject);
    }

    [Test]
    public void GetValue_FromCascadedPropertyWithAccessDenied ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockReferenceProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (false);
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockReferenceProperty, _mockProperty);
      object actualObject = path.GetValue (_mockBusinessObject, false, false);

      _mocks.VerifyAll ();
      Assert.IsNull (actualObject);
    }

    [Test]
    public void GetValue_FromSinglePropertyWithPermissionDeniedException ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Throw (new PermissionDeniedException ());
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockProperty);
      object actualObject = path.GetValue (_mockBusinessObject, false, false);

      _mocks.VerifyAll ();
      Assert.IsNull (actualObject);
    }

    [Test]
    public void GetValue_FromCascadedPropertyWithPermissionDeniedException ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockReferenceProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetProperty (_mockReferenceProperty)).Throw (new PermissionDeniedException ());
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockReferenceProperty, _mockProperty);
      object actualObject = path.GetValue (_mockBusinessObject, false, false);

      _mocks.VerifyAll ();
      Assert.IsNull (actualObject);
    }

    [Test]
    public void GetPropertyString_FromAndSinglePropertyWithAccessGranted ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetPropertyString (_mockProperty, string.Empty)).Return ("value");
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockProperty);
      string actual = path.GetString (_mockBusinessObject, string.Empty);

      _mocks.VerifyAll ();
      Assert.AreEqual ("value", actual);
    }

    [Test]
    public void GetPropertyString_FromCascadedPropertyWithAccessGranted ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockReferenceProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetProperty (_mockReferenceProperty)).Return (_mockBusinessObject);
        Expect.Call (_mockProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetProperty (_mockReferenceProperty)).Return (_mockBusinessObject);
        Expect.Call (_mockBusinessObject.GetPropertyString (_mockProperty, string.Empty)).Return ("value");
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockReferenceProperty, _mockProperty);
      string actual = path.GetString (_mockBusinessObject, string.Empty);

      _mocks.VerifyAll ();
      Assert.AreEqual ("value", actual);
    }

    [Test]
    public void GetPropertyString_WithAccessDenied ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (false);
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockProperty);
      string actual = path.GetString (_mockBusinessObject, string.Empty);

      _mocks.VerifyAll ();
      Assert.AreEqual ("×", actual);
    }

    [Test]
    public void GetPropertyString_FromCascadedPropertyWithAccessDenied ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockReferenceProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (false);
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockReferenceProperty, _mockProperty);
      string actual = path.GetString (_mockBusinessObject, string.Empty);

      _mocks.VerifyAll ();
      Assert.AreEqual ("×", actual);
    }

    [Test]
    public void GetPropertyString_FromSinglePropertyWithPermissionDeniedException ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetPropertyString (_mockProperty, string.Empty)).Throw (new PermissionDeniedException ());
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockProperty);
      string actual = path.GetString (_mockBusinessObject, string.Empty);

      _mocks.VerifyAll ();
      Assert.AreEqual ("×", actual);
    }

    [Test]
    public void GetPropertyString_FromCascadedPropertyWithPermissionDeniedException ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockReferenceProperty.IsAccessible (_mockBusinessObjectClass, _mockBusinessObject)).Return (true);
        Expect.Call (_mockBusinessObject.GetProperty (_mockReferenceProperty)).Throw (new PermissionDeniedException ());
      }
      _mocks.ReplayAll ();

      BusinessObjectPropertyPath path = new TestBusinessObjectPropertyPath (_mockReferenceProperty, _mockProperty);
      string actual = path.GetString (_mockBusinessObject, string.Empty);

      _mocks.VerifyAll ();
      Assert.AreEqual ("×", actual);
    }
  }
}