using System;
using System.Collections.Generic;
using System.Text;

using Remotion.Utilities;
using Remotion.Data.DomainObjects.Legacy.CodeGenerator.Sql;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.Sql
{
  public class SqlFileBuilderMock : SqlFileBuilderBase
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing

    public SqlFileBuilderMock (MappingConfiguration mappingConfiguration, RdbmsProviderDefinition rdbmsProviderDefinition)
      : base (mappingConfiguration, rdbmsProviderDefinition)
    {
    }

    // methods and properties

    public override string GetScript ()
    {
      return string.Format ("Contents of SetupDB for StorageProvider\r\n  {0}", RdbmsProviderDefinition.Name);
    }
  }
}