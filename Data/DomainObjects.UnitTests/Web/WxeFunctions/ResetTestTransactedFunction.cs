using System;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  public class ResetTestTransactedFunction : WxeTransactedFunction
  {
    private void Step1 ()
    {
      ClientTransaction transactionBefore = ClientTransactionScope.CurrentTransaction;
      Order order = Order.GetObject (new DomainObjectIDs ().Order1);
      order.OrderNumber = 7;
      transactionBefore.Rollback ();

      ResetTransaction ();
      
      Assert.AreNotEqual (transactionBefore, ClientTransactionScope.CurrentTransaction);
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.AreEqual (1, order.OrderNumber);
    }
  }
}