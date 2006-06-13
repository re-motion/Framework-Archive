using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Security;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.UnitTests.TestDomain;

namespace Rubicon.SecurityManager.Domain.UnitTests.Metadata
{
  [TestFixture]
  public class AbstractRoleDefinitionTest : DomainTest
  {
    [Test]
    public void Find_EmptyResult ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTwoAbstractRoleDefinitions ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection result = AbstractRoleDefinition.Find (transaction, new EnumWrapper[0]);

      Assert.IsEmpty (result);
    }

    [Test]
    public void Find_ValidAbstractRole ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTwoAbstractRoleDefinitions ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection result = AbstractRoleDefinition.Find (transaction, new EnumWrapper[] { new EnumWrapper (ProjectRoles.QualityManager) });

      Assert.AreEqual (1, result.Count);
      Assert.AreEqual ("QualityManager", ((AbstractRoleDefinition) result[0]).Name);
    }
  }
}
