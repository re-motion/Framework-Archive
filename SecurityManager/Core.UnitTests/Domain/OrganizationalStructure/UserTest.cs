using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Development.UnitTesting;
using Rubicon.Security;
using Rubicon.Security.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class UserTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ObjectID _expectedTenantID;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      _dbFixtures = new DatabaseFixtures ();
      Tenant tenant = _dbFixtures.CreateOrganizationalStructureWithTwoTenants (ClientTransaction.NewTransaction());
      _expectedTenantID = tenant.ID;
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new OrganizationalStructureTestHelper ();
      _testHelper.Transaction.EnterScope();
    }

    [Test]
    public void FindByUserName_ValidUser ()
    {
      User foundUser = User.FindByUserName ("test.user", ClientTransactionScope.CurrentTransaction);

      Assert.AreEqual ("test.user", foundUser.UserName);
    }

    [Test]
    public void FindByUserName_NotExistingUser ()
    {
      User foundUser = User.FindByUserName ("not.existing", ClientTransactionScope.CurrentTransaction);

      Assert.IsNull (foundUser);
    }

    [Test]
    public void GetRolesForGroup_Empty ()
    {
      User testUser = User.FindByUserName ("test.user", ClientTransactionScope.CurrentTransaction);
      Group parentOfOwnerGroup = Group.FindByUnqiueIdentifier ("UID: testParentOfOwningGroup", ClientTransactionScope.CurrentTransaction);
      List<Role> roles = testUser.GetRolesForGroup (parentOfOwnerGroup);

      Assert.AreEqual (0, roles.Count);
    }

    [Test]
    public void GetRolesForGroup_TwoRoles ()
    {
      User testUser = User.FindByUserName ("test.user", ClientTransactionScope.CurrentTransaction);
      Group testgroup = Group.FindByUnqiueIdentifier ("UID: testgroup", ClientTransactionScope.CurrentTransaction);
      List<Role> roles = testUser.GetRolesForGroup (testgroup);

      Assert.AreEqual (2, roles.Count);
    }

    [Test]
    public void Find_UsersByTenantID ()
    {
      DomainObjectCollection users = User.FindByTenantID (_expectedTenantID, ClientTransactionScope.CurrentTransaction);

      Assert.AreEqual (5, users.Count);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UserName_SameNameTwice ()
    {
      Tenant tenant = _testHelper.CreateTenant ("Testtenant", "UID: testTenant");
      Group group = _testHelper.CreateGroup ("TestGroup", "UID: testGroup", null, tenant);
      User newUser = _testHelper.CreateUser ("test.user", "Test", "User", "Ing.", group, tenant);

      ClientTransactionScope.CurrentTransaction.Commit ();
    }

    [Test]
    public void Get_Current_NotInitialized ()
    {
      Assert.IsNull (User.Current);
    }

    [Test]
    public void SetAndGet_Current ()
    {
      User user = CreateUser ();

      User.Current = user;
      Assert.AreSame (user, User.Current);

      User.Current = null;
    }

    [Test]
    public void SetAndGet_Current_Threading ()
    {
      User user = CreateUser ();
      User.Current = user;
      Assert.AreSame (user, User.Current);

      ThreadRunner.Run (delegate ()
          {
            Tenant otherTenant = _testHelper.CreateTenant ("OtherTenant", "UID: otherTenant");
            Group otherGroup = _testHelper.CreateGroup ("OtherGroup", "UID: otherGroup", null, otherTenant);
            User otherUser = _testHelper.CreateUser ("other.user", "Other", "User", "Ing.", otherGroup, otherTenant);

            Assert.IsNull (User.Current);
            User.Current = otherUser;
            Assert.AreSame (otherUser, User.Current);

          });

      Assert.AreSame (user, User.Current);
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
    public void GetSecurableType ()
    {
      ISecurableObject user = CreateUser ();

      Assert.AreSame (typeof (User), user.GetSecurableType ());
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
      Assert.AreEqual (user.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.AreEqual (user.UserName, securityContext.Owner);
      Assert.AreEqual (user.OwningGroup.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.AreEqual (user.Tenant.UniqueIdentifier, securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void CreateSecurityContext_WithNoGroup ()
    {
      User user = CreateUser ();
      user.OwningGroup = null;

      SecurityContext securityContext = ((ISecurityContextFactory) user).CreateSecurityContext ();
      Assert.AreEqual (user.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.AreEqual (user.UserName, securityContext.Owner);
      Assert.IsEmpty (securityContext.OwnerGroup);
      Assert.AreEqual (user.Tenant.UniqueIdentifier, securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void CreateSecurityContext_WithNoTenant ()
    {
      User user = CreateUser ();
      user.Tenant = null;

      SecurityContext securityContext = ((ISecurityContextFactory) user).CreateSecurityContext ();
      Assert.AreEqual (user.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.AreEqual (user.UserName, securityContext.Owner);
      Assert.AreEqual (user.OwningGroup.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.IsEmpty (securityContext.OwnerTenant);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void GetDisplayName_WithLastNameAndFirstNameAndTitle ()
    {
      User user = CreateUser ();
      user.LastName = "UserLastName";
      user.FirstName = "UserFirstName";
      user.Title = "UserTitle";

      Assert.AreEqual ("UserLastName UserFirstName, UserTitle", user.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithLastNameAndFirstName ()
    {
      User user = CreateUser ();
      user.LastName = "UserLastName";
      user.FirstName = "UserFirstName";
      user.Title = null;

      Assert.AreEqual ("UserLastName UserFirstName", user.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithLastName ()
    {
      User user = CreateUser ();
      user.LastName = "UserLastName";
      user.FirstName = null;
      user.Title = null;

      Assert.AreEqual ("UserLastName", user.DisplayName);
    }

    private User CreateUser ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant", "UID: testTenant");
      Group group = _testHelper.CreateGroup ("TestGroup", "UID: testGroup", null, tenant);
      User user = _testHelper.CreateUser ("test.user", "Test", "User", "Ing.", group, tenant);
   
      return user;
    }

  }
}
