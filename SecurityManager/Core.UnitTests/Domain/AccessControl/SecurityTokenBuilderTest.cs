using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.SecurityManager.UnitTests.TestDomain;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using System.Security.Principal;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class SecurityTokenBuilderTest : DomainTest
  {
    [Test]
    public void Create_AbstractRolesEmpty ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (0, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidAbstractRole ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithQualityManagerRole ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (1, token.AbstractRoles.Count);
      Assert.AreEqual ("QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.UnitTests", token.AbstractRoles[0].Name);
    }

    [Test]
    public void Create_WithValidAbstractRoles ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithQualityManagerAndDeveloperRoles ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), "The abstract role 'Undefined|Rubicon.SecurityManager.UnitTests.TestDomain.UndefinedAbstractRoles, Rubicon.SecurityManager.UnitTests' could not be found.")]
    public void Create_WithNotExistingAbstractRole ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithAbstractRoles (ProjectRole.Developer, UndefinedAbstractRoles.Undefined, ProjectRole.QualityManager);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidUser ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      Assert.AreEqual ("test.user", token.User.UserName);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), "The user 'notexisting.user' could not be found.")]
    public void Create_WithNotExistingUser ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateNotExistingUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);
    }

    [Test]
    public void Create_WithValidGroup ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      AccessControlObjectAssert.ContainsGroup ("UnqiueIdentifier: ownerGroup", token.Groups);
    }

    [Test]
    public void Create_WithoutGroup ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithoutOwnerGroup ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      Assert.AreEqual (0, token.Groups.Count);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), "The group 'UnqiueIdentifier: NotExistingGroup' could not be found.")]
    public void Create_WithNotExistingGroup ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithNotExistingOwnerGroup ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);
    }

    [Test]
    public void Create_WithParentGroup ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructure ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      AccessControlObjectAssert.ContainsGroup ("UnqiueIdentifier: rootGroup", token.Groups);
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
      return new SecurityContext (typeof (Order), "owner", "UnqiueIdentifier: ownerGroup", "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithoutOwnerGroup ()
    {
      return new SecurityContext (typeof (Order), "owner", null, "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwnerGroup ()
    {
      return new SecurityContext (typeof (Order), "owner", "UnqiueIdentifier: NotExistingGroup", "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithQualityManagerRole ()
    {
      return CreateContextWithAbstractRoles (ProjectRole.QualityManager);
    }

    private SecurityContext CreateContextWithQualityManagerAndDeveloperRoles ()
    {
      return CreateContextWithAbstractRoles (ProjectRole.QualityManager, ProjectRole.Developer);
    }

    private SecurityContext CreateContextWithAbstractRoles (params Enum[] abstractRoles)
    {
      return new SecurityContext (typeof (Order), "owner", "UnqiueIdentifier: ownerGroup", "ownerClient", new Dictionary<string, Enum> (), abstractRoles);
    }
  }
}
