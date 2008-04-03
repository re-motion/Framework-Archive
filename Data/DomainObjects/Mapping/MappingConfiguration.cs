using System;
using System.Collections;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
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
        if (s_mappingConfiguration == null)
        {
          lock (typeof (MappingConfiguration))
          {
            if (s_mappingConfiguration == null)
              s_mappingConfiguration = new MappingConfiguration (DomainObjectsConfiguration.Current.MappingLoader.CreateMappingLoader());
          }
        }

        return s_mappingConfiguration;
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
          mappingConfiguration.Validate();
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
    private bool _resolveTypes;
    
    // construction and disposing

    [Obsolete ("Use Remotion.Data.DomainObjects.Legacy.Mapping.XmlBasedMappingConfiguration.Create (string). (Version 1.7.42)", true)]
    public MappingConfiguration (string configurationFile)
    {
      throw new InvalidOperationException ("Use Remotion.Data.DomainObjects.Legacy.Mapping.XmlBasedMappingConfiguration.Create (string).");
    }

    [Obsolete ("Use Remotion.Data.DomainObjects.Legacy.Mapping.XmlBasedMappingConfiguration.Create (string, bool). (Version 1.7.42)", true)]
    public MappingConfiguration (string configurationFile, bool resolveTypes)
    {
      throw new InvalidOperationException ("Use Remotion.Data.DomainObjects.Legacy.Mapping.XmlBasedMappingConfiguration.Create (string, bool).");
    }

    public MappingConfiguration (IMappingLoader loader)
    {
      ArgumentUtility.CheckNotNull ("loader", loader);

      _classDefinitions = loader.GetClassDefinitions();
      if (_classDefinitions == null)
        throw new InvalidOperationException (string.Format ("IMappingLoader.GetClassDefinitions() evaluated and returned null."));

      _relationDefinitions = loader.GetRelationDefinitions (_classDefinitions);
      if (_relationDefinitions == null)
        throw new InvalidOperationException (string.Format ("IMappingLoader.GetRelationDefinitions (ClassDefinitionCollection) evaluated and returned null."));
      
      _resolveTypes = loader.ResolveTypes;
      
      Validate();
    }

    // methods and properties

    public void Validate()
    {
      _classDefinitions.Validate();
    }

    /// <summary>
    /// Gets the application name that is specified in the XML configuration file. 
    /// </summary>
    [Obsolete ("Querying the ApplicationName after the loading has completed is no longer supported. (Version 1.7.42)", true)]
    public string ApplicationName
    {
      get { throw new NotImplementedException ("Use Loader.GetApplicationName() instead."); }
    }

    /// <summary>
    /// Gets the XML configuration file.
    /// </summary>
    [Obsolete ("Querying the ConfigurationFile after the loading has completed is no longer supported. (Version 1.7.42)", true)]
    public string ConfigurationFile
    {
      get { throw new NotImplementedException ("Use Loader.ConfigurationFile instead."); }
    }

    /// <summary>
    /// Gets a flag whether type names in the configuration file should be resolved to their corresponding .NET <see cref="Type"/>.
    /// </summary>
    public bool ResolveTypes
    {
      get { return _resolveTypes; }
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

