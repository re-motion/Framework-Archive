using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Rubicon.ObjectBinding;
using Rubicon.Security;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectTest
  {
    private MockRepository _mocks;
    private IObjectSecurityProvider _mockObjectSecurityProvider;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private ClientTransaction _transaction;
    private SecurableOrder _securableOder;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockObjectSecurityProvider = _mocks.CreateMock<IObjectSecurityProvider> ();
      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy> ();

      _transaction = new ClientTransaction ();
      _securableOder = new SecurableOrder (_transaction, _mockObjectSecurityStrategy);
      _securableOder.SetDisplayName ("Value");
      SecurityProviderRegistry.Instance.SetProvider<IObjectSecurityProvider> (_mockObjectSecurityProvider);

      
      //_securableObject = new SecurableSearchObject (_mocks.CreateMock<IObjectSecurityStrategy> ());

      //Type securableType = typeof (SecurableSearchObject);
      //_securableProperty = new StringProperty (securableType.GetProperty ("StringProperty"), false, typeof (string), false, 200);

      //Type nonSecurableType = typeof (TestSearchObject);
      //_nonSecurablePropertyReadOnly = new StringProperty (nonSecurableType.GetProperty ("ReadOnlyStringProperty"), false, typeof (string), false, 200);
      //_nonSecurableProperty = new StringProperty (nonSecurableType.GetProperty ("StringProperty"), false, typeof (string), false, 200);
      //_nonSecurableObject = new TestSearchObject ();
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IObjectSecurityProvider> (null);
    }

    [Test]
    public void GetDisplayNameSafe_WithAccessGranted ()
    {
      ExpectHasAccessOnGetAccessor (true);
      _mocks.ReplayAll ();

      string actual = ((IBusinessObjectWithIdentity) _securableOder).DisplayNameSafe;

      _mocks.VerifyAll ();
      Assert.AreEqual ("Value", actual);
    }

    [Test]
    public void GetDisplayNameSafe_WithAccessDenied ()
    {
      ExpectHasAccessOnGetAccessor (false);
      _mocks.ReplayAll ();

      string actual = ((IBusinessObjectWithIdentity) _securableOder).DisplayNameSafe;

      _mocks.VerifyAll ();
      Assert.AreEqual ("×", actual);
    }

    private void ExpectHasAccessOnGetAccessor (bool returnValue)
    {
      Expect.Call (_mockObjectSecurityProvider.HasAccessOnGetAccessor (_securableOder, "DisplayName")).Return (returnValue);
    }
  }
}