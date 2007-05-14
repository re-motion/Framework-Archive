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
      SecurityContext context = CreateContext ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.IsEmpty (token.AbstractRoles);
    }

    [Test]
    public void Create_WithValidAbstractRole ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContext (ProjectRoles.QualityManager);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (1, token.AbstractRoles.Count);
      Assert.AreEqual ("QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRoles, Rubicon.SecurityManager.UnitTests", token.AbstractRoles[0].Name);
    }

    [Test]
    public void Create_WithValidAbstractRoles ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContext (ProjectRoles.QualityManager, ProjectRoles.Developer);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The abstract role 'Undefined|Rubicon.SecurityManager.UnitTests.TestDomain.UndefinedAbstractRoles, Rubicon.SecurityManager.UnitTests' could not be found.")]
    public void Create_WithNotExistingAbstractRole ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContext (ProjectRoles.Developer, UndefinedAbstractRoles.Undefined, ProjectRoles.QualityManager);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, CreateTestUser (), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidUser ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContext ();
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
      SecurityContext context = CreateContext ();
      IPrincipal user = CreateNotExistingUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);
    }

    [Test]
    public void Create_WithValidOwningClient ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      Assert.IsNotNull (token.OwningClient);
      Assert.AreEqual ("UID: testClient", token.OwningClient.UniqueIdentifier);
    }

    [Test]
    public void Create_WithoutOwningClient ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithoutOwningClient ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      Assert.IsNull (token.OwningClient);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), "The client 'UID: NotExistingClient' could not be found.")]
    public void Create_WithNotExistingOwningClient ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithNotExistingOwningClient ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);
    }

    [Test]
    public void Create_WithValidOwningGroup ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      AccessControlObjectAssert.ContainsGroup ("UID: testOwningGroup", token.OwningGroups);
    }

    [Test]
    public void Create_WithoutOwningGroup ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithoutOwningGroup ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);

      Assert.IsEmpty (token.OwningGroups);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The group 'UID: NotExistingGroup' could not be found.")]
    public void Create_WithNotExistingOwningGroup ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithNotExistingOwningGroup ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, user, context);
    }

    [Test]
    public void Create_WithParentOwningGroup ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContext ();
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

    private SecurityContext CreateContext (params Enum[] abstractRoles)
    {
      return new SecurityContext (typeof (Order), "owner", "UID: testOwningGroup", "UID: testClient", new Dictionary<string, Enum> (), abstractRoles);
    }

    private SecurityContext CreateContextWithoutOwningClient ()
    {
      return new SecurityContext (typeof (Order), "owner", "UID: testOwningGroup", null , new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningClient ()
    {
      return new SecurityContext (typeof (Order), "owner", "UID: testOwningGroup", "UID: NotExistingClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithoutOwningGroup ()
    {
      return new SecurityContext (typeof (Order), "owner", null, "UID: testClient", new Dictionary<string, Enum> (), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningGroup ()
    {
      return new SecurityContext (typeof (Order), "owner", "UID: NotExistingGroup", "UID: testClient", new Dictionary<string, Enum> (), new Enum[0]);
    }
  }
}
