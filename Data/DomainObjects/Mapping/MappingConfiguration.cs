using System;

using Rubicon.Data.DomainObjects.Configuration.Loader;

namespace Rubicon.Data.DomainObjects.Configuration.Mapping
{
public class MappingConfiguration
{
  // types

  // static members and constants

  private static MappingConfiguration s_mappingConfiguration;

  public static MappingConfiguration Current
  {
    get 
    {
      lock (typeof (MappingConfiguration))
      {
        if (s_mappingConfiguration == null)
        {
          s_mappingConfiguration = new MappingConfiguration (MappingLoader.Create ());
        }
        
        return s_mappingConfiguration;
      }
    }
  }

  public static void SetCurrent (MappingConfiguration mappingConfiguration)
  {
    lock (typeof (MappingConfiguration))
    {
      s_mappingConfiguration = mappingConfiguration;
    }
  }

  // member fields

  private ClassDefinitionCollection _classDefinitions;
  private RelationDefinitionCollection _relationDefinitions;

  // construction and disposing

  public MappingConfiguration (MappingLoader mappingDataLoader)
  {
    _classDefinitions = mappingDataLoader.GetClassDefinitions ();
    _relationDefinitions = mappingDataLoader.GetRelationDefinitions (_classDefinitions);
  }

  // methods and properties

  public ClassDefinitionCollection ClassDefinitions
  {
    get { return _classDefinitions; }
  }

  public RelationDefinitionCollection RelationDefinitions
  {
    get { return _relationDefinitions; }
  }
}
}