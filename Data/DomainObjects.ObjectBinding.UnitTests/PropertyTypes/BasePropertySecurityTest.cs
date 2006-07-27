using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.NullableValueTypes;
using Rubicon.Security;

using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.PropertyTypes
{
  [TestFixture]
  public class BasePropertySecurityTest
  {
    private MockRepository _mocks;
    private IObjectSecurityProvider _mockObjectSecurityProvider;
    private StringProperty _securableProperty;
    private StringProperty _nonSecurableProperty;
    private StringProperty _nonSecurablePropertyReadOnly;
    private SecurableSearchObject _securableObject;
    private TestSearchObject _nonSecurableObject;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockObjectSecurityProvider = _mocks.CreateMock<IObjectSecurityProvider> ();

      SecurityProviderRegistry.Instance.SetProvider<IObjectSecurityProvider> (_mockObjectSecurityProvider);
      _securableObject = new SecurableSearchObject (_mocks.CreateMock<IObjectSecurityStrategy> ());

      Type securableType = typeof (SecurableSearchObject);
      _securableProperty = new StringProperty (securableType.GetProperty ("StringProperty"), false, typeof (string), false, 200);

      Type nonSecurableType = typeof (TestSearchObject);
      _nonSecurablePropertyReadOnly = new StringProperty (nonSecurableType.GetProperty ("ReadOnlyStringProperty"), false, typeof (string), false, 200);
      _nonSecurableProperty = new StringProperty (nonSecurableType.GetProperty ("StringProperty"), false, typeof (string), false, 200);
      _nonSecurableObject = new TestSearchObject ();
 }

    [TearDown]
    public void TearDown ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IObjectSecurityProvider> (null);
    }

    [Test]
    public void IsAccessibleWithoutObjectSecurityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IObjectSecurityProvider> (null);
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
      SecurityProviderRegistry.Instance.SetProvider<IObjectSecurityProvider> (null);
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
      Expect.Call (_mockObjectSecurityProvider.HasAccessOnGetAccessor (_securableObject, _securableProperty.PropertyInfo.Name)).Return (returnValue);
    }

    private void ExpectHasAccessOnSetAccessor (bool returnValue)
    {
      Expect.Call (_mockObjectSecurityProvider.HasAccessOnSetAccessor (_securableObject, _securableProperty.PropertyInfo.Name)).Return (returnValue);
    }
  }
}
