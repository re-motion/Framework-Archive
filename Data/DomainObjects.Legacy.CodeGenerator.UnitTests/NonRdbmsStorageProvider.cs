using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests
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

    public override Remotion.Data.DomainObjects.DataManagement.DataContainerCollection LoadDataContainersByRelatedID (Remotion.Data.DomainObjects.Mapping.ClassDefinition classDefinition, string propertyName, ObjectID relatedID)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override void Save (Remotion.Data.DomainObjects.DataManagement.DataContainerCollection dataContainers)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override void SetTimestamp (Remotion.Data.DomainObjects.DataManagement.DataContainerCollection dataContainers)
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

    public override ObjectID CreateNewObjectID (Remotion.Data.DomainObjects.Mapping.ClassDefinition classDefinition)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override Remotion.Data.DomainObjects.DataManagement.DataContainerCollection ExecuteCollectionQuery (Remotion.Data.DomainObjects.Queries.IQuery query)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override object ExecuteScalarQuery (Remotion.Data.DomainObjects.Queries.IQuery query)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }

    public override DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> ids)
    {
      throw new NotSupportedException ("The method or operation is not supported.");
    }
  }
}
