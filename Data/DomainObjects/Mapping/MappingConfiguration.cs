using System;
using System.Collections;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
  public class MappingConfiguration
  {
    // types

    // static members and constants

    [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
    public const string ConfigurationAppSettingKey = "Rubicon.Data.DomainObjects.Mapping.ConfigurationFile";

    [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
    public const string DefaultConfigurationFile = "Mapping.xml";


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
              s_mappingConfiguration = CreateConfigurationFromFileBasedLoader();
          }
        }

        return s_mappingConfiguration;
      }
    }

    public static void SetCurrent (MappingConfiguration mappingConfiguration)
    {
      if (mappingConfiguration != null)
      {
        if (!mappingConfiguration.Loader.ResolveTypes)
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

    [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
    public static MappingConfiguration CreateConfigurationFromFileBasedLoader ()
    {
      return CreateConfigurationFromFileBasedLoader (
          LoaderUtility.GetConfigurationFileName (ConfigurationAppSettingKey, DefaultConfigurationFile),
          true);
    }

    [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
    public static MappingConfiguration CreateConfigurationFromFileBasedLoader (string configurationFile)
    {
      return CreateConfigurationFromFileBasedLoader (configurationFile, true);
    }

    [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
    public static MappingConfiguration CreateConfigurationFromFileBasedLoader (string configurationFile, bool resolveTypes)
    {
      Type mappingLoaderType = TypeUtility.GetType ("Rubicon.Data.DomainObjects.Legacy::ConfigurationLoader.FileBasedConfigurationLoader.MappingLoader", true, false);
      IMappingLoader mappingLoader = (IMappingLoader) Activator.CreateInstance (mappingLoaderType, configurationFile, resolveTypes);
      return new MappingConfiguration (mappingLoader);
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
    private IMappingLoader _loader;

    // construction and disposing

    [Obsolete ("Use MappingConfiguration.CreateConfigurationFromFileBasedLoader (string). (Version 1.7.42)", true)]
    public MappingConfiguration (string configurationFile)
    {
      throw new InvalidOperationException ("Use MappingConfiguration.CreateConfigurationFromFileBasedLoader (string).");
    }

    [Obsolete ("Use MappingConfiguration.CreateConfigurationFromFileBasedLoader (string, bool). (Version 1.7.42)", true)]
    public MappingConfiguration (string configurationFile, bool resolveTypes)
    {
      throw new InvalidOperationException ("Use MappingConfiguration.CreateConfigurationFromFileBasedLoader (string, bool).");
    }

    public MappingConfiguration (IMappingLoader loader)
    {
      ArgumentUtility.CheckNotNull ("loader", loader);

      _loader = loader;
      _classDefinitions = _loader.GetClassDefinitions();
      _relationDefinitions = _loader.GetRelationDefinitions (_classDefinitions);

      Validate();
    }

    // methods and properties

    public void Validate()
    {
      _classDefinitions.Validate();
    }

    public IMappingLoader Loader
    {
      get { return _loader; }
    }

    /// <summary>
    /// Gets the application name that is specified in the XML configuration file. 
    /// </summary>
    [Obsolete ("Use Loader.GetApplicationName() instead. (Version 1.7.42)", true)]
    public string ApplicationName
    {
      get { throw new NotImplementedException ("Use Loader.GetApplicationName() instead."); }
    }

    /// <summary>
    /// Gets the XML configuration file.
    /// </summary>
    [Obsolete ("Use Loader.ConfigurationFile instead. (Version 1.7.42)", true)]
    public string ConfigurationFile
    {
      get { throw new NotImplementedException ("Use Loader.ConfigurationFile instead."); }
    }

    /// <summary>
    /// Gets a flag whether type names in the configuration file should be resolved to their corresponding .NET <see cref="Type"/>.
    /// </summary>
    public bool ResolveTypes
    {
      get { return _loader.ResolveTypes; }
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