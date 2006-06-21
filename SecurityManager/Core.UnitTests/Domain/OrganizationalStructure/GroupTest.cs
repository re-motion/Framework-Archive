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
    [Test]
    public void Find_ValidGroup ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();

      Group foundGroup = Group.Find (transaction, "Testgroup");

      Assert.AreEqual ("Testgroup", foundGroup.Name);
    }

    [Test]
    public void Find_NotExistingGroup ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateTestData ();
      ClientTransaction transaction = new ClientTransaction ();

      Group foundGroup = Group.Find (transaction, "NotExistingGroup");

      Assert.IsNull (foundGroup);
    }

    [Test]
    public void Find_GroupsByClientID ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateGroupsWithDifferentClients ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection groups = Group.FindByClientID (dbFixtures.CurrentClient.ID);

      Assert.AreEqual (2, groups.Count);
    }
  }
}
