using System;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests
{
public class UnitTestStorageProviderStub : StorageProvider
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public UnitTestStorageProviderStub (UnitTestStorageProviderStubDefinition definition) : base (definition)
  {
  }

  // methods and properties

  public override DataContainer LoadDataContainer (ObjectID id)
  {
    DataContainer container = DataContainer.CreateForExisting (id, null);
    PropertyDefinition definition = new PropertyDefinition ("Name", "Name", "string", new NaInt32 (100));
    container.PropertyValues.Add (new PropertyValue (definition, "Max Sachbearbeiter"));
    return container;
  }

  public override void Save (DataContainerCollection dataContainers)
  {
  }

  public override void SetTimestamp (DataContainerCollection dataContainers)
  {
  }

  public override DataContainerCollection LoadDataContainersByRelatedID (ClassDefinition classDefinition, string propertyName, ObjectID relatedID)
  {
    return null;
  }

  public override void BeginTransaction ()
  {
  }

  public override void Commit ()
  {
  }

  public override void Rollback()
  {
  }

  public override DataContainer CreateNewDataContainer (ClassDefinition classDefinition)
  {
    return null;
  }
}
}
