using System;
using System.Collections.Generic;
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Collections;
using Rubicon.Security.Configuration;
using Rubicon.Security.UnitTests.Core.SampleDomain;
using Rubicon.Development.UnitTesting;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Security.UnitTests.Core.SecurityStrategyTests
{
  [TestFixture]
  public class Test
  {
    private MockRepository _mocks;
    private ISecurityProvider _mockSecurityProvider;
    private ISecurityContextFactory _stubContextFactory;
    private IPrincipal _user;
    private SecurityContext _context;
    private SecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();
      _stubContextFactory = _mocks.CreateMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("user"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "tenant", new Dictionary<string, Enum> (), new Enum[0]);
      SetupResult.For (_stubContextFactory.CreateSecurityContext ()).Return (_context);

      _strategy = new SecurityStrategy (new NullCache<string, AccessType[]> (), new NullGlobalAccessTypeCacheProvider ());
    }

    [Test]
    public void HasAccess ()
    {
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return (new AccessType[] { AccessType.Get (GeneralAccessTypes.Edit) });
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Edit));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasNotAccess ()
    {
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return (new AccessType[] { AccessType.Get (GeneralAccessTypes.Edit) });
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Create));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasAccessWithMultipleAllowedAccessResults ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessTypes.Create),
          AccessType.Get (GeneralAccessTypes.Delete),
          AccessType.Get (GeneralAccessTypes.Read) };

      Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return (mockResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _user, AccessType.Get (GeneralAccessTypes.Read));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessTypes.Create),
          AccessType.Get (GeneralAccessTypes.Delete),
          AccessType.Get (GeneralAccessTypes.Read) };

      Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return (mockResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _user,
          AccessType.Get (GeneralAccessTypes.Delete), AccessType.Get (GeneralAccessTypes.Create));

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasNotAccessWithMultipleRequiredAccessTypes ()
    {
      AccessType[] mockResult = new AccessType[] { 
          AccessType.Get (GeneralAccessTypes.Create),
          AccessType.Get (GeneralAccessTypes.Delete),
          AccessType.Get (GeneralAccessTypes.Read) };

      Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return(mockResult);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _user,
          AccessType.Get (GeneralAccessTypes.Delete), AccessType.Get (GeneralAccessTypes.Find));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasMultipleAccessWithoutAllowedAccessResults ()
    {
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return (new AccessType[0]);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _user,
          AccessType.Get (GeneralAccessTypes.Find), AccessType.Get (GeneralAccessTypes.Edit), AccessType.Get (GeneralAccessTypes.Read));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void HasMultipleAccessWithNull ()
    {
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return (null);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubContextFactory, _mockSecurityProvider, _user,
          AccessType.Get (GeneralAccessTypes.Find), AccessType.Get (GeneralAccessTypes.Edit), AccessType.Get (GeneralAccessTypes.Read));

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void Serialization ()
    {
      SecurityStrategy strategy =
          new SecurityStrategy (new Cache<string, AccessType[]> (), SecurityConfiguration.Current.GlobalAccessTypeCacheProvider);
      AccessType[] accessTypes = new AccessType[] { AccessType.Get (GeneralAccessTypes.Find) };
      strategy.LocalCache.Add ("foo", accessTypes);

      SecurityStrategy deserializedStrategy = Serializer.SerializeAndDeserialize (strategy);
      Assert.AreNotSame (strategy, deserializedStrategy);
      Assert.AreSame (SecurityConfiguration.Current.GlobalAccessTypeCacheProvider, deserializedStrategy.GlobalCacheProvider);
      
      AccessType[] newAccessTypes;
      bool result = deserializedStrategy.LocalCache.TryGetValue ("foo", out newAccessTypes);
      Assert.IsTrue (result);
      Assert.That (newAccessTypes, Is.EquivalentTo (accessTypes));

    }
  }
}
