using System;
using NUnit.Framework;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionMixinTest
  {
    [Test]
    public void ClientTransactionCanBeMixed ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClientTransaction), typeof (InvertingClientTransactionMixin)))
      {
        ClientTransaction mixedTransaction = ClientTransaction.NewTransaction ();
        Assert.IsNotNull (mixedTransaction);
        Assert.IsNotNull (Mixin.Get<InvertingClientTransactionMixin> (mixedTransaction));
      }
    }

    [Test]
    public void SubTransactionsAlsoMixed ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClientTransaction), typeof (InvertingClientTransactionMixin)))
      {
        ClientTransaction mixedTransaction = ClientTransaction.NewTransaction ();
        ClientTransaction mixedSubTransaction = mixedTransaction.CreateSubTransaction ();
        Assert.IsNotNull (mixedSubTransaction);
        Assert.IsNotNull (Mixin.Get<InvertingClientTransactionMixin> (mixedSubTransaction));
      }
    }

    [Test]
    public void TransactionMethodsCanBeOverridden ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClientTransaction), typeof (InvertingClientTransactionMixin)))
      {
        ClientTransaction invertedTransaction = ClientTransaction.NewTransaction();

        bool committed = false;
        bool rolledBack = false;
        invertedTransaction.Committed += delegate { committed = true; };
        invertedTransaction.RolledBack += delegate { rolledBack = true; };

        Assert.IsFalse (rolledBack);
        Assert.IsFalse (committed);

        invertedTransaction.Commit();

        Assert.IsTrue (rolledBack);
        Assert.IsFalse (committed);

        rolledBack = false;
        invertedTransaction.Rollback();

        Assert.IsFalse (rolledBack);
        Assert.IsTrue (committed);
      }
    }

    [Test]
    public void MixinCanAddInterface ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClientTransaction), typeof (ClientTransactionWithIDMixin)))
      {
        IClientTransactionWithID transactionWithID = (IClientTransactionWithID) ClientTransaction.NewTransaction ();
        Assert.AreEqual (transactionWithID.ID.ToString (), transactionWithID.ToString ());
        IClientTransactionWithID subTransactionWithID = (IClientTransactionWithID) transactionWithID.AsClientTransaction.CreateSubTransaction ();
        Assert.AreNotEqual (transactionWithID.ID, subTransactionWithID.ID);
        Assert.AreEqual (transactionWithID.ID, ((IClientTransactionWithID) subTransactionWithID.AsClientTransaction.ParentTransaction).ID);
      }
    }
  }
}