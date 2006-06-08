using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests
{
  public class NonRdbmsStorageProvider : StorageProvider
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public NonRdbmsStorageProvider (NonRdbmsStorageProviderDefinition definition) : base (definition)
    {
    }

    // methods and properties


    public override DataContainer LoadDataContainer (ObjectID id)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override Rubicon.Data.DomainObjects.DataManagement.DataContainerCollection LoadDataContainersByRelatedID (Rubicon.Data.DomainObjects.Mapping.ClassDefinition classDefinition, string propertyName, ObjectID relatedID)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override void Save (Rubicon.Data.DomainObjects.DataManagement.DataContainerCollection dataContainers)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override void SetTimestamp (Rubicon.Data.DomainObjects.DataManagement.DataContainerCollection dataContainers)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override void BeginTransaction ()
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override void Commit ()
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override void Rollback ()
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override DataContainer CreateNewDataContainer (Rubicon.Data.DomainObjects.Mapping.ClassDefinition classDefinition)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override Rubicon.Data.DomainObjects.DataManagement.DataContainerCollection ExecuteCollectionQuery (Rubicon.Data.DomainObjects.Queries.IQuery query)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override object ExecuteScalarQuery (Rubicon.Data.DomainObjects.Queries.IQuery query)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }
  }
}
