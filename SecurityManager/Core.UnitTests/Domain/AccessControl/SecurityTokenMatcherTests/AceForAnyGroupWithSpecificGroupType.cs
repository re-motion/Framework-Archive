/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceForAnyGroupWithSpecificGroupType : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithSpecificGroupType(_companyHelper.DepartmentGroupType);

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.AnyGroupWithSpecificGroupType));
      Assert.That (_ace.SpecificGroupType, Is.SameAs (_companyHelper.DepartmentGroupType));
      Assert.That (_ace.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.Undefined));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.None));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithRoleInGroupMatchingGroupType_Matches ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group userGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateToken (user, null, null, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsTrue (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithRoleInGroupWithOtherGroupType_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group userGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }

    [Test]
    public void TokenWithoutRole_DoesNotMatch ()
    {
      SecurityToken token = TestHelper.CreateToken (null, null, _companyHelper.AustrianProjectsDepartment, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.IsFalse (matcher.MatchesToken (token));
    }
  }
}