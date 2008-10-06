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
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclProbeTest : AclToolsTestBase
  {
    [Test]
    public void CreateAclProbe_User_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithAbstractRole();
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.User, Is.EqualTo (User));
    }

    // TODO: Enable as soon as GroupSelection supports SpecificGroup state.
    [Test]
    [Ignore ("Enable as soon as GroupSelection supports SpecificGroup state.")]
    public void CreateAclProbe_SpecificGroup_Test ()
    {
      //AccessControlEntry ace = TestHelper.CreateAceWithSpecficGroup (Group);
      //AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      //Assert.That (aclProbe.SecurityToken.OwningGroups, NUnit.Framework.SyntaxHelpers.List.Contains (ace.SpecificGroup));
    }

    [Test]
    public void CreateAclProbe_OwningGroup_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup();
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningGroups, List.Contains (Role.Group));

      var accessConditionsExpected = new AclExpansionAccessConditions();
      accessConditionsExpected.IsOwningGroupRequired = true;
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }

    [Test]
    public void CreateAclProbe_GroupSelectionAll_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithGroupSelectionAll();
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningGroups, List.Contains (ace.SpecificGroup));

      var accessConditionsExpected = new AclExpansionAccessConditions();
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "ace.GroupSelection=2147483647 is currently not supported by this method. Please extend method to handle the new GroupSelection state.")]
    public void CreateAclProbe_UnsupportedGroupSelection_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithGroupSelectionAll();
      FleshOutAccessControlEntryForTest (ace);
      ace.GroupSelection = (GroupSelection) int.MaxValue;
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
    }


    [Test]
    public void CreateAclProbe_SpecificTenant_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithSpecficTenant (Tenant);
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningTenant, Is.EqualTo (ace.SpecificTenant));

      var accessConditionsExpected = new AclExpansionAccessConditions();
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }

    [Test]
    public void CreateAclProbe_OwningTenant_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant();
      FleshOutAccessControlEntryForTest (ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.OwningTenant, Is.EqualTo (User.Tenant));

      var accessConditionsExpected = new AclExpansionAccessConditions();
      accessConditionsExpected.IsOwningTenantRequired = true;
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "ace.TenantSelection=-1 is currently not supported by this method. Please extend method to handle the new TenantSelection state.")]
    public void CreateAclProbe_UnsupportedTenantSelection_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant();
      FleshOutAccessControlEntryForTest (ace);
      ace.TenantSelection = (TenantSelection) (object) -1;
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
    }


    [Test]
    public void CreateAclProbe_SpecificAbstractRole_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithAbstractRole();
      FleshOutAccessControlEntryForTest (ace);
      Assert.That (ace.SpecificAbstractRole, Is.Not.Null);
      AclProbe aclProbe = AclProbe.CreateAclProbe (User, Role, ace);
      Assert.That (aclProbe.SecurityToken.AbstractRoles, List.Contains (ace.SpecificAbstractRole));

      var accessConditionsExpected = new AclExpansionAccessConditions();
      //accessConditionsExpected.IsAbstractRoleRequired = true;
      accessConditionsExpected.AbstractRole = ace.SpecificAbstractRole;
      Assert.That (aclProbe.AccessConditions, Is.EqualTo (accessConditionsExpected));
    }


    private void FleshOutAccessControlEntryForTest (AccessControlEntry ace)
    {
      ace.SpecificGroup = TestHelper.CreateGroup ("Specific Group for an ACE", null, Tenant);
    }
  }
}