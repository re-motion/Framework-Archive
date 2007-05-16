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
      dbFixtures.CreateOrganizationalStructureWithTwoTenants();
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection groupTypes = GroupType.FindAll (transaction);

      Assert.AreEqual (2, groupTypes.Count);
    }

    [Test]
    public void GetDisplayName ()
    {
      GroupType groupType = GroupType.NewObject (new ClientTransaction());
      groupType.Name = "GroupTypeName";

      Assert.AreEqual ("GroupTypeName", groupType.DisplayName);
    }
  }
}
