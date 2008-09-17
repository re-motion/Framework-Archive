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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Data.DomainObjects.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class PositionTest : DomainTest
  {
    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        DomainObjectCollection positions = Position.FindAll ();

        Assert.AreEqual (3, positions.Count);
      }
    }

    [Test]
    public void DeletePosition_WithAccessControlEntry ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("Position");
        AccessControlEntry ace = testHelper.CreateAceWithPosition (position, GroupSelection.All);

        position.Delete();

        Assert.IsTrue (ace.IsDiscarded);
      }
    }

    [Test]
    public void DeletePosition_WithRole ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Tenant tenant = testHelper.CreateTenant ("TestTenant", "UID: testTenant");
        Group userGroup = testHelper.CreateGroup ("UserGroup", Guid.NewGuid().ToString(), null, tenant);
        Group roleGroup = testHelper.CreateGroup ("RoleGroup", Guid.NewGuid().ToString(), null, tenant);
        User user = testHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, tenant);
        Position position = testHelper.CreatePosition ("Position");
        Role role = testHelper.CreateRole (user, roleGroup, position);

        position.Delete();

        Assert.IsTrue (role.IsDiscarded);
      }
    }

    [Test]
    public void DeletePosition_WithGroupTypePosition ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        GroupType groupType = testHelper.CreateGroupType ("GroupType");
        Position position = testHelper.CreatePosition ("Position");
        GroupTypePosition concretePosition = testHelper.CreateGroupTypePosition (groupType, position);

        position.Delete();

        Assert.IsTrue (concretePosition.IsDiscarded);
      }
    }

    [Test]
    public void GetDisplayName ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");

        Assert.AreEqual ("PositionName", position.DisplayName);
      }
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        IObjectSecurityStrategy objectSecurityStrategy = position.GetSecurityStrategy();
        Assert.IsNotNull (objectSecurityStrategy);
        Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
        DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
        Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
      }
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        Assert.AreSame (position.GetSecurityStrategy(), position.GetSecurityStrategy());
      }
    }

    [Test]
    public void GetSecurableType ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        ISecurableObject position = testHelper.CreatePosition ("PositionName");

        Assert.AreSame (typeof (Position), position.GetSecurableType());
      }
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");
        IDomainObjectSecurityContextFactory factory = position;

        Assert.IsFalse (factory.IsDiscarded);
        Assert.IsTrue (factory.IsNew);
        Assert.IsFalse (factory.IsDeleted);

        position.Delete();

        Assert.IsTrue (factory.IsDiscarded);
      }
    }

    [Test]
    public void CreateSecurityContext ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Position position = testHelper.CreatePosition ("PositionName");
        position.Delegation = Delegation.Enabled;

        ISecurityContext securityContext = ((ISecurityContextFactory) position).CreateSecurityContext();
        Assert.AreEqual (position.GetPublicDomainObjectType(), Type.GetType (securityContext.Class));
        Assert.IsEmpty (securityContext.Owner);
        Assert.IsEmpty (securityContext.OwnerGroup);
        Assert.IsEmpty (securityContext.OwnerTenant);
        Assert.IsEmpty (securityContext.AbstractRoles);
        Assert.AreEqual (1, securityContext.GetNumberOfStates());
        Assert.AreEqual (new EnumWrapper (Delegation.Enabled), securityContext.GetState ("Delegation"));
      }
    }
  }
}
