using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests
{
  public class UnitTestStorageProviderStub : StorageProvider
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public UnitTestStorageProviderStub (UnitTestStorageProviderStubDefinition definition)
      : base (definition)
    {
    }

    // methods and properties

    public override DataContainer LoadDataContainer (ObjectID id)
    {
      DataContainer container = DataContainer.CreateForExisting (id, null);
      PropertyDefinition definition = new XmlBasedPropertyDefinition ((XmlBasedClassDefinition) container.ClassDefinition, "Name", "Name", "string", 100);
      container.PropertyValues.Add (new PropertyValue (definition, "Max Sachbearbeiter"));
      return container;
    }

    public override DataContainerCollection ExecuteCollectionQuery (IQuery query)
    {
      return null;
    }

    public override object ExecuteScalarQuery (IQuery query)
    {
      return null;
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

    public override void Rollback ()
    {
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      return null;
    }

    public new object GetFieldValue (DataContainer dataContainer, string propertyName, ValueAccess valueAccess)
    {
      return base.GetFieldValue (dataContainer, propertyName, valueAccess);
    }

    public override DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> ids)
    {
      throw new NotImplementedException();
    }
  }
}
