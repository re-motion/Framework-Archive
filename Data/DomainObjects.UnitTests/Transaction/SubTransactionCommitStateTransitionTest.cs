using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  [Ignore ("TODO: FS - SubTransactions Commit")]
  public class SubTransactionCommitStateTransitionTest : ClientTransactionStateTransitionBaseTest
  {
    [Test]
    public void CommitRootChangedSubChanged ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.AreEqual (StateType.Changed, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        ++obj.OrderNumber;
        Assert.AreEqual (StateType.Changed, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootChangedSubUnchanged ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.AreEqual (StateType.Changed, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootChangedSubDeleted ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.AreEqual (StateType.Changed, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        obj.Delete ();
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.IsTrue (obj.IsDiscarded);
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootUnchangedSubChanged ()
    {
      Order obj = GetUnchanged ();
      Assert.AreEqual (StateType.Unchanged, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        ++obj.OrderNumber;
        Assert.AreEqual (StateType.Changed, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootUnchangedSubUnchanged ()
    {
      Order obj = GetUnchanged ();
      Assert.AreEqual (StateType.Unchanged, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
    }

    [Test]
    public void CommitRootUnchangedSubDeleted ()
    {
      Order obj = GetUnchanged ();
      Assert.AreEqual (StateType.Unchanged, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        obj.Delete();
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootNewSubChanged ()
    {
      Order obj = GetNewUnchanged ();
      Assert.AreEqual (StateType.New, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        ++obj.OrderNumber;
        Assert.AreEqual (StateType.Changed, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void CommitRootNewSubUnchanged ()
    {
      Order obj = GetNewUnchanged ();
      Assert.AreEqual (StateType.New, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void CommitRootNewSubDeleted()
    {
      Order obj = GetNewUnchanged ();
      Assert.AreEqual (StateType.New, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        obj.Delete();
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.IsTrue (obj.IsDiscarded);
    }

    [Test]
    public void CommitRootDeletedSubDiscarded ()
    {
      Order obj = GetDeleted ();
      Assert.AreEqual (StateType.Deleted, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.IsTrue (obj.IsDiscarded);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootDiscardedSubDiscarded ()
    {
      Order obj = GetDiscarded ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.IsTrue (obj.IsDiscarded);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.IsTrue (obj.IsDiscarded);
    }

    [Test]
    public void CommitRootUnknownSubChanged ()
    {
      Order obj;
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        obj = GetChangedThroughPropertyValue ();
        Assert.AreEqual (StateType.Changed, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootUnknownSubUnchanged ()
    {
      Order obj;
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        obj = GetUnchanged();
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.IsNull (ClientTransactionMock.DataManager.DataContainerMap[obj.ID]);
    }

    [Test]
    public void CommitRootUnknownSubNew ()
    {
      Order obj;
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        obj = GetNewUnchanged ();
        Assert.AreEqual (StateType.New, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void CommitRootUnknownSubDeleted ()
    {
      Order obj;
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        obj = GetDeleted ();
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootUnknownSubDiscarded ()
    {
      Order obj;
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        obj = GetDiscarded ();
        Assert.IsTrue (obj.IsDiscarded);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.IsNull (ClientTransactionMock.DataManager.DataContainerMap[obj.ID]);
    }
  }
}