using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.UnitTests;
using Rubicon.SecurityManager.UnitTests.Domain;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTypeTest : DomainTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    [Test]
    public void Find_GroupTypesByClientID ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateGroupTypesWithDifferentClients ();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection groupTypes = GroupType.FindByClientID (dbFixtures.CurrentClient.ID);

      Assert.AreEqual (2, groupTypes.Count);
    }
  }
}
