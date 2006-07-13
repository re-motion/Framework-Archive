using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.NullableValueTypes;
using Rubicon.Security;

using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.PropertyTypes
{
  [TestFixture]
  public class BasePropertySecurityTest
  {
    private Mockery _mocks;
    private IObjectSecurityProvider _mockObjectSecurityProvider;
    private StringProperty _property;
    private SecurableSearchObject _securableSearchObject;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _mockObjectSecurityProvider = _mocks.NewMock<IObjectSecurityProvider> ();

      SecurityProviderRegistry.Instance.SetProvider<IObjectSecurityProvider> (_mockObjectSecurityProvider);
      _securableSearchObject = new SecurableSearchObject (_mocks.NewMock<IObjectSecurityStrategy> ());

      Type domainObjectType = typeof (SecurableSearchObject);
      PropertyInfo stringPropertyInfo = domainObjectType.GetProperty ("StringProperty");
      _property = new StringProperty (stringPropertyInfo, false, typeof (string), false, new NaInt32 (200));
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
      Assert.AreEqual (true, _property.IsAccessible (_securableSearchObject));
    }

    [Test]
    public void IsAccessible ()
    {
      Expect.Once.On (_mockObjectSecurityProvider)
          .Method ("HasAccessOnGetAccessor")
          .With (_securableSearchObject, _property.PropertyInfo.Name)
          .Will (Return.Value (true));

      Assert.AreEqual (true, _property.IsAccessible (_securableSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsNotAccessible ()
    {
      Expect.Once.On (_mockObjectSecurityProvider)
          .Method ("HasAccessOnGetAccessor")
          .With (_securableSearchObject, _property.PropertyInfo.Name)
          .Will (Return.Value (false));

      Assert.AreEqual (false, _property.IsAccessible (_securableSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsAccessibleForNonSecurableType ()
    {
      Expect.Never.On (_mockObjectSecurityProvider);

      Type domainObjectType = typeof (TestSearchObject);
      PropertyInfo stringPropertyInfo = domainObjectType.GetProperty ("StringProperty");
      BaseProperty property = new StringProperty (stringPropertyInfo, false, typeof (string), false, new NaInt32 (200));

      TestSearchObject testSearchObject = new TestSearchObject ();
      Assert.AreEqual (true, property.IsAccessible (testSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsNotReadOnlyWithoutObjectSecurityProvider ()
    {
      SecurityProviderRegistry.Instance.SetProvider<IObjectSecurityProvider> (null);
      Assert.AreEqual (false, _property.IsReadOnly (_securableSearchObject));
    }

    [Test]
    public void IsReadOnly ()
    {
      Expect.Once.On (_mockObjectSecurityProvider)
          .Method ("HasAccessOnSetAccessor")
          .With (_securableSearchObject, _property.PropertyInfo.Name)
          .Will (Return.Value (false));

      Assert.AreEqual (true, _property.IsReadOnly (_securableSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsNotReadOnly ()
    {
      Expect.Once.On (_mockObjectSecurityProvider)
          .Method ("HasAccessOnSetAccessor")
          .With (_securableSearchObject, _property.PropertyInfo.Name)
          .Will (Return.Value (true));

      Assert.AreEqual (false, _property.IsReadOnly (_securableSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsNotReadOnlyForNonSecurableType ()
    {
      Expect.Never.On (_mockObjectSecurityProvider);

      Type domainObjectType = typeof (TestSearchObject);
      PropertyInfo stringPropertyInfo = domainObjectType.GetProperty ("StringProperty");
      BaseProperty property = new StringProperty (stringPropertyInfo, false, typeof (string), false, new NaInt32 (200));

      TestSearchObject testSearchObject = new TestSearchObject ();
      Assert.AreEqual (false, property.IsReadOnly (testSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsReadOnlyForNonSecurableType ()
    {
      Expect.Never.On (_mockObjectSecurityProvider);

      Type domainObjectType = typeof (TestSearchObject);
      PropertyInfo stringPropertyInfo = domainObjectType.GetProperty ("ReadOnlyStringProperty");
      BaseProperty property = new StringProperty (stringPropertyInfo, false, typeof (string), false, new NaInt32 (200));

      TestSearchObject testSearchObject = new TestSearchObject ();
      Assert.AreEqual (true, property.IsReadOnly (testSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
