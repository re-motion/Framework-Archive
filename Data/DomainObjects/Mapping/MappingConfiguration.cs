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
      if (mappingConfiguration != null)
      {
        if (!mappingConfiguration.ResolveTypes)
          throw CreateArgumentException ("mappingConfiguration", "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.");

        try
        {
          mappingConfiguration.Validate ();
        }
        catch (Exception ex)
        {
          throw CreateArgumentException (
              ex, "mappingConfiguration", "The specified MappingConfiguration is invalid due to the following reason: '{0}'.", ex.Message);
        }
      }

      lock (typeof (MappingConfiguration))
      {

        s_mappingConfiguration = mappingConfiguration;
      }
    }

    private static ArgumentException CreateArgumentException (Exception innerException, string argumentName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), argumentName, innerException);
    }

    private static ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
    {
      return CreateArgumentException (null, argumentName, message, args);
    }

    // member fields

    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;

    // construction and disposing

    public MappingConfiguration (string configurationFile, string schemaFile)
      : this (configurationFile, schemaFile, true)
    {
    }

    public MappingConfiguration (string configurationFile, string schemaFile, bool resolveTypes)
      : this (new MappingLoader (configurationFile, schemaFile, resolveTypes))
    {
    }

    public MappingConfiguration (MappingLoader loader)
      : base (loader)
    {
      ArgumentUtility.CheckNotNull ("loader", loader);

      _classDefinitions = loader.GetClassDefinitions ();
      _relationDefinitions = loader.GetRelationDefinitions (_classDefinitions);

      Validate ();
    }

    // methods and properties

    public void Validate ()
    {
      foreach (ClassDefinition classDefinition in _classDefinitions)
        classDefinition.Validate ();
    }

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

      return _classDefinitions.Contains (classDefinition);
    }

    public bool Contains (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      if (propertyDefinition.ClassDefinition == null)
        return false;

      ClassDefinition foundClassDefinition = _classDefinitions[propertyDefinition.ClassDefinition.ID];
      if (foundClassDefinition == null)
        return false;

      return foundClassDefinition.Contains (propertyDefinition);
    }

    public bool Contains (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      return _relationDefinitions.Contains (relationDefinition);
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