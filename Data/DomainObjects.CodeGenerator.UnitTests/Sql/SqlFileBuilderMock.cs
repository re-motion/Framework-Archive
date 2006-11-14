using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.Sql
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
      return string.Format ("Contents of SetupDB for StorageProvider\r\n  {0}", RdbmsProviderDefinition.ID);
    }
  }
}