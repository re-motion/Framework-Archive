using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

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
    public void FindByUnqiueIdentifier_ValidGroup ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      Group foundGroup = Group.FindByUnqiueIdentifier (_transaction, "UnqiueIdentifier: Testgroup");

      Assert.AreEqual ("UnqiueIdentifier: Testgroup", foundGroup.UniqueIdentifier);
    }

    [Test]
    public void FindByUnqiueIdentifier_NotExistingGroup ()
    {
      _dbFixtures.CreateOrganizationalStructure ();

      Group foundGroup = Group.FindByUnqiueIdentifier (_transaction, "UnqiueIdentifier: NotExistingGroup");

      Assert.IsNull (foundGroup);
    }

    [Test]
    public void Find_GroupsByClientID ()
    {
      _dbFixtures.CreateGroupsWithDifferentClients ();

      DomainObjectCollection groups = Group.FindByClientID (_dbFixtures.CurrentClient.ID, _transaction);

      Assert.AreEqual (2, groups.Count);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UniqueIdentifier_SameIdentifierTwice ()
    {
      _dbFixtures.CreateEmptyDomain ();

      ClientTransaction transaction1 = new ClientTransaction ();
      Client client1 = _dbFixtures.CreateClient (transaction1, "NewClient1");
      Group group1 = _dbFixtures.CreateGroup (transaction1, "NewGroup1", "UnqiueIdentifier: NewGroup", null, client1);

      transaction1.Commit ();

      ClientTransaction transaction2 = new ClientTransaction ();
      Client client2 = _dbFixtures.CreateClient (transaction2, "NewClient2");
      Group group2 = _dbFixtures.CreateGroup (transaction2, "NewGroup2", "UnqiueIdentifier: NewGroup", null, client2);

      transaction2.Commit ();
    }
  }
}
