using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class ClassDefinitionCollectionTest
{
  // types

  // static members and constants

  // member fields

  private ClassDefinitionCollection _collection;
  private ClassDefinition _classDefinition;

  // construction and disposing

  public ClassDefinitionCollectionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _classDefinition = new ClassDefinition ("Order", "Order", typeof (Order), DatabaseTest.c_testDomainProviderID);
    _collection = new ClassDefinitionCollection ();
  }

  [Test]
  public void Add ()
  {
    _collection.Add (_classDefinition);
    Assert.AreEqual (1, _collection.Count);
  }

  [Test]
  public void ClassDefinitionIndexer ()
  {
    _collection.Add (_classDefinition);
    Assert.AreSame (_classDefinition, _collection[typeof (Order)]);  
  }

  [Test]
  public void NumericIndexer ()
  {
    _collection.Add (_classDefinition);
    Assert.AreSame (_classDefinition, _collection[0]);  
  }

  [Test]
  public void ContainsTrue ()
  {
    _collection.Add (_classDefinition);
    Assert.IsTrue (_collection.Contains (typeof (Order)));
  }

  [Test]
  public void ContainsFalse ()
  {
    Assert.IsFalse (_collection.Contains (typeof (Order)));
  }

  [Test]
  public void CopyConstructor ()
  {
    _collection.Add (_classDefinition);

    ClassDefinitionCollection copiedCollection = new ClassDefinitionCollection (_collection, false);

    Assert.AreEqual (1, copiedCollection.Count);
    Assert.AreSame (_classDefinition, copiedCollection[0]);
  }

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Mapping does not contain class 'Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ClassDefinitionCollectionTest'.")]
  public void GetMandatoryForInvalidClass ()
  {
    TestMappingConfiguration.Current.ClassDefinitions.GetMandatory (this.GetType ());
  }

  [Test]
  [ExpectedException (typeof (MappingException), "Mapping does not contain class 'Zaphod'.")]
  public void GetMandatoryForInvalidClassID ()
  {
    TestMappingConfiguration.Current.ClassDefinitions.GetMandatory ("Zaphod");
  }

  [Test]
  public void ContainsClassDefinition ()
  {
    _collection.Add (_classDefinition);
    
    Assert.IsTrue (_collection.Contains (_classDefinition));    
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNullClassDefinition ()
  {
    _collection.Contains ((ClassDefinition) null);
  }

  [Test]
  public void ContainsClassID ()
  {
    _collection.Add (_classDefinition);
    Assert.IsTrue (_collection.Contains (_classDefinition.ID));
  }

  [Test]
  public void AddTwiceWithSameClassID ()
  {
    _collection.Add (_classDefinition);
    try
    {
      _collection.Add (new ClassDefinition ("Order", "Order", typeof (Customer), DatabaseTest.c_testDomainProviderID));
      Assert.Fail ("Expected an ArgumentException.");
    }
    catch (ArgumentException)
    {
      Assert.IsFalse (_collection.Contains (typeof (Customer)));
    }
  }
}
}
