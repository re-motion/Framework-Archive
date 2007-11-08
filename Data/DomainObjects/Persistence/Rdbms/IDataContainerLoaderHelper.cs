using System.Collections.Generic;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
  // Required for mocking
  public interface IDataContainerLoaderHelper
  {
    SelectCommandBuilder GetSelectCommandBuilder (RdbmsProvider provider, string entityName, IEnumerable<ObjectID> objectIDs);

    ConcreteTableInheritanceRelationLoader GetConcreteTableInheritanceRelationLoader (RdbmsProvider provider, ClassDefinition classDefinition,
        PropertyDefinition propertyDefinition, ObjectID relatedID);
  }
}