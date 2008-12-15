// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Linq;
using Remotion.Security;
using Remotion.Security.Metadata;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  /// <summary>
  /// The <see cref="SecurityTestHelper"/> is used for providing security specific test objects, such as a stubbed <see cref="SecurityClient"/>.
  /// </summary>
  public class SecurityTestHelper
  {
    public SecurityClient CreatedStubbedSecurityClient<T> (params Enum[] accessTypes)
        where T: ISecurableObject
    {
      ArgumentUtility.CheckNotNull ("accessTypes", accessTypes);

      var principalStub = CreatePrincipalStub();

      return new SecurityClient (
          CreateSecurityProviderStub (typeof (T), principalStub, accessTypes),
          new PermissionReflector(),
          CreateUserProviderStub (principalStub),
          MockRepository.GenerateStub<IFunctionalSecurityStrategy>());
    }

    private ISecurityProvider CreateSecurityProviderStub (Type securableClassType, ISecurityPrincipal principal, Enum[] returnedAccessTypes)
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg<ISecurityContext>.Matches (sc => TypeUtility.GetType (sc.Class, true) == securableClassType),
                      Arg.Is (principal)))
          .Return (returnedAccessTypes.Select (accessType => AccessType.Get (accessType)).ToArray());
      
      return securityProviderStub;
    }

    private IUserProvider CreateUserProviderStub (ISecurityPrincipal principal)
    {
      var userProviderStub = MockRepository.GenerateStub<IUserProvider>();
      userProviderStub.Stub (stub => stub.GetUser()).Return (principal);
      
      return userProviderStub;
    }

    private static ISecurityPrincipal CreatePrincipalStub ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();

      principalStub.Stub (stub => stub.User).Return ("user");

      return principalStub;
    }
  }
}