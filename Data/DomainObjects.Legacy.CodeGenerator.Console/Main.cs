using System;
using System.Configuration;
using System.IO;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Legacy.CodeGenerator.Sql;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.XmlBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Text.CommandLine;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator.Console
{
  public class MainClass
  {
    private static int Main (string[] args)
    {
      Arguments arguments;
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      try
      {
        arguments = (Arguments) parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        System.Console.WriteLine (e.Message);
        System.Console.WriteLine ("Usage:");
        System.Console.WriteLine (parser.GetAsciiSynopsis (Environment.GetCommandLineArgs()[0], 79));
        return 1;
      }

      try
      {
        ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
        fileMap.ExeConfigFilename = arguments.ConfigFile;
        System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration (fileMap, ConfigurationUserLevel.None);
        ConfigurationWrapper.SetCurrent (ConfigurationWrapper.CreateFromConfigurationObject (configuration));

        PersistenceConfiguration persistenceConfiguration = DomainObjectsConfiguration.Current.Storage;

        string mappingFilename = Path.Combine (
            Path.GetDirectoryName (arguments.ConfigFile),
            ConfigurationWrapper.Current.GetAppSetting (MappingLoader.ConfigurationAppSettingKey) ?? MappingLoader.DefaultConfigurationFile);
        MappingConfiguration mappingConfiguration = XmlBasedMappingConfiguration.Create (mappingFilename, false);

        if ((arguments.Mode & OperationMode.Sql) != 0)
        {
          Type sqlFileBuilderType = TypeUtility.GetType (arguments.SqlBuilderTypeName, true, false);
          SqlFileBuilderBase.Build (sqlFileBuilderType, mappingConfiguration, persistenceConfiguration, arguments.SqlOutput);
        }

        if ((arguments.Mode & OperationMode.DomainModel) != 0)
        {
          DomainModelBuilder.Build (
              mappingConfiguration,
              arguments.ClassOutput,
              arguments.DomainObjectBaseClass,
              arguments.DomainObjectCollectionBaseClass,
              arguments.Serializable,
              arguments.MultiLingualResources);
        }
      }
      catch (Exception e)
      {
        if (arguments.Verbose)
        {
          System.Console.Error.WriteLine ("Execution aborted. Exception stack:");
          for (; e != null; e = e.InnerException)
            System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace);
        }
        else
        {
          System.Console.Error.WriteLine ("Execution aborted: {0}", e.Message);
        }
        return 1;
      }
      return 0;
    }
  }
}