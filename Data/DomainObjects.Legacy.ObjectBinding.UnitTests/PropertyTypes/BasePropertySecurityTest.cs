using System;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.ObjectBinding;
using Remotion.Security;
using StringProperty=Remotion.Data.DomainObjects.ObjectBinding.PropertyTypes.StringProperty;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.PropertyTypes
{
  [TestFixture]
  public class BasePropertySecurityTest
  {
    private MockRepository _mocks;
    private IObjectSecurityAdapter _mockObjectSecurityAdapter;
    private StringProperty _securableProperty;
    private StringProperty _nonSecurableProperty;
    private StringProperty _nonSecurablePropertyReadOnly;
    private SecurableSearchObject _securableObject;
    private TestSearchObject _nonSecurableObject;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockObjectSecurityAdapter = _mocks.CreateMock<IObjectSecurityAdapter> ();

      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), _mockObjectSecurityAdapter);
      _securableObject = new SecurableSearchObject (_mocks.CreateMock<IObjectSecurityStrategy> ());

      Type securableType = typeof (SecurableSearchObject);
      IBusinessObjectClass securableBusinessObjectClass = new SearchObjectClass (securableType);
      _securableProperty = new StringProperty (securableBusinessObjectClass, securableType.GetProperty ("StringProperty"), false, typeof (string), false, 200);

      Type nonSecurableType = typeof (TestSearchObject);
      IBusinessObjectClass nonSecurableBusinessObjectClass = new SearchObjectClass (securableType);
      _nonSecurablePropertyReadOnly = new StringProperty (nonSecurableBusinessObjectClass, nonSecurableType.GetProperty ("ReadOnlyStringProperty"), false, typeof (string), false, 200);
      _nonSecurableProperty = new StringProperty (nonSecurableBusinessObjectClass, nonSecurableType.GetProperty ("StringProperty"), false, typeof (string), false, 200);
      _nonSecurableObject = new TestSearchObject ();
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
    }

    [Test]
    public void IsAccessibleWithoutObjectSecurityProvider ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
      _mocks.ReplayAll ();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject);

      _mocks.VerifyAll ();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsAccessible ()
    {
      ExpectHasAccessOnGetAccessor (true);
      _mocks.ReplayAll ();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject);

      _mocks.VerifyAll ();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsNotAccessible ()
    {
      ExpectHasAccessOnGetAccessor (false);
      _mocks.ReplayAll ();

      bool isAccessible = _securableProperty.IsAccessible (_securableObject);

      _mocks.VerifyAll ();
      Assert.IsFalse (isAccessible);
    }

    [Test]
    public void IsAccessibleForNonSecurableType ()
    {
      _mocks.ReplayAll ();

      bool isAccessible = _nonSecurableProperty.IsAccessible (_nonSecurableObject);

      _mocks.VerifyAll ();
      Assert.IsTrue (isAccessible);
    }

    [Test]
    public void IsNotReadOnlyWithoutObjectSecurityProvider ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
      _mocks.ReplayAll ();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll ();
      Assert.IsFalse (isReadOnly);
    }

    [Test]
    public void IsReadOnly ()
    {
      ExpectHasAccessOnSetAccessor (false);
      _mocks.ReplayAll ();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll ();
      Assert.IsTrue (isReadOnly);
    }

    [Test]
    public void IsNotReadOnly ()
    {
      ExpectHasAccessOnSetAccessor (true);
      _mocks.ReplayAll ();

      bool isReadOnly = _securableProperty.IsReadOnly (_securableObject);

      _mocks.VerifyAll ();
      Assert.IsFalse (isReadOnly);
    }

    [Test]
    public void IsNotReadOnlyForNonSecurableType ()
    {
      _mocks.ReplayAll ();

      bool isReadOnly = _nonSecurableProperty.IsReadOnly (_nonSecurableObject);

      _mocks.VerifyAll ();
      Assert.IsFalse (isReadOnly);
    }

    [Test]
    public void IsReadOnlyForNonSecurableType ()
    {
      _mocks.ReplayAll ();

      bool isReadOnly = _nonSecurablePropertyReadOnly.IsReadOnly (_nonSecurableObject);

      _mocks.VerifyAll ();
      Assert.IsTrue (isReadOnly);
    }

    private void ExpectHasAccessOnGetAccessor (bool returnValue)
    {
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (_securableObject, _securableProperty.PropertyInfo.Name)).Return (returnValue);
    }

    private void ExpectHasAccessOnSetAccessor (bool returnValue)
    {
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnSetAccessor (_securableObject, _securableProperty.PropertyInfo.Name)).Return (returnValue);
    }
  }
}
