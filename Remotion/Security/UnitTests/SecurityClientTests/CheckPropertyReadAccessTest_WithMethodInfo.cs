// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CheckPropertyReadAccessTest_WithMethodInfo
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private IMethodInformation _methodInformation;
    private MethodInfo _methodInfo;
    private PropertyInfo _propertyInfo;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTestHelper ();
      _securityClient = _testHelper.CreateSecurityClient();
      _methodInformation = MockRepository.GenerateMock<IMethodInformation>();
      _propertyInfo = typeof (SecurableObject).GetProperty ("IsVisible");
      _methodInfo = _propertyInfo.GetGetMethod();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyReadAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyReadAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, TestAccessTypes.First);
      _testHelper.ReplayAll();

      using (SecurityFreeSection.Activate())
      {
        _securityClient.CheckPropertyReadAccess (_testHelper.SecurableObject, _methodInfo);
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessType ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Read, true);
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyReadAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithDefaultAccessType_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Read, false);
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyReadAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessTypeAndWithinSecurityFreeSection ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation);
      _testHelper.ReplayAll();

      using (SecurityFreeSection.Activate())
      {
        _securityClient.CheckPropertyReadAccess (_testHelper.SecurableObject, _methodInfo);
      }

      _testHelper.VerifyAll();
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The securableObject did not return an IObjectSecurityStrategy.")]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, TestAccessTypes.First);
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyReadAccess (new SecurableObject (null), _methodInfo);

      _testHelper.VerifyAll();
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, null);
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyReadAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll();
    }

#if !DEBUG
    [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, null);
      _testHelper.ReplayAll();

      using (SecurityFreeSection.Activate())
      {
        _securityClient.CheckPropertyReadAccess (_testHelper.SecurableObject, _methodInfo);
      }

      _testHelper.VerifyAll();
    }
  }
}