using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.UnitTests.TestDomain;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.UnitTests.AccessControl
{
  [TestFixture]
  public class SecurityTokenBuilderTest
  {
    [Test]
    public void Create_AbstractRolesEmpty ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTwoAbstractRoleDefinitions ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateStatelessContext ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, context);

      Assert.AreEqual (0, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidAbstractRole ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTwoAbstractRoleDefinitions ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithQualityManagerRole ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, context);

      Assert.AreEqual (1, token.AbstractRoles.Count);
      Assert.AreEqual ("QualityManager|Rubicon.SecurityManager.Domain.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.Domain.UnitTests", token.AbstractRoles[0].Name);
    }

    [Test]
    public void Create_WithValidAbstractRoles ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTwoAbstractRoleDefinitions ();
      ClientTransaction transaction = new ClientTransaction ();
      SecurityContext context = CreateContextWithQualityManagerAndDeveloperRoles ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (transaction, context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    private SecurityContext CreateStatelessContext ()
    {
      return new SecurityContext (typeof (Order), "owner", "ownerGroup", "ownerClient", new Dictionary<string, Enum> (), new Enum[0]);
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
