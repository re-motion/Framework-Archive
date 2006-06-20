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
  public class SecurityTokenBuilderTest
  {
    [Test]
    public void Create_AbstractRolesEmpty ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
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
      dbFixtures.CreateTestData ();
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
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithQualityManagerAndDeveloperRoles ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidUser ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      Assert.AreEqual ("test.user", token.User.UserName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The user 'notexisting.user' could not be found.\r\nParameter name: userName")]
    public void Create_WithNotExistingUser ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
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
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      AccessControlObjectAssert.ContainsGroup ("ownerGroup", token.Groups);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The group 'NotExistingGroup' could not be found.\r\nParameter name: groupName")]
    public void Create_WithNotExistingGroup ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
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
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      AccessControlObjectAssert.ContainsGroup ("rootGroup", token.Groups);
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
      return new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwnerGroup ()
    {
      return new SecurityContext (typeof (Order), "owner", "NotExistingGroup", "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
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
      return new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", new Dictionary<string, Enum> (), abstractRoles);
    }
  }
}
