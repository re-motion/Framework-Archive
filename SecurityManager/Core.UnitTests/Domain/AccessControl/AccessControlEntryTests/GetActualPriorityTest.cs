using System;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class GetActualPriorityTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
    }

    [Test]
    public void CustomPriority ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject (_testHelper.Transaction);
      ace.Priority = 42;

      Assert.AreEqual (42, ace.ActualPriority);
    }

    [Test]
    public void EmptyAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject (_testHelper.Transaction);

      Assert.AreEqual (0, ace.ActualPriority);
    }

    [Test]
    public void AceWithAbstractRole ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject (_testHelper.Transaction);
      ace.SpecificAbstractRole = AbstractRoleDefinition.NewObject (_testHelper.Transaction, Guid.NewGuid(), "Test", 42);

      Assert.AreEqual (AccessControlEntry.AbstractRolePriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithUser ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject (_testHelper.Transaction);
      ace.User = UserSelection.Owner;

      Assert.AreEqual (AccessControlEntry.UserPriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithGroup ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject (_testHelper.Transaction);
      ace.Group = GroupSelection.OwningGroup;

      Assert.AreEqual (AccessControlEntry.GroupPriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithClient ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject (_testHelper.Transaction);
      ace.Client = ClientSelection.OwningClient;

      Assert.AreEqual (AccessControlEntry.ClientPriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithUserAndGroup ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject (_testHelper.Transaction);
      ace.User = UserSelection.Owner;
      ace.Group = GroupSelection.OwningGroup;

      int expectedPriority = AccessControlEntry.UserPriority + AccessControlEntry.GroupPriority;
      Assert.AreEqual (expectedPriority, ace.ActualPriority);
    }

    [Test]
    public void AceWithUserAndAbstractRoleAndGroupAndClient ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject (_testHelper.Transaction);
      ace.User = UserSelection.Owner;
      ace.SpecificAbstractRole = AbstractRoleDefinition.NewObject (_testHelper.Transaction, Guid.NewGuid(), "Test", 42);
      ace.Group = GroupSelection.OwningGroup;
      ace.Client = ClientSelection.OwningClient;

      int expectedPriority = AccessControlEntry.UserPriority + AccessControlEntry.AbstractRolePriority + AccessControlEntry.GroupPriority
                             + AccessControlEntry.ClientPriority;

      Assert.AreEqual (expectedPriority, ace.ActualPriority);
    }
  }
}