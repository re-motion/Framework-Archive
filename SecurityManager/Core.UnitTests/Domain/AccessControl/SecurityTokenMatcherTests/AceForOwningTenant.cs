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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceForOwningTenant : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();

      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithOwningTenant();

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.OwningTenant));
      Assert.That (_ace.TenantHierarchyCondition, Is.EqualTo (TenantHierarchyCondition.This));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.None));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.None));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipal_Matches ()
    {
      User principal = CreateUser (_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalAndDifferentOwningTenant_DoesNotMatch ()
    {
      User principal = CreateUser (TestHelper.CreateTenant ("Tenant"), null);
      Tenant owningTenant = _companyHelper.CompanyTenant;
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalInParent_DoesNotMatch ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant principalTenant = TestHelper.CreateTenant ("Tenant");
      owningTenant.Parent = principalTenant;
      User principal = CreateUser (principalTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalInChild_DoesNotMatch ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant principalTenant = TestHelper.CreateTenant ("Tenant");
      principalTenant.Parent = owningTenant;
      User principal = CreateUser (principalTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithoutOwningTenant_DoesNotMatch ()
    {
      User principal = CreateUser (_companyHelper.CompanyTenant, null);
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, null);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithoutPrincipal_DoesNotMatch ()
    {
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (null, _companyHelper.CompanyTenant);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithPrincipalWithoutTenantAndOwningTenant_DoesNotMatch ()
    {
      User principal = CreateUser (null, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void EmptyToken_DoesNotMatch ()
    {
      SecurityToken token = TestHelper.CreateEmptyToken ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}
