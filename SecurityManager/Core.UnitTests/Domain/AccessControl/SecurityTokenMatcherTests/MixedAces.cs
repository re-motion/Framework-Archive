// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class MixedAces : SecurityTokenMatcherTestBase
  {
    [Test]
    public void AceForOwningTenantAndAbstractRole_DoesNotMatch ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = TestHelper.CreateAceWithOwningTenant ();
      entry.SpecificAbstractRole = TestHelper.CreateTestAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (user, tenant);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void AceForPositionFromOwningGroupAndAbstractRole_DoesNotMatch ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Position managerPosition = TestHelper.CreatePosition ("Manager");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      Role role = TestHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = TestHelper.CreateAceWithPositionAndGroupCondition (managerPosition, GroupCondition.OwningGroup);
      entry.SpecificAbstractRole = TestHelper.CreateTestAbstractRole ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, group);

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}
