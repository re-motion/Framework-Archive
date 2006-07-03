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
    private DatabaseFixtures _dbFixtures;
    private ClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _dbFixtures = new DatabaseFixtures ();
      _transaction = new ClientTransaction ();
    }

    [Test]
    public void Find_ValidUser ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      User foundUser = User.Find (_transaction, "test.user");

      Assert.AreEqual ("test.user", foundUser.UserName);
    }

    [Test]
    public void Find_NotExistingUser ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      User foundUser = User.Find (_transaction, "not.existing");

      Assert.IsNull (foundUser);
    }

    [Test]
    public void GetRolesForGroup_Empty ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      User testUser = User.Find (_transaction, "test.user");
      Group parentOfOwnerGroup = Group.Find (_transaction, "parentOfOwnerGroup");
      List<Role> roles = testUser.GetRolesForGroup (parentOfOwnerGroup);

      Assert.AreEqual (0, roles.Count);
    }

    [Test]
    public void GetRolesForGroup_TwoRoles ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      User testUser = User.Find (_transaction, "test.user");
      Group testgroup = Group.Find (_transaction, "Testgroup");
      List<Role> roles = testUser.GetRolesForGroup (testgroup);

      Assert.AreEqual (2, roles.Count);
    }

    [Test]
    public void Find_UsersByClientID ()
    {
      _dbFixtures.CreateUsersWithDifferentClients ();

      DomainObjectCollection users = User.FindByClientID (_dbFixtures.CurrentClient.ID, _transaction);

      Assert.AreEqual (2, users.Count);
    }
  }
}
