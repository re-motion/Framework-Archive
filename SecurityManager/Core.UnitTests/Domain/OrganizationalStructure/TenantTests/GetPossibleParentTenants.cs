// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class GetPossibleParentTenants : TenantTestBase
  {
    private DatabaseFixtures _dbFixtures;
    private ObjectID _expectedTenantID;

    public override void SetUp ()
    {
      base.SetUp();

      _dbFixtures = new DatabaseFixtures();
      Tenant tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      _expectedTenantID = tenant.ID;

      SecurityConfiguration.Current.SecurityProvider = null;
    }

    public override void TearDown ()
    {
      base.TearDown();

      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void Test ()
    {
      Tenant root = Tenant.GetObject (_expectedTenantID);
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant grandChild1 = TestHelper.CreateTenant ("GrandChild1", "UID: GrandChild1");
      grandChild1.Parent = child1;
      Tenant grandChild2 = TestHelper.CreateTenant ("GrandChild2", "UID: GrandChild2");
      grandChild2.Parent = grandChild1;

      ClientTransaction.Current.Commit();

      var tenants = child1.GetPossibleParentTenants().ToArray();

      var expectedTenants = Tenant.FindAll().Except (new[] { child1, grandChild1, grandChild2 }).ToArray();

      Assert.That (tenants, Is.Not.Empty);
      Assert.That (tenants, Is.EquivalentTo (expectedTenants));
    }

    [Test]
    public void Test_WithSecurity_UsesSecurityFreeSectionForHierarchyFilter ()
    {
      Tenant root = Tenant.GetObject (_expectedTenantID);
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant grandChild1 = TestHelper.CreateTenant ("GrandChild1", "UID: GrandChild1");
      grandChild1.Parent = child1;

      ClientTransaction.Current.Commit();

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      var child1SecurityContext = ((ISecurityContextFactory) child1).CreateSecurityContext();
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg<ISecurityContext>.Is.NotEqual (child1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg.Is (child1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      var tenants = root.GetPossibleParentTenants().ToArray();

      var expectedTenants = Tenant.FindAll().Except (new[] { root, child1, grandChild1 }).ToArray();

      Assert.That (tenants, Is.Not.Empty);
      Assert.That (tenants, Is.EquivalentTo (expectedTenants));
    }

    [Test]
    public void Test_WithSecurity_FiltersPossibleParentsWithoutReadPermission ()
    {
      Tenant root = Tenant.GetObject (_expectedTenantID);
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant grandChild1 = TestHelper.CreateTenant ("GrandChild1", "UID: GrandChild1");
      grandChild1.Parent = child1;

      ClientTransaction.Current.Commit();

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      SecurityConfiguration.Current.SecurityProvider = securityProviderStub;

      var child1SecurityContext = ((ISecurityContextFactory) child1).CreateSecurityContext();
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg<ISecurityContext>.Is.NotEqual (child1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg.Is (child1SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      var tenants = grandChild1.GetPossibleParentTenants().ToArray();

      var expectedTenants = Tenant.FindAll().Except (new[] { child1, grandChild1 }).ToArray();

      Assert.That (tenants, Is.Not.Empty);
      Assert.That (tenants, Is.EquivalentTo (expectedTenants));
    }
  }
}