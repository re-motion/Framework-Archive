using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.UnitTests.TestDomain;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata.AbstractRoleDefinitionTests
{
  [TestFixture]
  public class TestWithTwoClients : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private ClientTransaction _transaction;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
    
      _dbFixtures = new DatabaseFixtures ();
      _dbFixtures.CreateOrganizationalStructureWithTwoClients ();
    }

    public override void SetUp ()
    {
      base.SetUp ();
      
      _transaction = new ClientTransaction ();
    }

    [Test]
    public void Find_EmptyResult ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.Find (_transaction, new EnumWrapper[0]);

      Assert.IsEmpty (result);
    }

    [Test]
    public void Find_ValidAbstractRole ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.Find (_transaction, new EnumWrapper[] { new EnumWrapper (ProjectRoles.QualityManager) });

      Assert.AreEqual (1, result.Count);
      Assert.AreEqual ("QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRoles, Rubicon.SecurityManager.UnitTests", ((AbstractRoleDefinition) result[0]).Name);
    }

    [Test]
    public void FindAll_TwoFound ()
    {
      DomainObjectCollection result = AbstractRoleDefinition.FindAll (_transaction);

      Assert.AreEqual (2, result.Count);
      for (int i = 0; i < result.Count; i++)
      {
        AbstractRoleDefinition abstractRole = (AbstractRoleDefinition) result[i];
        Assert.AreEqual (i, abstractRole.Index, "Wrong Index.");
      }
    }
  }
}
