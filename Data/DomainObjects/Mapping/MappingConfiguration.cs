using System;
using System.Collections;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
public class MappingConfiguration : ConfigurationBase
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

  public MappingConfiguration (string configurationFile, string schemaFile) 
      : this (new MappingLoader (configurationFile, schemaFile))
  {
  }

  public MappingConfiguration (MappingLoader loader) : base (loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

    _classDefinitions = loader.GetClassDefinitions ();
    _relationDefinitions = loader.GetRelationDefinitions (_classDefinitions);
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

  public bool Contains (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    // TODO: Use _classDefinitions.Contains, after it implements object.Equals.
    ClassDefinition foundClassDefinition = _classDefinitions[classDefinition.ID];
    return object.ReferenceEquals (classDefinition, foundClassDefinition);
  }

  public bool Contains (PropertyDefinition propertyDefinition)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    if (propertyDefinition.ClassDefinition == null)
      return false;

    ClassDefinition foundClassDefinition = _classDefinitions[propertyDefinition.ClassDefinition.ID];
    if (foundClassDefinition == null)
      return false;

    // TODO: Use PropertyDefinitionCollection.Contains, after it implements object.Equals.
    return object.ReferenceEquals (propertyDefinition, foundClassDefinition[propertyDefinition.PropertyName]);
  }

  public bool Contains (RelationDefinition relationDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

    // TODO: Use _relationDefinitions.Contains, after it implements object.Equals.
    RelationDefinition foundRelationDefinition = _relationDefinitions[relationDefinition.ID];
    return object.ReferenceEquals (relationDefinition, foundRelationDefinition);
  }

  public bool Contains (IRelationEndPointDefinition relationEndPointDefinition)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

    if (relationEndPointDefinition.RelationDefinition == null)
      return false;

    RelationDefinition foundRelationDefinition = _relationDefinitions[relationEndPointDefinition.RelationDefinition.ID];
    if (foundRelationDefinition == null)
      return false;

    return ((IList) foundRelationDefinition.EndPointDefinitions).Contains (relationEndPointDefinition);
  }
}
}