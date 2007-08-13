using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTypeTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();

      new ClientTransactionScope();
    }

    [Test]
    public void FindAll ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateOrganizationalStructureWithTwoTenants (ClientTransaction.NewTransaction());

      DomainObjectCollection groupTypes = GroupType.FindAll ();

      Assert.AreEqual (2, groupTypes.Count);
    }

    [Test]
    public void GetDisplayName ()
    {
      GroupType groupType = GroupType.NewObject();
      groupType.Name = "GroupTypeName";

      Assert.AreEqual ("GroupTypeName", groupType.DisplayName);
    }
  }
}
