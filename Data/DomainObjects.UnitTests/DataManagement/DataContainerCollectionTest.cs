using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class DataContainerCollectionTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private DataContainer _dataContainer;
  private DataContainerCollection _collection;

  // construction and disposing

  public DataContainerCollectionTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _dataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
    _collection = new DataContainerCollection ();
  }

  [Test]
  public void Add ()
  {
    _collection.Add (_dataContainer);
    Assert.AreEqual (1, _collection.Count);
  }

  [Test]
  public void ObjectIDIndexer ()
  {
    _collection.Add (_dataContainer);
    Assert.AreSame (_dataContainer, _collection[_dataContainer.ID]);  
  }

  [Test]
  public void NumericIndexer ()
  {
    _collection.Add (_dataContainer);
    Assert.AreSame (_dataContainer, _collection[0]);  
  }

  [Test]
  public void ContainsObjectIDTrue ()
  {
    _collection.Add (_dataContainer);
    Assert.IsTrue (_collection.Contains (_dataContainer.ID));
  }

  [Test]
  public void ContainsObjectIDFalse ()
  {
    Assert.IsFalse (_collection.Contains (_dataContainer.ID));
  }

  [Test]
  public void CopyConstructor ()
  {
    _collection.Add (_dataContainer);

    DataContainerCollection copiedCollection = new DataContainerCollection (_collection, false);

    Assert.AreEqual (1, copiedCollection.Count);
    Assert.AreSame (_dataContainer, copiedCollection[0]);
  }

  [Test]
  public void GetEmptyDifference ()
  {
    DataContainerCollection difference = _collection.GetDifference (new DataContainerCollection ());
    Assert.AreEqual (0, difference.Count);
  }

  [Test]
  public void GetDifferenceFromEmptySet ()
  {
    _collection.Add (_dataContainer);
    DataContainerCollection difference = _collection.GetDifference (new DataContainerCollection ());
    Assert.AreEqual (1, difference.Count);
    Assert.AreSame (_dataContainer, difference[0]);
  }

  [Test]
  public void GetDifference ()
  {
    DataContainer differentDataContainer = TestDataContainerFactory.CreateOrder2DataContainer ();
    
    _collection.Add (_dataContainer);
    _collection.Add (differentDataContainer);

    DataContainerCollection secondCollection = new DataContainerCollection ();
 
    secondCollection.Add (_dataContainer);

    DataContainerCollection difference = _collection.GetDifference (secondCollection);

    Assert.AreEqual (1, difference.Count);
    Assert.AreSame (differentDataContainer, difference[0]);
  }

  [Test]
  public void EmptyMerge ()
  {
    DataContainerCollection mergedCollection = _collection.Merge (new DataContainerCollection ());
    Assert.AreEqual (0, mergedCollection.Count);
  }

  [Test]
  public void MergeeCollectionAndEmptyCollection ()
  {
    _collection.Add (_dataContainer);
    DataContainerCollection mergedCollection = _collection.Merge (new DataContainerCollection ());

    Assert.AreEqual (1, mergedCollection.Count);
    Assert.AreSame (_dataContainer, mergedCollection[0]);
  }

  [Test]
  public void MergeEmptyCollectionAndCollection ()
  {
    DataContainerCollection secondCollection = new DataContainerCollection ();
    secondCollection.Add (_dataContainer);

    DataContainerCollection mergedCollection = _collection.Merge (secondCollection);

    Assert.AreEqual (0, mergedCollection.Count);
  }

  [Test]
  public void MergeTwoCollectionsWithEqualDataContainer ()
  {
    _collection.Add (_dataContainer);

    DataContainerCollection secondCollection = new DataContainerCollection ();
    DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
    secondCollection.Add (container);

    DataContainerCollection mergedCollection = _collection.Merge (secondCollection);

    Assert.AreEqual (1, mergedCollection.Count);
    Assert.AreSame (container, mergedCollection[0]);
  }

  [Test]
  public void MergeTwoCollections ()
  {
    _collection.Add (_dataContainer);
    DataContainer order2 = TestDataContainerFactory.CreateOrder2DataContainer ();
    _collection.Add (order2);

    DataContainerCollection secondCollection = new DataContainerCollection ();
    DataContainer order1 = TestDataContainerFactory.CreateOrder1DataContainer ();
    secondCollection.Add (order1);

    DataContainerCollection mergedCollection = _collection.Merge (secondCollection);

    Assert.AreEqual (2, mergedCollection.Count);
    Assert.AreSame (order1, mergedCollection[_dataContainer.ID]);
    Assert.AreSame (order2, mergedCollection[order2.ID]);
  }

  [Test]
  public void GetByOriginalState ()
  {
    _collection.Add (_dataContainer);
    DataContainerCollection originalContainers = _collection.GetByState (StateType.Unchanged);

    Assert.IsNotNull (originalContainers);
    Assert.AreEqual (1, originalContainers.Count);
    Assert.AreSame (_dataContainer, originalContainers[0]);
  }

  [Test]
  public void GetByChangedState ()
  {
    _collection.Add (_dataContainer);
    _collection.Add (TestDataContainerFactory.CreateCustomer1DataContainer ());

    _dataContainer["OrderNumber"] = 10;

    DataContainerCollection changedContainers = _collection.GetByState (StateType.Changed);

    Assert.IsNotNull (changedContainers);
    Assert.AreEqual (1, changedContainers.Count);
    Assert.AreSame (_dataContainer, changedContainers[0]); 
  }

  [Test]
  public void RemoveByID ()
  {
    _collection.Add (_dataContainer);
    Assert.AreEqual (1, _collection.Count);

    _collection.Remove (_dataContainer.ID);
    Assert.AreEqual (0, _collection.Count);
  }

  [Test]
  public void RemoveByDataContainer ()
  {
    _collection.Add (_dataContainer);
    Assert.AreEqual (1, _collection.Count);

    _collection.Remove (_dataContainer);
    Assert.AreEqual (0, _collection.Count);
  }

  [Test]
  public void RemoveByIndex ()
  {
    _collection.Add (_dataContainer);
    Assert.AreEqual (1, _collection.Count);

    _collection.Remove (0);
    Assert.AreEqual (0, _collection.Count);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void RemoveNullDataContainer ()
  {
    _collection.Remove ((DataContainer) null);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void RemoveNullObjectID ()
  {
    _collection.Remove ((ObjectID) null);
  }

  [Test]
  public void ContainsDataContainer ()
  {
    _collection.Add (_dataContainer);

    Assert.IsTrue (_collection.Contains (_dataContainer));
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNullDataContainer ()
  {
    _collection.Contains ((DataContainer) null);
  }

  [Test]
  public void Clear ()
  {
    _collection.Add (_dataContainer);
    Assert.AreEqual (1, _collection.Count);

    _collection.Clear ();
    Assert.AreEqual (0, _collection.Count);
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void GetByInvalidState ()
  {
    _collection.GetByState ((StateType) 1000);
  }
}
}