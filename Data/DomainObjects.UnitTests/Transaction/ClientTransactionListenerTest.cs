using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

using Mock_Is = Rhino.Mocks.Constraints.Is;
using Mock_Property = Rhino.Mocks.Constraints.Property;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionListenerTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionListener _listener;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository ();
      
      _listener = _mockRepository.CreateMock<IClientTransactionListener> ();

      ClientTransactionMock.AddListener (_listener);
    }

    private void Expect (Proc expectation, Proc triggeringCode)
    {
      _mockRepository.BackToRecordAll ();

      using (_mockRepository.Ordered ())
      {
        expectation();
      }

      _mockRepository.ReplayAll ();

      triggeringCode ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NewObjectCreating ()
    {
      Expect (delegate
        {
          _listener.NewObjectCreating (typeof (ClassWithAllDataTypes));

          _listener.DataContainerMapRegistering (null);
          LastCall.IgnoreArguments ();
        },
        delegate { ClassWithAllDataTypes.NewObject (); });
    }

    [Test]
    public void ObjectsLoadingObjectsLoaded ()
    {
      Expect (delegate
        {
          _listener.ObjectLoading (DomainObjectIDs.ClassWithAllDataTypes1);

          _listener.DataContainerMapRegistering (null);
          LastCall.IgnoreArguments ();

          _listener.ObjectsLoaded (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
        },
        delegate { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1); });
    }

    [Test]
    public void ObjectsObjectDeletingObjectsDeletedRelationEndPointMapPerformingDelete2 ()
    {
      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      Expect (delegate
        {
          _listener.ObjectDeleting (cwadt);

          _listener.RelationEndPointMapPerformingDelete (null);
          LastCall.Constraints (Mock_Property.Value ("Length", 0));

          _listener.ObjectDeleted (cwadt);
        },
        delegate { cwadt.Delete (); });
    }

    [Test]
    public void PropertyValueReadingPropertyValueRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      Expect (delegate
        {
          _listener.PropertyValueReading (order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], ValueAccess.Current);
          _listener.PropertyValueRead (order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], orderNumber, ValueAccess.Current);
        },
        delegate { int i = order.OrderNumber; });
    }

    [Test]
    public void PropertyValueChangingPropertyValueChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      int orderNumber = order.OrderNumber;

      Expect (delegate
        {
          _listener.PropertyValueChanging (order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], orderNumber, 43);
          _listener.PropertyValueChanged (order.InternalDataContainer,
              order.InternalDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"], orderNumber, 43);
        },
        delegate { order.OrderNumber = 43; });
    }

    [Test]
    public void RelationReadingRelationRead ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer customer = order.Customer;
      ObjectList<OrderItem> orderItems = order.OrderItems;

      Expect (delegate
        {
          _listener.RelationReading (order, typeof (Order).FullName + ".Customer", ValueAccess.Current);
          _listener.RelationRead (order, typeof (Order).FullName + ".Customer", customer, ValueAccess.Current);

          _listener.RelationReading (order, typeof (Order).FullName + ".OrderItems", ValueAccess.Current);
          _listener.RelationRead (order, typeof (Order).FullName + ".OrderItems", orderItems, ValueAccess.Current);
          LastCall.Constraints (Mock_Is.Equal (order), Mock_Is.Equal (typeof (Order).FullName + ".OrderItems"),
              Mock_Property.Value ("Count", orderItems.Count), Mock_Is.Equal (ValueAccess.Current));
        },
        delegate
        {
          Customer c = order.Customer;
          ObjectList<OrderItem> i = order.OrderItems;
        });
    }

    [Test]
    public void RelationChangingRelationChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Customer customer = order.Customer;
      Customer newCustomer = Customer.NewObject();

      Expect (delegate
        {
          _listener.ObjectLoading (null);
          LastCall.IgnoreArguments().Repeat.Any ();
          _listener.ObjectsLoaded (null);
          LastCall.IgnoreArguments ().Repeat.Any ();

          _listener.DataContainerMapRegistering (null);
          LastCall.IgnoreArguments ();

          _listener.RelationEndPointMapRegistering (null);
          LastCall.IgnoreArguments ().Repeat.Any();

          _listener.RelationChanging (order, typeof (Order).FullName + ".Customer", customer, newCustomer);
          _listener.RelationChanging (newCustomer, typeof (Customer).FullName + ".Orders", null, order);
          _listener.RelationChanging (customer, typeof (Customer).FullName + ".Orders", order, null);
          _listener.RelationChanged (order, typeof (Order).FullName + ".Customer");
          _listener.RelationChanged (newCustomer, typeof (Customer).FullName + ".Orders");
          _listener.RelationChanged (customer, typeof (Customer).FullName + ".Orders");
        },
        delegate
        {
          order.Customer = newCustomer;
        });
    }

    [Test]
    public void FilterQueryResult ()
    {
      Query query = new Query ("StoredProcedureQuery");
      OrderCollection orders = (OrderCollection) ClientTransactionMock.QueryManager.GetCollection (query);

      Expect (delegate
        {
          _listener.ObjectLoading (null);
          LastCall.IgnoreArguments ().Repeat.Any ();
          _listener.ObjectsLoaded (null);
          LastCall.IgnoreArguments ().Repeat.Any ();

          _listener.FilterQueryResult (null, null);
          LastCall.Constraints (Mock_Property.Value ("Count", orders.Count), Mock_Is.Equal (query));
        },
        delegate
        {
          ClientTransactionMock.QueryManager.GetCollection (query);
        });
    }


    [Test]
    public void TransactionCommittingTransactionCommitted ()
    {
      SetDatabaseModifyable ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      Expect (delegate
        {
          _listener.TransactionCommitting (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
          _listener.TransactionCommitted (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
        },
        delegate
        {
          ClientTransactionMock.Commit ();
        });
    }

    [Test]
    public void TransactionRollingBackTransactionRolledBack ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;

      Expect (delegate
        {
          _listener.TransactionRollingBack (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
          _listener.TransactionRolledBack (null);
          LastCall.Constraints (Mock_Property.Value ("Count", 1));
        },
        delegate
        {
          ClientTransactionMock.Rollback ();
        });
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Expect (delegate
        {
          _listener.RelationReading (null, null, ValueAccess.Current);
          LastCall.IgnoreArguments ();
          _listener.ObjectLoading (null);
          LastCall.IgnoreArguments ();
          _listener.DataContainerMapRegistering (null);
          LastCall.IgnoreArguments ();

          _listener.RelationEndPointMapRegistering (null);
          LastCall.Constraints (Mock_Property.Value ("ObjectID", DomainObjectIDs.Customer1));

          _listener.ObjectsLoaded (null);
          LastCall.IgnoreArguments ();
          _listener.RelationRead (null, null, (DomainObject) null, ValueAccess.Current);
          LastCall.IgnoreArguments ();
        },
        delegate
        {
          Customer customer = order.Customer;
        });
    }

    [Test]
    public void RelationEndPointMapUnregisteringDataManagerMarkingObjectDiscardedDataContainerMapUnregistering ()
    {
      Order order = Order.NewObject ();

      Expect (delegate
        {
          _listener.ObjectDeleting (null);
          LastCall.IgnoreArguments ();
          _listener.RelationEndPointMapPerformingDelete (null);
          LastCall.IgnoreArguments ();

          _listener.RelationEndPointMapUnregistering (null);
          LastCall.Constraints (Mock_Property.Value ("ObjectID", order.ID)).Repeat.Times (4); // four related objects/object collections in Order

          _listener.DataContainerMapUnregistering (order.InternalDataContainer);

          _listener.DataManagerMarkingObjectDiscarded (order.ID);

          _listener.ObjectDeleted (null);
          LastCall.IgnoreArguments ();
        },
        delegate
        {
          order.Delete ();
        });
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      Expect (delegate
        {
          _listener.ObjectLoading (null);
          LastCall.IgnoreArguments ();

          _listener.DataContainerMapRegistering (null);
          LastCall.Constraints (Mock_Property.Value ("ID", DomainObjectIDs.ClassWithAllDataTypes1));

          _listener.ObjectsLoaded (null);
          LastCall.IgnoreArguments ();
        },
        delegate
        {
          ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        });
    }
  }
}
