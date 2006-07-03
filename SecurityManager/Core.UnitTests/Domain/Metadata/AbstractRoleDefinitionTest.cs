using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Security;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.UnitTests.TestDomain;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class AbstractRoleDefinitionTest : DomainTest
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
    public void Find_EmptyResult ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      DomainObjectCollection result = AbstractRoleDefinition.Find (_transaction, new EnumWrapper[0]);

      Assert.IsEmpty (result);
    }

    [Test]
    public void Find_ValidAbstractRole ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      DomainObjectCollection result = AbstractRoleDefinition.Find (_transaction, new EnumWrapper[] { new EnumWrapper (ProjectRole.QualityManager) });

      Assert.AreEqual (1, result.Count);
      Assert.AreEqual ("QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.UnitTests", ((AbstractRoleDefinition) result[0]).Name);
    }

    [Test]
    public void FindAll_EmptyResult ()
    {
      _dbFixtures.CreateEmptyDomain ();

      DomainObjectCollection result = AbstractRoleDefinition.FindAll (_transaction);

      Assert.AreEqual (0, result.Count);
    }

    [Test]
    public void FindAll_TwoFound ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      DomainObjectCollection result = AbstractRoleDefinition.FindAll (_transaction);

      Assert.AreEqual (2, result.Count);
    }

    [Test]
    public void Get_DisplayName ()
    {
      AbstractRoleDefinition abstractRole = new AbstractRoleDefinition (_transaction);
      abstractRole.Name = "Value|Namespace.TypeName, Assembly";

      Assert.AreEqual ("Value|Namespace.TypeName, Assembly", abstractRole.DisplayName);
    }
  }
}
