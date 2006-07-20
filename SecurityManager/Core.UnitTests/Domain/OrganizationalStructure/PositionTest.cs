using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class PositionTest : DomainTest
  {
    [Test]
    public void Find_PositionsByClientID ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      Client exptectedClient = dbFixtures.CreateOrganizationalStructureWithTwoClients ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection positions = Position.FindByClientID (exptectedClient.ID, transaction);

      Assert.AreEqual (2, positions.Count);
    }

  }
}
