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
    private OrganisationalStructureTestHelper _testHelper;
    private ObjectID _expectedClientID;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
     
      _dbFixtures = new DatabaseFixtures ();
      Client client = _dbFixtures.CreateOrganizationalStructureWithTwoClients ();
      _expectedClientID = client.ID;
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new OrganisationalStructureTestHelper ();
    }

    [Test]
    public void FindByUserName_ValidUser ()
    {
      User foundUser = User.FindByUserName (_testHelper.Transaction, "test.user");

      Assert.AreEqual ("test.user", foundUser.UserName);
    }

    [Test]
    public void FindByUserName_NotExistingUser ()
    {
      User foundUser = User.FindByUserName (_testHelper.Transaction, "not.existing");

      Assert.IsNull (foundUser);
    }

    [Test]
    public void GetRolesForGroup_Empty ()
    {
      User testUser = User.FindByUserName (_testHelper.Transaction, "test.user");
      Group parentOfOwnerGroup = Group.FindByUnqiueIdentifier (_testHelper.Transaction, "UnqiueIdentifier: parentOfOwnerGroup");
      List<Role> roles = testUser.GetRolesForGroup (parentOfOwnerGroup);

      Assert.AreEqual (0, roles.Count);
    }

    [Test]
    public void GetRolesForGroup_TwoRoles ()
    {
      User testUser = User.FindByUserName (_testHelper.Transaction, "test.user");
      Group testgroup = Group.FindByUnqiueIdentifier (_testHelper.Transaction, "UnqiueIdentifier: Testgroup");
      List<Role> roles = testUser.GetRolesForGroup (testgroup);

      Assert.AreEqual (2, roles.Count);
    }

    [Test]
    public void Find_UsersByClientID ()
    {
      DomainObjectCollection users = User.FindByClientID (_expectedClientID, _testHelper.Transaction);

      Assert.AreEqual (2, users.Count);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UserName_SameNameTwice ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Group group = _testHelper.CreateGroup ("TestGroup", "UnqiueIdentifier: TestGroup", null, client);
      User newUser = _testHelper.CreateUser ("test.user", "Test", "User", "Ing.", group, client);

      _testHelper.Transaction.Commit ();
    }
  }
}
