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
    _classDefinition = new ClassDefinition ("Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order));
    _collection = new ClassDefinitionCollection ();
  }

  [Test]
  public void Initialize ()
  {
    Assert.IsTrue (_collection.AreResolvedTypesRequired);
  }

  [Test]
  public void AddWithResolvedType ()
  {
    Assert.AreEqual (0, _collection.Count);

    _collection.Add (_classDefinition);
    
    Assert.AreEqual (1, _collection.Count);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Collection allows only ClassDefinitions with resolved types and therefore ClassDefinition 'Order' cannot be added.")]
  public void AddWithUnresolvedType ()
  {
    ClassDefinition classDefinitionWithUnresolvedType = new ClassDefinition ("Order", "OrderTable", "StorageProvider", "UnresolvedType", false);
    _collection.Add (classDefinitionWithUnresolvedType);
  }

  [Test]
  public void AddTwiceWithSameType ()
  {
    _collection.Add (_classDefinition);

    try
    {
      _collection.Add (new ClassDefinition ("OtherID", "OtherTable", DatabaseTest.c_testDomainProviderID, typeof (Order)));
      Assert.Fail ("Expected an ArgumentException.");
    }
    catch (ArgumentException e)
    {
      Assert.AreEqual (
          "A ClassDefinition with Type 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order' is already part of this collection.\r\nParameter name: value", 
          e.Message);

      Assert.IsFalse (_collection.Contains ("OtherID"));
    }
  }

  [Test]
  public void AddTwiceWithSameClassID ()
  {
    _collection.Add (_classDefinition);
    try
    {
      _collection.Add (new ClassDefinition ("Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Customer)));
      Assert.Fail ("Expected an ArgumentException.");
    }
    catch (ArgumentException)
    {
      // Note: Do not check exception message here, because it's raised by the .NET Framework (Hashtable.Add).
      Assert.IsFalse (_collection.Contains (typeof (Customer)));
    }
  }

  [Test]
  public void TypeIndexer ()
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
  public void ContainsClassTypeTrue ()
  {
    _collection.Add (_classDefinition);
    Assert.IsTrue (_collection.Contains (typeof (Order)));
  }

  [Test]
  public void ContainsClassTypeFalse ()
  {
    Assert.IsFalse (_collection.Contains (typeof (Order)));
  }

  [Test]
  public void ContainsClassDefinitionTrue ()
  {
    _collection.Add (_classDefinition);
    Assert.IsTrue (_collection.Contains (_classDefinition));
  }

  [Test]
  public void ContainsClassDefinitionFalse ()
  {
    _collection.Add (_classDefinition);

    ClassDefinition copy = new ClassDefinition (
        _classDefinition.ID, _classDefinition.EntityName, _classDefinition.StorageProviderID, _classDefinition.ClassType, _classDefinition.BaseClass);

    Assert.IsFalse (_collection.Contains (copy));
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
}
}
