using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionCollectionTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private MockRepository _mockRepository;
    private ClientTransactionExtensionCollection _collection;
    private ClientTransactionExtensionCollection _collectionWithExtensions;
    private IClientTransactionExtension _extension1;
    private IClientTransactionExtension _extension2;

    private Order _order;
    private DataContainer _dataContainer;
    private PropertyValue _propertyValue;

    // construction and disposing

    public ClientTransactionExtensionCollectionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      _collection = new ClientTransactionExtensionCollection ();
      _extension1 = _mockRepository.CreateMock<IClientTransactionExtension> ();
      _extension2 = _mockRepository.CreateMock<IClientTransactionExtension> ();

      _collectionWithExtensions = new ClientTransactionExtensionCollection ();
      _collectionWithExtensions.Add ("Name1", _extension1);
      _collectionWithExtensions.Add ("Name2", _extension2);

      _order = new Order ();
      _dataContainer = _order.DataContainer;
      _propertyValue = _dataContainer.PropertyValues["OrderNumber"];
    }

    [Test]
    public void Add ()
    {
      Assert.AreEqual (0, _collection.Count);
      _collection.Add ("Name", _extension1);
      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void Insert ()
    {
      _collection.Add ("Name1", _extension1);
      Assert.AreEqual (1, _collection.Count);
      Assert.AreSame (_extension1, _collection[0]);

      _collection.Insert (0, "Name2", _extension2);
      Assert.AreEqual (2, _collection.Count);
      Assert.AreSame (_extension2, _collection[0]);
      Assert.AreSame (_extension1, _collection[1]);
    }

    [Test]
    public void Remove ()
    {
      _collection.Add ("Name", _extension1);
      Assert.AreEqual (1, _collection.Count);
      _collection.Remove ("Name");
      Assert.AreEqual (0, _collection.Count);
      _collection.Remove ("Name");
      //expectation: no exception
    }

    [Test]
    public void Indexer ()
    {
      _collection.Add ("Name1", _extension1);
      _collection.Add ("Name2", _extension2);
      Assert.AreSame (_extension1, _collection[0]);
      Assert.AreSame (_extension2, _collection[1]);
    }

    [Test]
    public void IndexerWithName ()
    {
      _collection.Add ("Name1", _extension1);
      _collection.Add ("Name2", _extension2);
      Assert.AreSame (_extension1, _collection["Name1"]);
      Assert.AreSame (_extension2, _collection["Name2"]);
    }

    [Test]
    public void IndexOf ()
    {
      _collection.Add ("Name", _extension1);

      Assert.AreEqual (0, _collection.IndexOf ("Name"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "An extension with name 'Name' is already part of the collection.\r\nParameter name: extensionName")]
    public void AddWithDuplicateName ()
    {
      _collection.Add ("Name", _extension1);
      _collection.Add ("Name", _extension2);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "An extension with name 'Name' is already part of the collection.\r\nParameter name: extensionName")]
    public void InsertWithDuplicateName ()
    {
      _collection.Insert (0, "Name", _extension1);
      _collection.Insert (0, "Name", _extension2);
    }

    //TODO check with ML if same extension may be added to the collection with different name. 

    [Test]
    public void PropertyChanging ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.PropertyValueChanging (_dataContainer, _propertyValue, 0, 1);
        _extension2.PropertyValueChanging (_dataContainer, _propertyValue, 0, 1);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueChanging (_dataContainer, _propertyValue, 0, 1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyChanged ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.PropertyValueChanged (_dataContainer, _propertyValue, 0, 1);
        _extension2.PropertyValueChanged (_dataContainer, _propertyValue, 0, 1);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueChanged (_dataContainer, _propertyValue, 0, 1);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyReading ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.PropertyValueReading (_dataContainer, _propertyValue, ValueAccess.Original);
        _extension2.PropertyValueReading (_dataContainer, _propertyValue, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueReading (_dataContainer, _propertyValue, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PropertyRead ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.PropertyValueRead (_dataContainer, _propertyValue, 0, ValueAccess.Original);
        _extension2.PropertyValueRead (_dataContainer, _propertyValue, 0, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.PropertyValueRead (_dataContainer, _propertyValue, 0, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChanging ()
    {
      OrderTicket orderTicket = _order.OrderTicket;
      OrderTicket newOrderTicket = new OrderTicket ();

      _mockRepository.BackToRecord (_extension1);
      _mockRepository.BackToRecord (_extension2);

      using (_mockRepository.Ordered ())
      {
        _extension1.RelationChanging (_order, "OrderNumber", orderTicket, newOrderTicket);
        _extension2.RelationChanging (_order, "OrderNumber", orderTicket, newOrderTicket);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationChanging (_order, "OrderNumber", orderTicket, newOrderTicket);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationChanged ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.RelationChanged (_order, "OrderNumber");
        _extension2.RelationChanged (_order, "OrderNumber");
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationChanged (_order, "OrderNumber");

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NewObjectCreating ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.NewObjectCreating (typeof(Order));
        _extension2.NewObjectCreating (typeof (Order));
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.NewObjectCreating (typeof (Order));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleting ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.ObjectDeleting (_order);
        _extension2.ObjectDeleting (_order);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectDeleting (_order);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectDeleted ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.ObjectDeleted (_order);
        _extension2.ObjectDeleted (_order);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectDeleted (_order);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Committing ()
    {
      DomainObjectCollection domainObjectCollection = new DomainObjectCollection ();
      using (_mockRepository.Ordered ())
      {
        _extension1.Committing (domainObjectCollection);
        _extension2.Committing (domainObjectCollection);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.Committing (domainObjectCollection);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Committed ()
    {
      DomainObjectCollection domainObjectCollection = new DomainObjectCollection ();
      using (_mockRepository.Ordered ())
      {
        _extension1.Committed (domainObjectCollection);
        _extension2.Committed (domainObjectCollection);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.Committed (domainObjectCollection);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RollingBack ()
    {
      DomainObjectCollection domainObjectCollection = new DomainObjectCollection ();
      using (_mockRepository.Ordered ())
      {
        _extension1.RollingBack (domainObjectCollection);
        _extension2.RollingBack (domainObjectCollection);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RollingBack (domainObjectCollection);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RolledBack ()
    {
      DomainObjectCollection domainObjectCollection = new DomainObjectCollection ();
      using (_mockRepository.Ordered ())
      {
        _extension1.RolledBack (domainObjectCollection);
        _extension2.RolledBack (domainObjectCollection);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RolledBack (domainObjectCollection);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ObjectsLoaded ()
    {
      DomainObjectCollection loadedDomainObjects = new DomainObjectCollection ();
      loadedDomainObjects.Add (_order);

      using (_mockRepository.Ordered ())
      {
        _extension1.ObjectsLoaded (loadedDomainObjects);
        _extension2.ObjectsLoaded (loadedDomainObjects);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.ObjectsLoaded (loadedDomainObjects);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void FilterQueryResult ()
    {
      DomainObjectCollection domainObjects = new DomainObjectCollection ();
      Query query = new Query ("OrderQuery");

      using (_mockRepository.Ordered ())
      {
        _extension1.FilterQueryResult (domainObjects, query);
        _extension2.FilterQueryResult (domainObjects, query);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.FilterQueryResult (domainObjects, query);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationReading ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension1.RelationReading (_order, "OrderItems", ValueAccess.Current);
        _extension2.RelationReading (_order, "OrderItems", ValueAccess.Current);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationReading (_order, "OrderItems", ValueAccess.Current);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationReadWithOneToOneRelation ()
    {
      OrderTicket orderTicket = _order.OrderTicket;

      using (_mockRepository.Ordered ())
      {
        _extension1.RelationRead (_order, "OrderTicket", orderTicket, ValueAccess.Original);
        _extension2.RelationRead (_order, "OrderTicket", orderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationRead (_order, "OrderTicket", orderTicket, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RelationReadWithOneToManyRelation ()
    {
      DomainObjectCollection orderItems = _order.OrderItems;

      using (_mockRepository.Ordered ())
      {
        _extension1.RelationRead (_order, "OrderItems", orderItems, ValueAccess.Original);
        _extension2.RelationRead (_order, "OrderItems", orderItems, ValueAccess.Original);
      }

      _mockRepository.ReplayAll ();

      _collectionWithExtensions.RelationRead (_order, "OrderItems", orderItems, ValueAccess.Original);

      _mockRepository.VerifyAll ();
    }

  }
}
