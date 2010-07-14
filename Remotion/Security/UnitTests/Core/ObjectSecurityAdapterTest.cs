// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Security.Principal;
using NUnit.Framework;
using Remotion.Reflection;
using Rhino.Mocks;
using Remotion.Security.Configuration;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.Configuration;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class ObjectSecurityAdapterTest
  {
    // types

    // static members

    // member fields

    private IObjectSecurityAdapter _securityAdapter;
    private MockRepository _mocks;
    private SecurableObject _securableObject;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private ISecurityProvider _mockSecurityProvider;
    private IPrincipalProvider _mockPrincipalProvider;
    private ISecurityPrincipal _userStub;
    private IPermissionProvider _mockPermissionProvider;
    private IMemberResolver _mockMemberResolver;
    private IPropertyInformation _mockPropertyInformation;

    // construction and disposing

    public ObjectSecurityAdapterTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new ObjectSecurityAdapter ();

      _mocks = new MockRepository ();

      _mockSecurityProvider = _mocks.StrictMock<ISecurityProvider> ();
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockPrincipalProvider = _mocks.StrictMock<IPrincipalProvider> ();
      _mockPermissionProvider = _mocks.StrictMock<IPermissionProvider> ();
      _mockMemberResolver = _mocks.StrictMock<IMemberResolver> ();
      _mockPropertyInformation = _mocks.StrictMock<IPropertyInformation>();
      
      _userStub = _mocks.Stub < ISecurityPrincipal>();
      SetupResult.For (_userStub.User).Return ("user");
      SetupResult.For (_mockPrincipalProvider.GetPrincipal ()).Return (_userStub);

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.PrincipalProvider = _mockPrincipalProvider;
      SecurityConfiguration.Current.PermissionProvider = _mockPermissionProvider;
      SecurityConfiguration.Current.MemberResolver = _mockMemberResolver;

      _mockObjectSecurityStrategy = _mocks.StrictMock<IObjectSecurityStrategy> ();
      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void HasAccessOnGetAccessor_AccessGranted ()
    {
      ExpectMemberResolverGetPropertyInformation ("Name", _mockPropertyInformation);
      ExpectGetRequiredPropertyReadPermissions (_mockPropertyInformation);
      
      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessor_AccessDenied ()
    {
      ExpectMemberResolverGetPropertyInformation ("Name", _mockPropertyInformation);
      ExpectGetRequiredPropertyReadPermissions (_mockPropertyInformation);
      ExpectExpectObjectSecurityStrategyHasAccess (false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessor_WithinSecurityFreeSeciton_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityAdapter.HasAccessOnGetAccessor (_securableObject, "Name");
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_AccessGranted ()
    {
      ExpectMemberResolverGetPropertyInformation ("Name", _mockPropertyInformation);
      ExpectGetRequiredPropertyWritePermissions (_mockPropertyInformation);
      ExpectExpectObjectSecurityStrategyHasAccess (true);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_AccessDenied ()
    {
      ExpectMemberResolverGetPropertyInformation ("Name", _mockPropertyInformation);
      ExpectGetRequiredPropertyWritePermissions (_mockPropertyInformation);
      ExpectExpectObjectSecurityStrategyHasAccess (false);
      _mocks.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, "Name");

      _mocks.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessor_WithinSecurityFreeSeciton_AccessGranted ()
    {
      _mocks.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityAdapter.HasAccessOnSetAccessor (_securableObject, "Name");
      }

      _mocks.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    private void ExpectExpectObjectSecurityStrategyHasAccess (bool accessAllowed)
    {
      AccessType[] accessTypes = new AccessType[] { AccessType.Get (TestAccessTypes.First) };
      Expect.Call (_mockObjectSecurityStrategy.HasAccess (_mockSecurityProvider, _userStub, accessTypes)).Return (accessAllowed);
    }

    private void ExpectGetRequiredPropertyReadPermissions (IPropertyInformation propertyInformation)
    {
      Expect.Call (_mockPermissionProvider.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyInformation)).Return (new Enum[] { TestAccessTypes.First });
    }

    public void ExpectMemberResolverGetPropertyInformation (string propertyName, IPropertyInformation returnValue)
    {
      Expect.Call (_mockMemberResolver.GetPropertyInformation (typeof (SecurableObject), propertyName)).Return (returnValue);
    }

    private void ExpectGetRequiredPropertyWritePermissions (IPropertyInformation propertyInformation)
    {
      Expect.Call (_mockPermissionProvider.GetRequiredPropertyWritePermissions (typeof (SecurableObject), propertyInformation)).Return (new Enum[] { TestAccessTypes.First });
    }
  }
}
