using System;
using System.IO;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.RdbmsTools.SchemaGeneration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.RdbmsTools
{
  /// <summary>
  /// The <see cref="RdbmsToolsRunner"/> type contains the encapsulates the execution of the various functionality provided by the 
  /// <b>Rubicon.Data.DomainObjects.RdbmsTools</b> assembly.
  /// </summary>
  [Serializable]
  public class RdbmsToolsRunner : AppDomainRunnerBase
  {
    public static RdbmsToolsRunner Create (RdbmsToolsParameter rdbmsToolsParameter)
    {
      AppDomainSetup appDomainSetup = new AppDomainSetup();
      appDomainSetup.ApplicationName = "RdbmsTools";
      appDomainSetup.ApplicationBase = rdbmsToolsParameter.BaseDirectory;
      appDomainSetup.DynamicBase = Path.Combine (Path.GetTempPath(), "Rubicon");
      if (!string.IsNullOrEmpty (rdbmsToolsParameter.ConfigFile))
      {
        appDomainSetup.ConfigurationFile = Path.IsPathRooted (rdbmsToolsParameter.ConfigFile)
                                               ? rdbmsToolsParameter.ConfigFile
                                               : Path.Combine (rdbmsToolsParameter.BaseDirectory, rdbmsToolsParameter.ConfigFile);
      }
      return new RdbmsToolsRunner (appDomainSetup, rdbmsToolsParameter);
    }

    private readonly RdbmsToolsParameter _rdbmsToolsParameter;

    protected RdbmsToolsRunner (AppDomainSetup appDomainSetup, RdbmsToolsParameter rdbmsToolsParameter)
        : base (appDomainSetup)
    {
      _rdbmsToolsParameter = rdbmsToolsParameter;
    }

    protected override void CrossAppDomainCallbackHandler ()
    {
      InitializeConfiguration();

      if ((_rdbmsToolsParameter.Mode & OperationMode.BuildSchema) != 0)
        BuildSchema();
    }

    protected virtual void InitializeConfiguration ()
    {
      DomainObjectsConfiguration.SetCurrent (
          new FakeDomainObjectsConfiguration (DomainObjectsConfiguration.Current.MappingLoader, GetPersistenceConfiguration()));

      MappingConfiguration.SetCurrent (new MappingConfiguration (DomainObjectsConfiguration.Current.MappingLoader.CreateMappingLoader()));
    }

    protected PersistenceConfiguration GetPersistenceConfiguration ()
    {
      PersistenceConfiguration persistenceConfiguration = DomainObjectsConfiguration.Current.Storage;
      if (persistenceConfiguration.StorageProviderDefinition == null)
      {
        ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
        RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Default", typeof (SqlProvider), "Initial Catalog=DatebaseName;");
        storageProviderDefinitionCollection.Add (providerDefinition);

        persistenceConfiguration = new PersistenceConfiguration (storageProviderDefinitionCollection, providerDefinition);
      }

      return persistenceConfiguration;
    }

    protected virtual void BuildSchema ()
    {
      Type sqlFileBuilderType = TypeUtility.GetType (_rdbmsToolsParameter.SchemaFileBuilderTypeName, true, false);
      FileBuilderBase.Build (
          sqlFileBuilderType,
          MappingConfiguration.Current,
          DomainObjectsConfiguration.Current.Storage,
          _rdbmsToolsParameter.SchemaOutputDirectory);
    }
  }
}