using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlEntryPriorityComparerTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();

      new ClientTransactionScope();
    }

    [Test]
    public void Compare_Equals ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      leftAce.Priority = 42;
      AccessControlEntry rightAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      rightAce.Priority = 42;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.AreEqual (0, comparer.Compare (leftAce, rightAce));
      Assert.AreEqual (0, comparer.Compare (rightAce, leftAce));
    }

    [Test]
    public void Compare_LeftIsLessThanRight ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      leftAce.Priority = 24;
      AccessControlEntry rightAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      rightAce.Priority = 42;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Less (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_LeftIsGreaterThanRight ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      leftAce.Priority = 42;
      AccessControlEntry rightAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      rightAce.Priority = 24;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Greater (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_LeftIsLessThanRightAndRightIsCalculated ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      leftAce.Priority = 2;
      AccessControlEntry rightAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      rightAce.User = UserSelection.Owner;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Less (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_RightIsNull ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      leftAce.Priority = 0;
      AccessControlEntry rightAce = null;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Greater (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_LeftIsNull ()
    {
      AccessControlEntry leftAce = null;
      AccessControlEntry rightAce = AccessControlEntry.NewObject (ClientTransactionScope.CurrentTransaction);
      rightAce.Priority = 0;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Less (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_BothAreNull ()
    {
      AccessControlEntry leftAce = null;
      AccessControlEntry rightAce = null;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.AreEqual (0, comparer.Compare (leftAce, rightAce));
    }
  }
}
