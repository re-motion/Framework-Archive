using System;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionScopeTest
  {
    [SetUp]
    public void SetUp ()
    {
      ClientTransactionScope.SetCurrentTransaction (null);
    }

    [TearDown]
    public void TearDown ()
    {
      ClientTransactionScope.SetCurrentTransaction (null);
    }

    [Test]
    public void ScopeSetsAndResetsCurrentTransaction ()
    {
      ClientTransaction clientTransaction = new ClientTransaction ();
      Assert.AreNotSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
      using (new ClientTransactionScope (clientTransaction))
      {
        Assert.AreSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreNotSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void ScopeSetsNullTransaction ()
    {
      ClientTransactionScope.SetCurrentTransaction (new ClientTransaction ());
      Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
      using (new ClientTransactionScope (null))
      {
        Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
      }
      Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
    }

    [Test]
    public void NestedScopes ()
    {
      ClientTransaction clientTransaction1 = new ClientTransaction ();
      ClientTransaction clientTransaction2 = new ClientTransaction ();
      ClientTransaction original = ClientTransactionScope.CurrentTransaction;
      
      Assert.AreNotSame (clientTransaction1, original);
      Assert.AreNotSame (clientTransaction2, original);
      Assert.IsNotNull (original);

      using (new ClientTransactionScope (clientTransaction1))
      {
        Assert.AreSame (clientTransaction1, ClientTransactionScope.CurrentTransaction);
        using (new ClientTransactionScope (clientTransaction2))
        {
          Assert.AreSame (clientTransaction2, ClientTransactionScope.CurrentTransaction);
        }
        Assert.AreSame (clientTransaction1, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreSame (original, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void LeavesEmptyTransaction ()
    {
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
      using (new ClientTransactionScope (new ClientTransaction ()))
      {
        Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
      }
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
    }
  }
}
