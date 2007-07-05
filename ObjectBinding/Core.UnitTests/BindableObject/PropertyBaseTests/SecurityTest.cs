using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;
using Rubicon.Security;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.PropertyBaseTests
{
  [TestFixture]
  [Ignore]
  public class SecurityTest : TestBase
  {
    private MockRepository _mocks;
    private BindableObjectProvider _businessObjectProvider;
    private IObjectSecurityAdapter _mockObjectSecurityAdapter;
    private IBusinessObjectProperty _securableProperty;
    private IBusinessObjectProperty _nonSecurableProperty;
    private IBusinessObjectProperty _nonSecurablePropertyReadOnly;
    private IBusinessObject _securableObject;
    private IBusinessObject _nonSecurableObject;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository();
      _businessObjectProvider = new BindableObjectProvider();
      _mockObjectSecurityAdapter = _mocks.CreateMock<IObjectSecurityAdapter>();

      SecurityAdapterRegistry.Instance.SetAdapter<IObjectSecurityAdapter> (_mockObjectSecurityAdapter);

      _securableObject = (IBusinessObject) ObjectFactory.Create<SecurableClassWithReferenceType<SimpleReferenceType>>()
                                               .With (_mocks.CreateMock<IObjectSecurityStrategy>());
      TypeFactory.InitializeMixedInstance (_securableObject);

      _nonSecurableObject = (IBusinessObject) ObjectFactory.Create<ClassWithReferenceType<SimpleReferenceType>>().With();

      _securableProperty = CreateProperty ("Scalar");
      _nonSecurablePropertyReadOnly = CreateProperty ("ReadOnlyScalar");
      _nonSecurableProperty = CreateProperty ("Scalar");
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter<IObjectSecurityAdapter> (null);
    }

    [Test]
    public void IsAccessibleWithoutObjectSecurityProvider ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter<IObjectSecurityAdapter> (null);
      _mocks.ReplayAll();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsAccessible ()
    {
      ExpectHasAccessOnGetAccessor (true);
      _mocks.ReplayAll();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsNotAccessible ()
    {
      ExpectHasAccessOnGetAccessor (false);
      _mocks.ReplayAll();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject.BusinessObjectClass, _securableObject);

      _mocks.VerifyAll();
      Assert.IsFalse (isAccessible);
    }

    [Test]
    public void IsAccessibleForNonSecurableType ()
    {
      _mocks.ReplayAll();

      bool isAccessible = _nonSecurableProperty.IsAccessible (_nonSecurableObject.BusinessObjectClass, _nonSecurableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsNotReadOnlyWithoutObjectSecurityProvider ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter<IObjectSecurityAdapter> (null);
      _mocks.ReplayAll();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll();
      Assert.IsFalse (isReadOnly);
    }

    [Test]
    public void IsReadOnly ()
    {
      ExpectHasAccessOnSetAccessor (false);
      _mocks.ReplayAll();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isReadOnly);
    }

    [Test]
    public void IsNotReadOnly ()
    {
      ExpectHasAccessOnSetAccessor (true);
      _mocks.ReplayAll();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll();
      Assert.IsFalse (isReadOnly);
    }

    [Test]
    public void IsNotReadOnlyForNonSecurableType ()
    {
      _mocks.ReplayAll();

      bool isReadOnly = _nonSecurableProperty.IsReadOnly (_nonSecurableObject);

      _mocks.VerifyAll();
      Assert.IsFalse (isReadOnly);
    }

    [Test]
    public void IsReadOnlyForNonSecurableType ()
    {
      _mocks.ReplayAll();

      bool isReadOnly = _nonSecurablePropertyReadOnly.IsReadOnly (_nonSecurableObject);

      _mocks.VerifyAll();
      Assert.IsTrue (isReadOnly);
    }

    private void ExpectHasAccessOnGetAccessor (bool returnValue)
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnGetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableProperty).PropertyInfo.Name)).Return (returnValue);
    }

    private void ExpectHasAccessOnSetAccessor (bool returnValue)
    {
      Expect.Call (
          _mockObjectSecurityAdapter.HasAccessOnSetAccessor (
              (ISecurableObject) _securableObject, ((StubPropertyBase) _securableProperty).PropertyInfo.Name)).Return (returnValue);
    }

    private StubPropertyBase CreateProperty (string propertyName)
    {
      return new StubPropertyBase (
          new PropertyBase.Parameters (
              _businessObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), propertyName), null, false));
    }
  }
}