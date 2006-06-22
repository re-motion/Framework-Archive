using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class UserTest : DomainTest
  {
    [Test]
    public void Find_ValidUser ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();

      User foundUser = User.Find (transaction, "test.user");

      Assert.AreEqual ("test.user", foundUser.UserName);
    }

    [Test]
    public void Find_NotExistingUser ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();

      User foundUser = User.Find (transaction, "not.existing");

      Assert.IsNull (foundUser);
    }

    [Test]
    public void GetRolesForGroup_Empty ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();

      User testUser = User.Find (transaction, "test.user");
      Group parentOfOwnerGroup = Group.Find (transaction, "parentOfOwnerGroup");
      List<Role> roles = testUser.GetRolesForGroup (parentOfOwnerGroup);

      Assert.AreEqual (0, roles.Count);
    }

    [Test]
    public void GetRolesForGroup_TwoRoles ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();

      User testUser = User.Find (transaction, "test.user");
      Group testgroup = Group.Find (transaction, "Testgroup");
      List<Role> roles = testUser.GetRolesForGroup (testgroup);

      Assert.AreEqual (2, roles.Count);
    }

    [Test]
    public void Find_UsersByClientID ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateUsersWithDifferentClients ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection users = User.FindByClientID (dbFixtures.CurrentClient.ID, transaction);

      Assert.AreEqual (2, users.Count);
    }
  }
}
