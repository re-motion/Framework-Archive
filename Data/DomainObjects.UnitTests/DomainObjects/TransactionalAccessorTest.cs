using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class TransactionalAccessorTest : ClientTransactionBaseTest
  {
    private TransactionalAccessor<T> CreateTransactionalAccessor<T> (PropertyAccessor accessor)
    {
      return (TransactionalAccessor<T>) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (TransactionalAccessor<T>), accessor);
    }

    [Test]
    public void TransactionalAccessorGetValue ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor orderNumberAccessor = order.Properties[typeof (Order), "OrderNumber"];

      ClientTransaction secondTransaction = ClientTransaction.NewTransaction ();
      secondTransaction.EnlistDomainObject (order);

      orderNumberAccessor.SetValue (12);
      orderNumberAccessor.SetValueTx (secondTransaction, 13);

      TransactionalAccessor<int> orderNumberTx = CreateTransactionalAccessor<int> (orderNumberAccessor);
      Assert.AreEqual (12, orderNumberTx[ClientTransactionMock]);
      Assert.AreEqual (13, orderNumberTx[secondTransaction]);
    }

    [Test]
    public void TransactionalAccessorSetValue ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor orderNumberAccessor = order.Properties[typeof (Order), "OrderNumber"];

      ClientTransaction secondTransaction = ClientTransaction.NewTransaction ();
      secondTransaction.EnlistDomainObject (order);

      TransactionalAccessor<int> orderNumberTx = CreateTransactionalAccessor<int> (orderNumberAccessor);

      orderNumberTx[ClientTransactionMock] = 12;
      orderNumberTx[secondTransaction] = 13;

      
      Assert.AreEqual (12, orderNumberAccessor.GetValue<int>());
      Assert.AreEqual (13, orderNumberAccessor.GetValueTx<int> (secondTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = 
        "Actual type 'System.Int32' of property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber' does not match expected type "
        + "'System.Double'.")]
    public void TransactionalAccessorThrowsOnInvalidT ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor orderNumberAccessor = order.Properties[typeof (Order), "OrderNumber"];

      CreateTransactionalAccessor<double> (orderNumberAccessor);
    }

    [Test]
    public void GetValueSetValueOnObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      ClientTransaction secondTransaction = ClientTransaction.NewTransaction ();
      secondTransaction.EnlistDomainObject (order);

      order.OrderNumberTx[ClientTransactionMock] = 7;
      order.OrderNumberTx[secondTransaction] = 56;

      Assert.AreEqual (7, order.OrderNumberTx[ClientTransactionMock]);
      Assert.AreEqual (56, order.OrderNumberTx[secondTransaction]);
    }
  }
}