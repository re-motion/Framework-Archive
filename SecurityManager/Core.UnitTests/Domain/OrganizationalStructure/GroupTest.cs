using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTest : DomainTest
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
    public void Find_ValidGroup ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      Group foundGroup = Group.Find (_transaction, "Testgroup");

      Assert.AreEqual ("Testgroup", foundGroup.Name);
    }

    [Test]
    public void Find_NotExistingGroup ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      Group foundGroup = Group.Find (_transaction, "NotExistingGroup");

      Assert.IsNull (foundGroup);
    }

    [Test]
    public void Find_GroupsByClientID ()
    {
      _dbFixtures.CreateGroupsWithDifferentClients ();

      DomainObjectCollection groups = Group.FindByClientID (_dbFixtures.CurrentClient.ID, _transaction);

      Assert.AreEqual (2, groups.Count);
    }
  }
}
