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
      ClientTransaction.SetCurrent (null);
    }

    [TearDown]
    public void TearDown ()
    {
      ClientTransaction.SetCurrent (null);
    }

    [Test]
    public void ScopeSetsAndResetsCurrentTransaction ()
    {
      ClientTransaction clientTransaction = new ClientTransaction ();
      Assert.AreNotSame (clientTransaction, ClientTransaction.Current);
      using (new ClientTransactionScope (clientTransaction))
      {
        Assert.AreSame (clientTransaction, ClientTransaction.Current);
      }
      Assert.AreNotSame (clientTransaction, ClientTransaction.Current);
    }

    [Test]
    public void ScopeSetsNullTransaction ()
    {
      ClientTransaction.SetCurrent (new ClientTransaction ());
      Assert.IsTrue (ClientTransaction.HasCurrent);
      using (new ClientTransactionScope (null))
      {
        Assert.IsFalse (ClientTransaction.HasCurrent);
      }
      Assert.IsTrue (ClientTransaction.HasCurrent);
    }

    [Test]
    public void NestedScopes ()
    {
      ClientTransaction clientTransaction1 = new ClientTransaction ();
      ClientTransaction clientTransaction2 = new ClientTransaction ();
      ClientTransaction original = ClientTransaction.Current;
      
      Assert.AreNotSame (clientTransaction1, original);
      Assert.AreNotSame (clientTransaction2, original);
      Assert.IsNotNull (original);

      using (new ClientTransactionScope (clientTransaction1))
      {
        Assert.AreSame (clientTransaction1, ClientTransaction.Current);
        using (new ClientTransactionScope (clientTransaction2))
        {
          Assert.AreSame (clientTransaction2, ClientTransaction.Current);
        }
        Assert.AreSame (clientTransaction1, ClientTransaction.Current);
      }
      Assert.AreSame (original, ClientTransaction.Current);
    }

    [Test]
    public void LeavesEmptyTransaction ()
    {
      Assert.IsFalse (ClientTransaction.HasCurrent);
      using (new ClientTransactionScope (new ClientTransaction ()))
      {
        Assert.IsTrue (ClientTransaction.HasCurrent);
      }
      Assert.IsFalse (ClientTransaction.HasCurrent);
    }
  }
}
