using System;
using System.IO;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator.MigrationToReflectionBasedMapping
{
  public class MigrationDomainModelBuilder : DomainModelBuilder
  {
    public MigrationDomainModelBuilder ()
    {
    }

    protected override DomainObjectBuilder CreateDomainObjectBuilder (MappingConfiguration mappingConfiguration, TextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("writer", writer);

      return new MigrationDomainObjectBuilder (mappingConfiguration, writer);
    }
  }
}