using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.Security.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.UnitTests.Domain.AccessControl;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class PositionTest : DomainTest
  {
    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructureWithTwoClients ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection positions = Position.FindAll (transaction);

      Assert.AreEqual (3, positions.Count);
    }

    [Test]
    public void DeletePosition_WithAccessControlEntry ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      Position position = testHelper.CreatePosition ("Position");
      AccessControlEntry ace = testHelper.CreateAceWithPosition (position, GroupSelection.All);

      position.Delete ();

      Assert.IsTrue (ace.IsDiscarded);
    }

    [Test]
    public void DeletePosition_WithRole ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      Client client = testHelper.CreateClient ("TestClient", "UID: testClient");
      Group userGroup = testHelper.CreateGroup ("UserGroup", Guid.NewGuid ().ToString(), null, client);
      Group roleGroup = testHelper.CreateGroup ("RoleGroup", Guid.NewGuid ().ToString (), null, client);
      User user = testHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, client);
      Position position = testHelper.CreatePosition ("Position");
      Role role = testHelper.CreateRole (user, roleGroup, position);

      position.Delete ();

      Assert.IsTrue (role.IsDiscarded);
    }

    [Test]
    public void DeletePosition_WithGroupTypePosition ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      GroupType groupType = testHelper.CreateGroupType ("GroupType");
      Position position = testHelper.CreatePosition ("Position");
      GroupTypePosition concretePosition = testHelper.CreateGroupTypePosition (groupType, position);

      position.Delete ();

      Assert.IsTrue (concretePosition.IsDiscarded);
    }

    [Test]
    public void GetDisplayName ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      Position position = testHelper.CreatePosition ("PositionName");

      Assert.AreEqual ("PositionName", position.DisplayName);
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      ISecurableObject position = testHelper.CreatePosition ("PositionName");

      IObjectSecurityStrategy objectSecurityStrategy = position.GetSecurityStrategy ();
      Assert.IsNotNull (objectSecurityStrategy);
      Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
      DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
      Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      ISecurableObject position = testHelper.CreatePosition ("PositionName");

      Assert.AreSame (position.GetSecurityStrategy (), position.GetSecurityStrategy ());
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      Position position = testHelper.CreatePosition ("PositionName");
      IDomainObjectSecurityContextFactory factory = position;

      Assert.IsFalse (factory.IsDiscarded);
      Assert.IsTrue (factory.IsNew);
      Assert.IsFalse (factory.IsDeleted);

      position.Delete ();

      Assert.IsTrue (factory.IsDiscarded);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      Position position = testHelper.CreatePosition ("PositionName");
      position.Delegation = Delegation.Enabled;

      SecurityContext securityContext = ((ISecurityContextFactory) position).CreateSecurityContext ();
      Assert.AreEqual (position.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.IsEmpty (securityContext.Owner);
      Assert.IsEmpty (securityContext.OwnerGroup);
      Assert.IsEmpty (securityContext.OwnerClient);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.AreEqual (1, securityContext.GetNumberOfStates());
      Assert.AreEqual (new EnumWrapper (Delegation.Enabled), securityContext.GetState ("Delegation"));
    }
  }
}
