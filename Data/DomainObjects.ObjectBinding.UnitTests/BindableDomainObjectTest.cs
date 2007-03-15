using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Rubicon.ObjectBinding;
using Rubicon.Security;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectTest : DatabaseTest
  {
    private MockRepository _mocks;
    private IObjectSecurityAdapter _mockObjectSecurityAdapter;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private ClientTransaction _transaction;
    private SecurableOrder _securableOder;

    public override void SetUp ()
    {
      base.SetUp ();
      _mocks = new MockRepository ();
      _mockObjectSecurityAdapter = _mocks.CreateMock<IObjectSecurityAdapter> ();
      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy> ();

      _transaction = new ClientTransaction ();
      _securableOder = new SecurableOrder (_transaction, _mockObjectSecurityStrategy);
      _securableOder.SetDisplayName ("Value");
      SecurityAdapterRegistry.Instance.SetAdapter<IObjectSecurityAdapter> (_mockObjectSecurityAdapter);

      
      //_securableObject = new SecurableSearchObject (_mocks.CreateMock<IObjectSecurityStrategy> ());

      //Type securableType = typeof (SecurableSearchObject);
      //_securableProperty = new StringProperty (securableType.GetProperty ("StringProperty"), false, typeof (string), false, 200);

      //Type nonSecurableType = typeof (TestSearchObject);
      //_nonSecurablePropertyReadOnly = new StringProperty (nonSecurableType.GetProperty ("ReadOnlyStringProperty"), false, typeof (string), false, 200);
      //_nonSecurableProperty = new StringProperty (nonSecurableType.GetProperty ("StringProperty"), false, typeof (string), false, 200);
      //_nonSecurableObject = new TestSearchObject ();
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityAdapterRegistry.Instance.SetAdapter<IObjectSecurityAdapter> (null);
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
      Expect.Call (_mockObjectSecurityAdapter.HasAccessOnGetAccessor (_securableOder, "DisplayName")).Return (returnValue);
    }
  }
}