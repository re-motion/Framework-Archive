using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlEntryPriorityComparerTest : DomainTest
  {
    [Test]
    public void Compare_Equals ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry leftAce = new AccessControlEntry (transaction);
      leftAce.Priority = 42;
      AccessControlEntry rightAce = new AccessControlEntry (transaction);
      rightAce.Priority = 42;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.AreEqual (0, comparer.Compare (leftAce, rightAce));
      Assert.AreEqual (0, comparer.Compare (rightAce, leftAce));
    }

    [Test]
    public void Compare_LeftIsLessThanRight ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry leftAce = new AccessControlEntry (transaction);
      leftAce.Priority = 24;
      AccessControlEntry rightAce = new AccessControlEntry (transaction);
      rightAce.Priority = 42;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Less (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_LeftIsGreaterThanRight ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry leftAce = new AccessControlEntry (transaction);
      leftAce.Priority = 42;
      AccessControlEntry rightAce = new AccessControlEntry (transaction);
      rightAce.Priority = 24;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Greater (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_LeftIsLessThanRightAndRightIsCalculated ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry leftAce = new AccessControlEntry (transaction);
      leftAce.Priority = 2;
      AccessControlEntry rightAce = new AccessControlEntry (transaction);
      rightAce.User = UserSelection.Owner;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Less (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_RightIsNull ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry leftAce = new AccessControlEntry (transaction);
      leftAce.Priority = 0;
      AccessControlEntry rightAce = null;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Greater (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_LeftIsNull ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry leftAce = null;
      AccessControlEntry rightAce = new AccessControlEntry (transaction);
      rightAce.Priority = 0;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Less (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_BothAreNull ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry leftAce = null;
      AccessControlEntry rightAce = null;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.AreEqual (0, comparer.Compare (leftAce, rightAce));
    }
  }
}
