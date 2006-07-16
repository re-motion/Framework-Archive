using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

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
    public void FindByUserName_ValidUser ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      User foundUser = User.FindByUserName (_transaction, "test.user");

      Assert.AreEqual ("test.user", foundUser.UserName);
    }

    [Test]
    public void FindByUserName_NotExistingUser ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      User foundUser = User.FindByUserName (_transaction, "not.existing");

      Assert.IsNull (foundUser);
    }

    [Test]
    public void GetRolesForGroup_Empty ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      User testUser = User.FindByUserName (_transaction, "test.user");
      Group parentOfOwnerGroup = Group.FindByUnqiueIdentifier (_transaction, "UnqiueIdentifier: parentOfOwnerGroup");
      List<Role> roles = testUser.GetRolesForGroup (parentOfOwnerGroup);

      Assert.AreEqual (0, roles.Count);
    }

    [Test]
    public void GetRolesForGroup_TwoRoles ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      User testUser = User.FindByUserName (_transaction, "test.user");
      Group testgroup = Group.FindByUnqiueIdentifier (_transaction, "UnqiueIdentifier: Testgroup");
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

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UserName_SameNameTwice ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      ClientTransaction transaction = new ClientTransaction ();
      Client client = _dbFixtures.CreateClient (transaction, "Testclient");
      Group group = _dbFixtures.CreateGroup (transaction, "TestGroup", "UnqiueIdentifier: TestGroup", null, client);
      User newUser = _dbFixtures.CreateUser (transaction, "test.user", "Test", "User", "Ing.", group, client);

      transaction.Commit ();
    }
  }
}
