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
    [Test]
    public void Find_EmptyResult ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection result = AbstractRoleDefinition.Find (transaction, new EnumWrapper[0]);

      Assert.IsEmpty (result);
    }

    [Test]
    public void Find_ValidAbstractRole ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection result = AbstractRoleDefinition.Find (transaction, new EnumWrapper[] { new EnumWrapper (ProjectRole.QualityManager) });

      Assert.AreEqual (1, result.Count);
      Assert.AreEqual ("QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.UnitTests", ((AbstractRoleDefinition) result[0]).Name);
    }

    [Test]
    public void FindAll_EmptyResult ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyTestData ();

      ClientTransaction transaction = new ClientTransaction ();
      DomainObjectCollection result = AbstractRoleDefinition.FindAll (transaction);

      Assert.AreEqual (0, result.Count);
    }

    [Test]
    public void FindAll_TwoFound ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();

      ClientTransaction transaction = new ClientTransaction ();
      DomainObjectCollection result = AbstractRoleDefinition.FindAll (transaction);

      Assert.AreEqual (2, result.Count);
    }

    [Test]
    public void Get_DisplayName ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AbstractRoleDefinition abstractRole = new AbstractRoleDefinition (transaction);
      abstractRole.Name = "Value|Namespace.TypeName, Assembly";

      Assert.AreEqual ("Value|Namespace.TypeName, Assembly", abstractRole.DisplayName);
    }
  }
}
