using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.StorageProviders
{
[TestFixture]
public class StorageProviderDefinitionCollectionTest
{
  // types

  // static members and constants

  // member fields

  private StorageProviderDefinitionCollection _collection;
  private StorageProviderDefinition _definition;

  // construction and disposing

  public StorageProviderDefinitionCollectionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _collection = new StorageProviderDefinitionCollection ();

    _definition = new UnitTestStorageProviderStubDefinition (
        "UnitTestStorageProviderStub", typeof (UnitTestStorageProviderStub));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "StorageProviderDefinition 'UnitTestStorageProviderStub' already exists in collection.\r\nParameter name: value")]
  public void DuplicateStorageProverIDs ()
  {
    _collection.Add (_definition);

    _collection.Add (new UnitTestStorageProviderStubDefinition (
        "UnitTestStorageProviderStub", typeof (UnitTestStorageProviderStub)));
  }

  [Test]
  public void ContainsStorageProviderDefinitionTrue ()
  {
    _collection.Add (_definition);
    
    Assert.IsTrue (_collection.Contains (_definition));    
  }

  [Test]
  public void ContainsStorageProviderDefinitionFalse ()
  {
    _collection.Add (_definition);
    
    StorageProviderDefinition copy = new UnitTestStorageProviderStubDefinition (
        "UnitTestStorageProviderStub", typeof (UnitTestStorageProviderStub));

    Assert.IsFalse (_collection.Contains (copy));    
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNullStorageProviderDefinition ()
  {
    _collection.Contains ((StorageProviderDefinition) null);
  }

  [Test]
  public void GetMandatory ()
  {
    _collection.Add (_definition);

    Assert.AreSame (_definition, _collection.GetMandatory (_definition.ID));
  }

  [Test]
  [ExpectedException (typeof (StorageProviderConfigurationException), 
      "StorageProviderDefinition 'UnitTestStorageProviderStub' does not exist.")]
  public void GetMandatoryForNonExisting ()
  {
    _collection.GetMandatory ("UnitTestStorageProviderStub");
  }
}
}
