using System;
using System.Collections.Generic;
using System.Security.Principal;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.UnitTests.TestDomain;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class SecurityTokenBuilderTest : DomainTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructureWithTwoClients ();
    }

    [Test]
    public void Create_AbstractRolesEmpty ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (0, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidAbstractRole ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithQualityManagerRole ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (1, token.AbstractRoles.Count);
      Assert.AreEqual ("QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRoles, Rubicon.SecurityManager.UnitTests", token.AbstractRoles[0].Name);
    }

    [Test]
    public void Create_WithValidAbstractRoles ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithQualityManagerAndDeveloperRoles ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The abstract role 'Undefined|Rubicon.SecurityManager.UnitTests.TestDomain.UndefinedAbstractRoles, Rubicon.SecurityManager.UnitTests' could not be found.")]
    public void Create_WithNotExistingAbstractRole ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithAbstractRoles (ProjectRoles.Developer, UndefinedAbstractRoles.Undefined, ProjectRoles.QualityManager);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidUser ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      Assert.AreEqual ("test.user", token.User.UserName);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The user 'notexisting.user' could not be found.")]
    public void Create_WithNotExistingUser ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateNotExistingUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);
    }

    [Test]
    public void Create_WithValidGroup ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      AccessControlObjectAssert.ContainsGroup ("UID: testOwnerGroup", token.OwningGroups);
    }

    [Test]
    public void Create_WithoutOwningGroup ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithoutOwnerGroup ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      Assert.AreEqual (0, token.OwningGroups.Count);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The group 'UID: NotExistingGroup' could not be found.")]
    public void Create_WithNotExistingOwningGroup ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithNotExistingOwnerGroup ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);
    }

    [Test]
    public void Create_WithParentOwningGroup ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      AccessControlObjectAssert.ContainsGroup ("UID: testRootGroup", token.OwningGroups);
    }

    private IPrincipal CreateTestUser ()
    {
      return CreateUser ("test.user");
    }

    private IPrincipal CreateNotExistingUser ()
    {
      return CreateUser ("notexisting.user");
    }

    private IPrincipal CreateUser (string userName)
    {
      return new GenericPrincipal (new GenericIdentity (userName), new string[0]);
    }

    private SecurityContext CreateStatelessContext ()
    {
      return new SecurityContext (typeof (Order), "owner", "UID: testOwnerGroup", "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithoutOwnerGroup ()
    {
      return new SecurityContext (typeof (Order), "owner", null, "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwnerGroup ()
    {
      return new SecurityContext (typeof (Order), "owner", "UID: NotExistingGroup", "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithQualityManagerRole ()
    {
      return CreateContextWithAbstractRoles (ProjectRoles.QualityManager);
    }

    private SecurityContext CreateContextWithQualityManagerAndDeveloperRoles ()
    {
      return CreateContextWithAbstractRoles (ProjectRoles.QualityManager, ProjectRoles.Developer);
    }

    private SecurityContext CreateContextWithAbstractRoles (params Enum[] abstractRoles)
    {
      return new SecurityContext (typeof (Order), "owner", "UID: testOwnerGroup", "ownerClient", new Dictionary<string, Enum> (), abstractRoles);
    }
  }
}
