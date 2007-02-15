using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTypeTest : DomainTest
  {
    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructureWithTwoClients ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection groupTypes = GroupType.FindAll (transaction);

      Assert.AreEqual (2, groupTypes.Count);
    }
  }
}
