using System;
using System.IO;
using System.Collections;
using System.Reflection;
using Rubicon.Text.CommandLine;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Console
{
  public class MainClass
  {
    static int Main (string[] args)
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
        System.Console.WriteLine (parser.GetAsciiSynopsis (System.Environment.GetCommandLineArgs ()[0], 79));
        return 1;
      }

      try
      {
        StorageProviderConfiguration storageProviderConfiguration = new StorageProviderConfiguration (
            Path.Combine (arguments.ConfigDirectory, StorageProviderConfigurationLoader.DefaultConfigurationFile),
            Path.Combine (arguments.SchemaDirectory, StorageProviderConfigurationLoader.DefaultSchemaFile));

        MappingConfiguration mappingConfiguration = new MappingConfiguration (
            Path.Combine (arguments.ConfigDirectory, MappingLoader.DefaultConfigurationFile),
            Path.Combine (arguments.SchemaDirectory, MappingLoader.DefaultSchemaFile),
            false);

        if ((arguments.Mode & OperationMode.Sql) != 0)
          SqlBuilder.Build (mappingConfiguration, storageProviderConfiguration, arguments.SqlOutput);

        if ((arguments.Mode & OperationMode.DomainModel) != 0)
        {
          DomainModelBuilder.Build (
              mappingConfiguration,
              arguments.ClassOutput,
              arguments.DomainObjectBaseClass, arguments.DomainObjectCollectionBaseClass,
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
          {
            System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType ().FullName, e.Message, e.StackTrace);
          }
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
