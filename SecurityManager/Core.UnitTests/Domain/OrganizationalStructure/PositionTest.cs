using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.UnitTests.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.AccessControl;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class PositionTest : DomainTest
  {
    [Test]
    public void Find_PositionsByClientID ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      Client exptectedClient = dbFixtures.CreateOrganizationalStructureWithTwoClients ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection positions = Position.FindByClientID (exptectedClient.ID, transaction);

      Assert.AreEqual (2, positions.Count);
    }

    [Test]
    public void DeletePosition_WithAccessControlEntry ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      Position position = testHelper.CreatePosition ("Position", testHelper.CreateClient ("Client"));
      AccessControlEntry ace = testHelper.CreateAceWithPosition (position, GroupSelection.All);

      position.Delete ();

      Assert.IsTrue (ace.IsDiscarded);
    }

    [Test]
    public void DeletePosition_WithRole ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      Client client = testHelper.CreateClient ("Client");
      Group userGroup = testHelper.CreateGroup ("UserGroup", Guid.NewGuid ().ToString(), null, client);
      Group roleGroup = testHelper.CreateGroup ("RoleGroup", Guid.NewGuid ().ToString (), null, client);
      User user = testHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, client);
      Position position = testHelper.CreatePosition ("Position", client);
      Role role = testHelper.CreateRole (user, roleGroup, position);

      position.Delete ();

      Assert.IsTrue (role.IsDiscarded);
    }

    [Test]
    public void DeletePosition_WithConcretePosition ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      Client client = testHelper.CreateClient ("Client");
      GroupType groupType = testHelper.CreateGroupType ("GroupType", client);
      Position position = testHelper.CreatePosition ("Position", client);
      ConcretePosition concretePosition = testHelper.CreateConcretePosition ("ConcretePosition", groupType, position);

      position.Delete ();

      Assert.IsTrue (concretePosition.IsDiscarded);
    }

    [Test]
    public void GetDisplayName ()
    {
      OrganisationalStructureTestHelper testHelper = new OrganisationalStructureTestHelper ();
      Client client = testHelper.CreateClient ("Client");
      Position position = testHelper.CreatePosition ("PositionName", client);

      Assert.AreEqual ("PositionName", position.DisplayName);
    }

  }
}
