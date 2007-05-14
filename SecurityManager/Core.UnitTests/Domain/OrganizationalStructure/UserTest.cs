using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Security.Data.DomainObjects;

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
      User foundUser = User.FindByUserName ("test.user", _testHelper.Transaction);

      Assert.AreEqual ("test.user", foundUser.UserName);
    }

    [Test]
    public void FindByUserName_NotExistingUser ()
    {
      User foundUser = User.FindByUserName ("not.existing", _testHelper.Transaction);

      Assert.IsNull (foundUser);
    }

    [Test]
    public void GetRolesForGroup_Empty ()
    {
      User testUser = User.FindByUserName ("test.user", _testHelper.Transaction);
      Group parentOfOwnerGroup = Group.FindByUnqiueIdentifier ("UID: testParentOfOwningGroup", _testHelper.Transaction);
      List<Role> roles = testUser.GetRolesForGroup (parentOfOwnerGroup);

      Assert.AreEqual (0, roles.Count);
    }

    [Test]
    public void GetRolesForGroup_TwoRoles ()
    {
      User testUser = User.FindByUserName ("test.user", _testHelper.Transaction);
      Group testgroup = Group.FindByUnqiueIdentifier ("UID: testgroup", _testHelper.Transaction);
      List<Role> roles = testUser.GetRolesForGroup (testgroup);

      Assert.AreEqual (2, roles.Count);
    }

    [Test]
    public void Find_UsersByClientID ()
    {
      DomainObjectCollection users = User.FindByClientID (_expectedClientID, _testHelper.Transaction);

      Assert.AreEqual (5, users.Count);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UserName_SameNameTwice ()
    {
      Client client = _testHelper.CreateClient ("Testclient", "UID: testClient");
      Group group = _testHelper.CreateGroup ("TestGroup", "UID: testGroup", null, client);
      User newUser = _testHelper.CreateUser ("test.user", "Test", "User", "Ing.", group, client);

      _testHelper.Transaction.Commit ();
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject user = CreateUser ();

      IObjectSecurityStrategy objectSecurityStrategy = user.GetSecurityStrategy ();
      Assert.IsNotNull (objectSecurityStrategy);
      Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
      DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
      Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject user = CreateUser ();

      Assert.AreSame (user.GetSecurityStrategy (), user.GetSecurityStrategy ());
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      User user = CreateUser ();
      IDomainObjectSecurityContextFactory factory = user;

      Assert.IsFalse (factory.IsDiscarded);
      Assert.IsTrue (factory.IsNew);
      Assert.IsFalse (factory.IsDeleted);

      user.Delete ();

      Assert.IsTrue (factory.IsDiscarded);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      User user = CreateUser ();

      SecurityContext securityContext = ((ISecurityContextFactory) user).CreateSecurityContext ();
      Assert.AreEqual (user.GetType(), Type.GetType (securityContext.Class));
      Assert.AreEqual (user.UserName, securityContext.Owner);
      Assert.AreEqual (user.Group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.AreEqual (user.Client.UniqueIdentifier, securityContext.OwnerClient);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void CreateSecurityContext_WithNoGroup ()
    {
      User user = CreateUser ();
      user.Group = null;

      SecurityContext securityContext = ((ISecurityContextFactory) user).CreateSecurityContext ();
      Assert.AreEqual (user.GetType (), Type.GetType (securityContext.Class));
      Assert.AreEqual (user.UserName, securityContext.Owner);
      Assert.IsEmpty (securityContext.OwnerGroup);
      Assert.AreEqual (user.Client.UniqueIdentifier, securityContext.OwnerClient);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void CreateSecurityContext_WithNoClient ()
    {
      User user = CreateUser ();
      user.Client = null;

      SecurityContext securityContext = ((ISecurityContextFactory) user).CreateSecurityContext ();
      Assert.AreEqual (user.GetType (), Type.GetType (securityContext.Class));
      Assert.AreEqual (user.UserName, securityContext.Owner);
      Assert.AreEqual (user.Group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.IsEmpty (securityContext.OwnerClient);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    private User CreateUser ()
    {
      Client client = _testHelper.CreateClient ("TestClient", "UID: testClient");
      Group group = _testHelper.CreateGroup ("TestGroup", "UID: testGroup", null, client);
      User user = _testHelper.CreateUser ("test.user", "Test", "User", "Ing.", group, client);
   
      return user;
    }

  }
}
