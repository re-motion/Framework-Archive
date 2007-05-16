using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTypePositionTest : DomainTest
  {
    [Test]
    public void GetDisplayName_WithGroupTypeAndPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();

      Assert.AreEqual ("GroupTypeName / PositionName", groupTypePosition.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithoutPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();
      groupTypePosition.Position = null;

      Assert.AreEqual ("GroupTypeName / ", groupTypePosition.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithoutGroupType ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();
      groupTypePosition.GroupType = null;

      Assert.AreEqual (" / PositionName", groupTypePosition.DisplayName);
    }

    [Test]
    public void GetDisplayName_WithoutGroupTypeAndWithoutPosition ()
    {
      GroupTypePosition groupTypePosition = CreateGroupTypePosition ();
      groupTypePosition.GroupType = null;
      groupTypePosition.Position = null;

      Assert.AreEqual (" / ", groupTypePosition.DisplayName);
    }

    private static GroupTypePosition CreateGroupTypePosition ()
    {
      OrganizationalStructureFactory factory = new OrganizationalStructureFactory();
      ClientTransaction clientTransaction = new ClientTransaction ();

      GroupTypePosition groupTypePosition = GroupTypePosition.NewObject (clientTransaction);
      
      groupTypePosition.GroupType = GroupType.NewObject (clientTransaction);
      groupTypePosition.GroupType.Name = "GroupTypeName";

      groupTypePosition.Position = factory.CreatePosition (clientTransaction);
      groupTypePosition.Position.Name = "PositionName";

      return groupTypePosition;
    }
  }
}