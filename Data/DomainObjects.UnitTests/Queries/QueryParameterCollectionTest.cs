using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects.UnitTests.Queries
{
[TestFixture]
public class QueryParameterCollectionTest
{
  // types

  // static members and constants

  // member fields

  private QueryParameterCollection _collection;
  private QueryParameter _parameter;

  // construction and disposing

  public QueryParameterCollectionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _parameter = new QueryParameter ("name", "value");
    _collection = new QueryParameterCollection ();
  }

  [Test]
  public void Add ()
  {
    _collection.Add (_parameter);
    Assert.AreEqual (1, _collection.Count);
  }

  [Test]
  public void QueryParameterIndexer ()
  {
    _collection.Add (_parameter);
    Assert.AreSame (_parameter, _collection[_parameter.Name]);  
  }

  [Test]
  public void NumericIndexer ()
  {
    _collection.Add (_parameter);
    Assert.AreSame (_parameter, _collection[0]);  
  }

  [Test]
  public void ContainsTrue ()
  {
    _collection.Add (_parameter);
    Assert.IsTrue (_collection.Contains (_parameter.Name));
  }

  [Test]
  public void ContainsFalse ()
  {
    Assert.IsFalse (_collection.Contains (_parameter.Name));
  }

  [Test]
  public void CopyConstructor ()
  {
    _collection.Add (_parameter);

    QueryParameterCollection copiedCollection = new QueryParameterCollection (_collection, false);

    Assert.AreEqual (1, copiedCollection.Count);
    Assert.AreSame (_parameter, copiedCollection[0]);
  }

  [Test]
  public void ContainsQueryParameter ()
  {
    _collection.Add (_parameter);
    
    Assert.IsTrue (_collection.Contains (_parameter));    
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNullQueryParameter ()
  {
    _collection.Contains ((QueryParameter) null);
  }

  [Test]
  public void ContainsQueryParameterName ()
  {
    _collection.Add (_parameter);
    
    Assert.IsTrue (_collection.Contains (_parameter.Name));    
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNullQueryParameterName ()
  {
    _collection.Contains ((string) null);
  }
}
}
